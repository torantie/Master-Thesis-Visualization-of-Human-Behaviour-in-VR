using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class AudioFileReader : FileIOObject
{
    private readonly Dictionary<int, AudioClip> m_audioClips = new Dictionary<int, AudioClip>();

    private RecordingInfo m_recordingInfo;


    public AudioFileReader(StudyDefinition a_studyDefinition, StudySession a_studySession, StudyParticipant a_studyParticipant) : base(a_studyDefinition, a_studySession, a_studyParticipant)
    {
        SetRecordingInfo(a_studyDefinition, a_studySession, a_studyParticipant);
    }

    public AudioFileReader(StudyManager a_studyManager) : base(a_studyManager)
    { 
        SetRecordingInfo(a_studyManager.ActiveStudy, a_studyManager.CurrentStudySession, a_studyManager.LocalStudyParticipant);
    }

    public async Task<(List<AudioClip> AudioClips, float Start, float Stop)> GetAudioClipsAndPlayRange(SpatialAudioEventDataPoint a_audioEventDataPoint)
    {
        await LoadAllAudioFiles();

        if (!TryGetRecordingSession(a_audioEventDataPoint, out var recordingSession))
            return default;

        var startInSeconds = (float)(a_audioEventDataPoint.pointInTimeUtc - recordingSession.RecordingStart).TotalSeconds;
        var stopInSeconds = (float)(a_audioEventDataPoint.endPointInTimeUtc - a_audioEventDataPoint.pointInTimeUtc).TotalSeconds + startInSeconds;
        Debug.LogFormat("GetAudioClipsAndPlayRange: startInSeconds = {0}, stopInSeconds = {1}", startInSeconds, stopInSeconds);

        List<AudioClip> audioClips = new List<AudioClip>();

        foreach (var clipId in recordingSession.clipIds)
        {
            audioClips.Add(m_audioClips[clipId]);
            Debug.LogFormat("GetAudioClipsAndPlayRange: Added audio clip with clipId = {0}", clipId);
        }

        //Todo
        //float totalAudioClipsLength = 0;
        //float newStart = 0;
        //float newStop = 0;
        //foreach (var audioClip in m_audioClips)
        //{
        //    var prev = totalAudioClipsLength;
        //    totalAudioClipsLength += audioClip.length;
        //    if ((totalAudioClipsLength >= stopInSeconds && totalAudioClipsLength <= startInSeconds)
        //        || (totalAudioClipsLength > startInSeconds && prev < startInSeconds))
        //    {
        //        //Calculate new Start and Stop in Case the audio event took place after one or multiple previous audio clips
        //        if (audioClips.Count == 0)
        //        {
        //            newStart = startInSeconds - prev;
        //            newStop = stopInSeconds - prev;
        //        }


        //    }
        //}
        return (audioClips, startInSeconds, stopInSeconds);
    }

    private bool TryGetRecordingSession(SpatialAudioEventDataPoint a_audioEventDataPoint, out RecordingSession a_recordingSession)
    {
        a_recordingSession = default;
        try
        {
            foreach (var recordingSession in m_recordingInfo.recordingSessions)
            {
                if (recordingSession.sessionId == a_audioEventDataPoint.sessionId
                    && recordingSession.participantId == a_audioEventDataPoint.participantId
                    && recordingSession.taskId == a_audioEventDataPoint.taskId
                    && a_audioEventDataPoint.pointInTimeUtc >= recordingSession.RecordingStart
                    && a_audioEventDataPoint.pointInTimeUtc <= recordingSession.RecordingStop)
                {
                    a_recordingSession = recordingSession;
                    Debug.LogFormat("Found recording session ({0}) in provided recording info. {1}{2}",
                        JsonUtility.ToJson(a_recordingSession), Environment.NewLine, JsonUtility.ToJson(m_recordingInfo));
                    break;
                }
            }

            if (a_recordingSession != default)
                return true;

            Debug.LogErrorFormat("Could not find recording start in provided recording info. {0}{1}", Environment.NewLine, JsonUtility.ToJson(m_recordingInfo));
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }

        return false;
    }

    private async Task LoadAllAudioFiles()
    {

        var audioDirectoryPath = LogPathNameHelper.GetAudioDirectoryPath(StudyDefinition, StudySession, StudyParticipant);
        var audioFileCount = Directory.GetFiles(audioDirectoryPath, $"*.{LogDirectoryDefines.AudioFileType}").Length;

        if (m_audioClips.Count == audioFileCount)
        {
            Debug.LogFormat("Audio files in directory {0} already loaded.", audioDirectoryPath);
        }

        for (int i = 0; i < audioFileCount; i++)
        {
            var audioFilePath = LogPathNameHelper.GetAudioFilePath(i, StudyDefinition, StudySession, StudyParticipant);
            m_audioClips[i] = await LoadClip(audioFilePath);
        }
    }

    /// <summary>
    /// https://answers.unity.com/questions/1518536/load-audioclip-from-folder-on-computer-into-game-i.html
    /// </summary>
    /// <param name="a_path"></param>
    /// <returns></returns>
    private async Task<AudioClip> LoadClip(string a_path)
    {
        AudioClip clip = null;
        using (UnityWebRequest unityWebRequest = UnityWebRequestMultimedia.GetAudioClip(a_path, AudioType.WAV))
        {
            unityWebRequest.SendWebRequest();

            try
            {
                while (!unityWebRequest.isDone) await Task.Delay(5);

                if (unityWebRequest.result == UnityWebRequest.Result.Success)
                {
                    clip = DownloadHandlerAudioClip.GetContent(unityWebRequest);
                }
                else
                {
                    Debug.LogErrorFormat($"LoadClip: {unityWebRequest.error}, uri: {unityWebRequest.uri}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }

        return clip;
    }

    private void SetRecordingInfo(StudyDefinition a_studyDefinition, StudySession a_studySession, StudyParticipant a_studyParticipant)
    {
        TextFileReader textFileReader = new TextFileReader(a_studyDefinition, a_studySession, a_studyParticipant);
        if (!textFileReader.TryReadFromRecordingInfo(out m_recordingInfo))
        {
            Debug.LogErrorFormat("Could not read and set recording info for AudioFileReader. Study: {0}, session: {1}, participant: {2}",
                a_studyDefinition, a_studySession, a_studyParticipant);
        }
    }
}
