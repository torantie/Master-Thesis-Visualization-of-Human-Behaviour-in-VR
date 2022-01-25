using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Timers;
using UnityEngine;

public class AudioRecorder : MonoBehaviour
{
    [SerializeField]
    private LoggingManager m_loggingManager;

    /// <summary>
    /// Cannot be longer than 3600s = 1 hour
    /// </summary>
    [SerializeField, Range(1, 3599)]
    private int m_maxClipLengthInSeconds;

    private AudioClip m_currentClip;

    private readonly List<AudioClip> m_audioClipsToSave = new List<AudioClip>();

    private readonly Dictionary<int, (AudioClip audioClip, float clipLengthInSeconds, AudioFileWriter audioFileWriter)> m_audioClipsToSavePending =
        new Dictionary<int, (AudioClip audioClip, float clipLengthInSeconds, AudioFileWriter audioFileWriter)>();

    private readonly object m_audioClipsToSavePendingSync = new object();

    private int m_recordingFreq = 44100;

    private TextFileWriter m_textFileWriter;

    private RecordingInfo m_recordingInfo;

    private RecordingSession m_currentRecordingSession;

    private DateTime m_lastElapsedDateTime;

    public bool IsRecording { get; private set; }


    // Update is called once per frame
    void Update()
    {
        if (IsRecording)
        {
            var now = DateTime.UtcNow;
            var elapsedTime = (now - m_lastElapsedDateTime).TotalSeconds;
            //Debug.LogFormat("elapsedTime: {0}, m_lastElapsedDateTime: {1}, now: {2}, ", elapsedTime, m_lastElapsedDateTime, now);

            if (elapsedTime >= m_maxClipLengthInSeconds)
            {
                OnReachedMaxClipLength();
                m_lastElapsedDateTime = now;
            }
        }
    }

