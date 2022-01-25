using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioFileWriter : FileIOObject
{
    public AudioFileWriter(StudyDefinition a_studyDefinition, StudySession a_studySession, StudyParticipant a_studyParticipant) : base(a_studyDefinition, a_studySession, a_studyParticipant)
    {

    }

    public AudioFileWriter(StudyManager a_studyManager) : base(a_studyManager)
    {

    }

    public void WriteAudioFiles(List<AudioClip> a_audioClips, int a_offset)
    {
        for (int i = 0; i < a_audioClips.Count; i++)
        {
            WriteAudioFile(a_audioClips[i], i + a_offset);
        }
    }

    public void WriteAudioFile(AudioClip a_audioClip, int a_audioClipNumber)
    {
        try
        {
            SavWav.Save(LogPathNameHelper.GetAudioFilePath(a_audioClipNumber, StudyDefinition, StudySession, StudyParticipant), a_audioClip);
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    public void WriteAudioFile(int frequency, int channels, int samples, float[] sampleData, int a_audioClipNumber)
    {
        try
        {
            SavWav.Save(LogPathNameHelper.GetAudioFilePath(a_audioClipNumber, StudyDefinition, StudySession, StudyParticipant), frequency, channels, samples, sampleData);
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    public static void WriteAudioFile(string a_filePath, int frequency, int channels, int samples, float[] sampleData)
    {
        try
        {
            SavWav.Save(a_filePath, frequency, channels, samples, sampleData);
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    public static float[] GetSampleData(AudioClip a_audioClip)
    {
        var samples = new float[a_audioClip.samples];

        a_audioClip.GetData(samples, 0);

        return samples;
    }

}
