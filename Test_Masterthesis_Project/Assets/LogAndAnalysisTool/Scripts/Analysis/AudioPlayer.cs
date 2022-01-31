using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    /// <summary>
    /// Audio source playing sound.
    /// </summary>
    private AudioSource m_audioSource;

    /// <summary>
    /// Reads audio to play.
    /// </summary>
    private AudioFileReader m_audioFileReader;

    /// <summary>
    /// Spatial audio event to know location in audio clips to play audio.
    /// </summary>
    public SpatialAudioEventDataPoint AudioEventDataPoint { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        SetAudioFileReader();
        if (m_audioSource == null)
        {
            m_audioSource = GetComponent<AudioSource>();
        }
        else
        {
            Debug.LogError("No AudioSource attached.");
        }
    }

    /// <summary>
    /// Sets AudioFileReader with information from the ActiveStudy and the attached VisualDataPoint.
    /// </summary>
    private void SetAudioFileReader()
    {
        if (TryGetComponent<VisualDataPoint>(out var visualDataPoint))
        {
            var loggingDataPoint = visualDataPoint.LoggingDataPoint;
            var activeStudy = AnalysisManager.Instance.StudyManager.ActiveStudy;
            if (!TryCreateAudioFileReader(loggingDataPoint, activeStudy, out m_audioFileReader))
            {
                Debug.LogErrorFormat("Could not get audio file reader for study: {0}, session: {1}, participant: {2}",
                    activeStudy.studyName, loggingDataPoint.sessionId, loggingDataPoint.participantId);
            }
        }
        else
        {
            Debug.LogError("No VisualDataPoint attached.");
        }
    }

    /// <summary>
    /// Tries to create an AudioFileReader with the provided LoggingDataPoint and StudyDefinition.
    /// </summary>
    /// <param name="a_loggingDataPoint">Datapoint holding information for the AudioFileReader.</param>
    /// <param name="a_activeStudy">Current active study.</param>
    /// <param name="a_audioFileReader">Created AudioFileReader.</param>
    /// <returns>True if reader was successfully created.</returns>
    private bool TryCreateAudioFileReader(LoggingDataPoint a_loggingDataPoint, StudyDefinition a_activeStudy, out AudioFileReader a_audioFileReader)
    {
        a_audioFileReader = default;
        try
        {
            foreach (var studySession in a_activeStudy.studySessions)
            {
                foreach (var studyParticipant in studySession.studyParticipants)
                {
                    if (studySession.id.Equals(a_loggingDataPoint.sessionId)
                        && studyParticipant.id.Equals(a_loggingDataPoint.participantId))
                    {
                        a_audioFileReader = new AudioFileReader(a_activeStudy, studySession, studyParticipant);
                        return true;
                    }
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex);
        }

        return false;
    }

    public void Stop()
    {
        m_audioSource.Stop();
    }

    public async Task Play()
    {
        if (AudioEventDataPoint == default)
        {
            Debug.Log("No AudioEventDataPoint set to play. Trying to use attached VisualDataPoint information.");

            if (TryGetAudioEventDataPoint(out var audioEventDataPoint))
            {
                AudioEventDataPoint = audioEventDataPoint;
            }
        }

        var clipsAndRange = await m_audioFileReader.GetAudioClipsAndPlayRange(AudioEventDataPoint);
        StartCoroutine(PlayAudioClips(clipsAndRange.Start, clipsAndRange.Stop, clipsAndRange.AudioClips));
    }

    public IEnumerator PlayAudioClips(float a_firstClipStartTime, float a_lastClipEndTime, List<AudioClip> a_audioClips)
    {
        if (m_audioSource != null)
        {
            float totalLength = 0;
            foreach (var clip in a_audioClips)
            {
                totalLength += clip.length;
                Debug.LogFormat("clip length {0}", clip.length);
            }
            Debug.LogFormat("Total length {0}", totalLength);

            for (int i = 0; i < a_audioClips.Count; i++)
            {
                AudioClip audioClip = a_audioClips[i];
                m_audioSource.clip = audioClip;
                m_audioSource.time = 0;
                var isLastClip = i == a_audioClips.Count - 1;
                var isFirstClip = i == 0;
                var endTime = audioClip.length;
                if (isFirstClip)
                {
                    Debug.LogFormat("Playing first clip. Starts at: {0}", a_firstClipStartTime);
                    m_audioSource.time = a_firstClipStartTime;
                }

                m_audioSource.Play();

                if (isLastClip)
                {
                    endTime = a_lastClipEndTime;
                    Debug.LogFormat("Playing last clip. Scheduled end at: {0}", endTime);
                }

                var waitTime = endTime - a_firstClipStartTime;

                Debug.LogFormat("Playback Position {0} waitTime:{1}", m_audioSource.time, waitTime);
                yield return new WaitForSecondsRealtime(waitTime);
                Debug.LogFormat("Playback Position {0} waitTime:{1}", m_audioSource.time, waitTime);
                m_audioSource.Stop();
            }
        }
        else
        {
            Debug.LogError("No AudioSource attached. Cannot play audio.");
        }
    }


    private bool TryGetAudioEventDataPoint(out SpatialAudioEventDataPoint a_audioEventDataPoint)
    {
        a_audioEventDataPoint = default;
        Debug.Log("No AudioEventDataPoint set to play. Trying to use attached VisualDataPoint information.");
        if (TryGetComponent<VisualDataPoint>(out var visualDataPoint))
        {
            if (visualDataPoint.LoggingDataPoint is SpatialAudioEventDataPoint audioEventDataPoint)
            {
                a_audioEventDataPoint = audioEventDataPoint;
                return true;
            }
            else
            {
                Debug.LogWarning("No AudioEventDataPoint found in VisualDataPoint.");
                return false;
            }
        }
        else
        {
            Debug.LogWarning("No VisualDataPoint attached.");
            return false;
        }
    }
}