    public void StartRecording()
    {
        try
        {
            if (Application.HasUserAuthorization(UserAuthorization.Microphone))
            {
                Debug.Log("User authorized Microphone usage.");

                if (!IsRecording)
                {

                    Microphone.GetDeviceCaps("", out var minFreq, out var maxFreq);

                    Debug.LogFormat("Microphone.GetDeviceCaps minFreq: {0}, maxFreq: {0}", minFreq, maxFreq);

                    //https://docs.unity3d.com/ScriptReference/Microphone.GetDeviceCaps.html
                    // Value 0 for both min and max indicates support for any frequency. Just take 44100 in that case.
                    if (maxFreq == 0 && minFreq == 0)
                    {
                        m_recordingFreq = 44100;
                    }
                    else
                    {
                        m_recordingFreq = maxFreq;
                    }

                    Debug.LogFormat("Before start: {0}", DateTime.UtcNow);
                    m_currentClip = Microphone.Start(null, false, m_maxClipLengthInSeconds, m_recordingFreq);
                    Debug.LogFormat("After start: {0}", DateTime.UtcNow);
                    InitRecordingInfo();
                    m_lastElapsedDateTime = m_currentRecordingSession.RecordingStart;


                    IsRecording = true;
                    Debug.LogFormat("IsRecording freq: {0}, recording start: {1}, clip length in seconds: {2}",
                        m_currentRecordingSession.recordingFrequency,
                        m_currentRecordingSession.RecordingStart,
                        m_maxClipLengthInSeconds);
                }
                else
                {
                    Debug.Log("StartRecording: AudioRecorder is already recording.");
                }
            }
            else
            {
                Debug.LogWarning("StartRecording: User did not authorize Microphone usage.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            SaveAndStopRecording(true);
        }
    }

    public void SaveAndStopRecording(bool a_inParallel)
    {
        try
        {
            if (IsRecording)
            {
                Microphone.End(null);
                IsRecording = false;
                var recordingStop = DateTime.UtcNow.Ticks;
                Debug.LogFormat("Recording stopped: {0}", DateTime.UtcNow);

                m_audioClipsToSave.Add(m_currentClip);

                var clipsToSave = PrepareClipsToSave(recordingStop);
                var audioFileWriter = new AudioFileWriter(m_loggingManager.StudyManager);

                if (a_inParallel)
                {
                    SaveClipsParallel(clipsToSave, audioFileWriter);
                }
                else
                {
                    SaveClips(clipsToSave, audioFileWriter);
                }
                Debug.Log("SaveAndStopRecording: Saved and stopped recording.");
            }
            else
            {
                Debug.Log("SaveAndStopRecording: AudioRecorder is not recording.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    private void InitRecordingInfo()
    {
        if (m_recordingInfo == null)
        {
            var textFileReader = new TextFileReader(m_loggingManager.StudyManager);
            if (textFileReader.TryReadFromRecordingInfo(out RecordingInfo recordingInfo))
            {
                m_recordingInfo = recordingInfo;
            }
            else
            {
                m_recordingInfo = new RecordingInfo();
            }
        }
        var recordingSession = new RecordingSession
        {
            recordingStart = DateTime.UtcNow.Ticks,
            recordingFrequency = m_recordingFreq,
            taskId = m_loggingManager.StudyManager.TaskId,
            participantId = m_loggingManager.StudyManager.LocalStudyParticipant.id,
            sessionId = m_loggingManager.StudyManager.CurrentStudySession.id,
        };
        m_recordingInfo.recordingSessions.Add(recordingSession);
        m_currentRecordingSession = recordingSession;

        m_textFileWriter = new TextFileWriter(m_loggingManager.StudyManager);

        m_textFileWriter.WriteToRecordingInfo(m_recordingInfo, true, true);
    }

    private void OnReachedMaxClipLength()
    {
        try
        {
            Debug.Log("AudioRecorder timer elapsed.");

            var currentClip = m_currentClip;
            Microphone.End(null);
            m_currentClip = Microphone.Start(null, false, m_maxClipLengthInSeconds, m_recordingFreq);
            m_audioClipsToSave.Add(currentClip);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    private void SaveClips(Dictionary<int, (AudioClip audioClip, float clipLengthInSeconds)> a_clipsToSave, AudioFileWriter a_audioFileWriter)
    {
        try
        {
            foreach (var clipToSave in a_clipsToSave)
            {
                SaveClip(a_audioFileWriter, clipToSave.Key, clipToSave.Value.audioClip, clipToSave.Value.clipLengthInSeconds);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    private void SaveClip(AudioFileWriter a_audioFileWriter, int a_clipId, AudioClip a_clipToSave, float a_clipLengthInSeconds)
    {
        try
        {
            var audioClip = a_clipToSave;
            if (a_clipLengthInSeconds != a_clipToSave.length)
            {
                audioClip = TrimAndCreateAudioClip(audioClip, a_clipLengthInSeconds);
            }

            a_audioFileWriter.WriteAudioFile(audioClip, a_clipId);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    private Dictionary<int, (AudioClip audioClip, float clipLengthInSeconds)> PrepareClipsToSave(long a_recordingStop)
    {
        Dictionary<int, (AudioClip audioClip, float clipLengthInSeconds)> clipsToSave = new Dictionary<int, (AudioClip audioClip, float clipLengthInSeconds)>();
        m_currentRecordingSession.recordingStop = a_recordingStop;
        var recordingLengthInSeconds = (float)m_currentRecordingSession.RecordingLengthInSeconds;

        for (int i = 0; i < m_audioClipsToSave.Count; i++)
        {
            var clipId = i + m_recordingInfo.amountOfClips;
            var audioClipToSave = m_audioClipsToSave[i];
            m_currentRecordingSession.clipIds.Add(clipId);

            float clipLengthInSeconds;
            if (i == m_audioClipsToSave.Count - 1)
            {
                clipLengthInSeconds = Mathf.Ceil(recordingLengthInSeconds);
                Debug.LogFormat("PrepareClipsToSave: Lastclip clipId = {0}, recordingLengthInSeconds = {1}", clipId, recordingLengthInSeconds);
            }
            else
            {
                recordingLengthInSeconds -= audioClipToSave.length;
                clipLengthInSeconds = audioClipToSave.length;
            }

            Debug.LogFormat("PrepareClipsToSave: clipId = {0}, clipLengthInSeconds = {1}", clipId, clipLengthInSeconds);
            clipsToSave.Add(clipId, (audioClipToSave, clipLengthInSeconds));
        }
        m_recordingInfo.amountOfClips += m_audioClipsToSave.Count;

        m_textFileWriter.WriteToRecordingInfo(m_recordingInfo, true, true);

        m_audioClipsToSave.Clear();
        return clipsToSave;
    }

    private void SaveClipsParallel(Dictionary<int, (AudioClip audioClip, float clipLengthInSeconds)> a_clipsToSave, AudioFileWriter a_audioFileWriter)
    {
        foreach (var clipToSave in a_clipsToSave)
        {
            var audioClipToSave = clipToSave.Value.audioClip;
            var clipLengthInSeconds = clipToSave.Value.clipLengthInSeconds;
            lock (m_audioClipsToSavePendingSync)
            {
                m_audioClipsToSavePending[clipToSave.Key] = (audioClipToSave, clipLengthInSeconds, a_audioFileWriter);
            }
            var samplesLength = clipToSave.Value.audioClip.samples;
            //Trimming
            if (clipLengthInSeconds != clipToSave.Value.audioClip.length)
            {
                samplesLength = GetTrimmedLengthSamples(audioClipToSave, clipToSave.Value.clipLengthInSeconds);
            }

            StartCoroutine(SaveAudioClipSamples(clipToSave.Key, audioClipToSave, samplesLength));
        }
    }

    private IEnumerator SaveAudioClipSamples(int a_audioClipId, AudioClip a_audioClipToSave, int a_samplesLength)
    {
        List<float> samples = new List<float>();
        var offset = 0;
        var denominator = a_samplesLength <= 10000 ? 0 : a_samplesLength / 10000;
        var partSizes = DivideEvenly(a_samplesLength, denominator);
        Debug.LogFormat("GetSamples partSizes.Count: {0}, denominator: {1}, samplesLength: {2}", partSizes.Count(), denominator, a_samplesLength);

        foreach (var partSize in partSizes)
        {
            var samplePart = new float[partSize];
            a_audioClipToSave.GetData(samplePart, offset);
            yield return null;
            samples.AddRange(samplePart);
            offset += partSize;
        }
        Debug.LogFormat("GetSamples clip to float array finished. a_samples.Count: {0}", samples.Count);
        var frequency = a_audioClipToSave.frequency;
        var channels = a_audioClipToSave.channels;

        Thread t = new Thread(() =>
        {
            (AudioClip audioClip, float clipLengthInSeconds, AudioFileWriter audioFileWriter) clipInformation;
            lock (m_audioClipsToSavePendingSync)
            {
                if (!m_audioClipsToSavePending.TryGetValue(a_audioClipId, out clipInformation))
                {
                    return;
                }
                m_audioClipsToSavePending.Remove(a_audioClipId);
            }
            Thread.Sleep(10000);
            //Todo remove to array
            Debug.LogFormat("Saving clip with id: {0}", a_audioClipId);
            clipInformation.audioFileWriter.WriteAudioFile(frequency, channels, a_samplesLength, samples.ToArray(), a_audioClipId);
        });
        t.Start();
    }

    /// <summary>
    /// https://stackoverflow.com/questions/577427/evenly-divide-in-c-sharp
    /// </summary>
    /// <param name="a_numerator"></param>
    /// <param name="a_denominator"></param>
    /// <returns></returns>
    private IEnumerable<int> DivideEvenly(int a_numerator, int a_denominator)
    {
        if (a_denominator == 0)
        {
            yield return a_numerator;
            yield break;
        }

        int div = Math.DivRem(a_numerator, a_denominator, out int rem);

        for (int i = 0; i < a_denominator; i++)
        {
            yield return i < rem ? div + 1 : div;
        }
    }

    /// <summary>
    /// https://stackoverflow.com/questions/19047529/dynamic-size-for-microphone-input
    /// </summary>
    /// <param name="a_audioClip"></param>
    /// <returns></returns>
    private AudioClip TrimAndCreateAudioClip(AudioClip a_audioClip, float a_timeSinceRecordStarted)
    {
        try
        {
            var (samples, lengthSamples) = TrimAudioClipSamples(a_audioClip, a_timeSinceRecordStarted);

            var trimmedClip = AudioClip.Create("", lengthSamples, a_audioClip.channels, a_audioClip.frequency, false);
            trimmedClip.SetData(samples, 0);

            return trimmedClip;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }

        return a_audioClip;
    }

    private (float[] samples, int lengthSamples) TrimAudioClipSamples(AudioClip a_audioClip, float a_timeSinceRecordStarted)
    {
        try
        {
            int lengthSamples = GetTrimmedLengthSamples(a_audioClip, a_timeSinceRecordStarted);
            float[] samples = new float[lengthSamples];
            a_audioClip.GetData(samples, 0);

            return (samples, lengthSamples);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }

        return (new float[0], 0);
    }

    private int GetTrimmedLengthSamples(AudioClip a_audioClip, float a_timeSinceRecordStarted)
    {
        float clipLengthInSeconds = a_audioClip.length;
        float samplesLength = a_audioClip.samples;
        float samplesPerSec = (float)samplesLength / clipLengthInSeconds;
        var lengthSamples = (int)(a_timeSinceRecordStarted * samplesPerSec);

        Debug.LogFormat("GetTrimmedLengthSamples: lengthSamples = {0}, a_timeSinceRecordStarted = {1}, clipLengthInSeconds = {2}"
            , lengthSamples, a_timeSinceRecordStarted, clipLengthInSeconds);
        return lengthSamples;
    }

    private void OnApplicationQuit()
    {
        SaveAndStopRecording(false);

        //Todo m_audioClipsToConvertPending need the right path when saving. Problematic if they save for current study+session+participant
        lock (m_audioClipsToSavePendingSync)
        {
            foreach (var keyValuePair in m_audioClipsToSavePending)
            {
                var key = keyValuePair.Key;
                var value = keyValuePair.Value;
                var audioFileWriter = keyValuePair.Value.audioFileWriter;
                SaveClip(audioFileWriter, key, value.audioClip, value.clipLengthInSeconds);
            }
        }

    }
}
