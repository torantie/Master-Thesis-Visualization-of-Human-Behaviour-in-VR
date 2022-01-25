using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TextFileReader : FileIOObject
{
    public TextFileReader(StudyDefinition a_studyDefinition, StudySession a_studySession, StudyParticipant a_studyParticipant) : base(a_studyDefinition, a_studySession, a_studyParticipant)
    {

    }

    public TextFileReader(StudyManager a_studyManager) : base(a_studyManager)
    {

    }

    public bool TryReadFromLoggingInfo(out ParticipantLoggingInfo a_participantLoggingInfo)
    {
        try
        {
            var (_, FilePath) = LogPathNameHelper.GetLoggingInfoPaths(StudyDefinition, StudySession, StudyParticipant);

            if (TryReadFileText(FilePath, out var fileText))
            {
                a_participantLoggingInfo = JsonUtility.FromJson<ParticipantLoggingInfo>(fileText);
                return true;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
        a_participantLoggingInfo = null;
        return false;
    }

    public bool TryReadFromRecordingInfo(out RecordingInfo a_recordingInfo)
    {
        try
        {
            var (_, FilePath) = LogPathNameHelper.GetRecordingInfoPaths(StudyDefinition, StudySession, StudyParticipant);

            if (TryReadFileText(FilePath, out var fileText))
            {
                a_recordingInfo = JsonUtility.FromJson<RecordingInfo>(fileText);
                return true;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
        a_recordingInfo = null;
        return false;
    }

    public bool TryReadLoggingDataPoints<T>(LoggingDataPointType a_loggingDataObjectType, out T[] a_loggingDataPoints) where T : LoggingDataPoint
    {
        var (_, FilePath) = LogPathNameHelper.GetCSVPaths(a_loggingDataObjectType, StudyDefinition, StudySession, StudyParticipant);
        return TryReadFromCSV(FilePath, out a_loggingDataPoints);
    }

    public bool LoggingDataPointsExist(LoggingDataPointType a_loggingDataObjectType)
    {
        var (_, FilePath) = LogPathNameHelper.GetCSVPaths(a_loggingDataObjectType, StudyDefinition, StudySession, StudyParticipant);
        return File.Exists(FilePath);
    }

    public bool TryReadFromNLPCalc(out NLPCalcResponse[] a_nlpCalcResponses)
    {
        try
        {
            var (_, FilePath) = LogPathNameHelper.GetNLPCalcPaths(StudyDefinition, StudySession, StudyParticipant);

            if (TryReadFileText(FilePath, out var fileText))
            {
                a_nlpCalcResponses = JsonArrayHelper.FromJson<NLPCalcResponse>(fileText);
                return true;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
        a_nlpCalcResponses = null;
        return false;
    }

    public static bool TryReadStudySessionsFromDirectory(StudyDefinition a_studyDefinition, out List<StudySession> a_studySessions)
    {
        a_studySessions = new List<StudySession>();

        try
        {
            var studyDirectory = LogPathNameHelper.GetDirectoryPath(a_studyDefinition, null, null);
            DirectoryInfo directoryInfo = new DirectoryInfo(studyDirectory);
            foreach (var directory in directoryInfo.GetDirectories())
            {
                if (directory.Name.Contains(StudySession.GetNamePrefix()))
                {
                    var prefixLength = StudySession.GetNamePrefix().Length;
                    var sessionId = directory.Name.Substring(prefixLength);
                    var studySession = new StudySession() { id = sessionId };
                    var sessionDirectory = LogPathNameHelper.GetDirectoryPath(a_studyDefinition, studySession, null);
                    DirectoryInfo sessionDirectoryInfo = new DirectoryInfo(sessionDirectory);

                    foreach (var participantDirectory in sessionDirectoryInfo.GetDirectories())
                    {
                        if (participantDirectory.Name.Contains(StudyParticipant.GetNamePrefix()))
                        {
                            var prefixLength2 = StudyParticipant.GetNamePrefix().Length;
                            var participantId = participantDirectory.Name.Substring(prefixLength2);
                            var studyParticipant = new StudyParticipant() { id = participantId };
                            studySession.studyParticipants.Add(studyParticipant);
                        }
                    }

                    a_studySessions.Add(studySession);
                }
            }
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex);
        }

        return false;
    }

    public static bool TryReadActiveStudyDefinition(out StudyDefinition a_studyDefinition)
    {
        try
        {
            var studyDefFileName = LogDirectoryDefines.StudyDefFileName;
            var logDirectoryPath = LogDirectoryDefines.LogDirectoryPath;

            DirectoryInfo directoryInfo = new DirectoryInfo(logDirectoryPath);
            foreach (var directory in directoryInfo.GetDirectories())
            {
                foreach (var file in directory.GetFiles())
                {
                    if (file.Name == studyDefFileName)
                    {
                        if (TryReadFileText(file.FullName, out var fileText))
                        {
                            a_studyDefinition = JsonUtility.FromJson<StudyDefinition>(fileText);
                            if (a_studyDefinition.isActive)
                            {
                                return true;
                            }
                        }
                    }
                }
                Debug.Log(string.Format("TryReadActiveStudyDefinition: Search directory {0} for active study definition.", directory.FullName));
            }

            Debug.Log("TryReadActiveStudyDefinition: Could not find active study definition.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex);
        }
        a_studyDefinition = null;
        return false;
    }

    public static bool TryReadAllStudyDefinitions(out List<StudyDefinition> a_studyDefinitions)
    {
        a_studyDefinitions = new List<StudyDefinition>();

        try
        {
            var studyDefFileName = LogDirectoryDefines.StudyDefFileName;
            var logDirectoryPath = LogPathNameHelper.GetDirectoryPath();

            DirectoryInfo directoryInfo = new DirectoryInfo(logDirectoryPath);
            foreach (var directory in directoryInfo.GetDirectories())
            {
                foreach (var file in directory.GetFiles())
                {
                    if (file.Name == studyDefFileName)
                    {
                        if (TryReadFileText(file.FullName, out var fileText))
                        {
                            a_studyDefinitions.Add(JsonUtility.FromJson<StudyDefinition>(fileText));
                        }
                        else
                        {
                            Debug.LogWarning(string.Format("TryReadAllStudyDefinitions: Could not read study definition in directory {0}", directory.FullName));
                        }
                    }
                }
            }

            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex);
        }

        return false;
    }

    public static bool TryReadStudyDefinitionWithId(string a_studyDefinitionId, out StudyDefinition a_studyDefinition)
    {
        a_studyDefinition = default;

        try
        {
            if (a_studyDefinitionId.Equals(default))
                return false;

            if (TryReadAllStudyDefinitions(out var studyDefinitions))
            {
                foreach (var studyDefinition in studyDefinitions)
                {
                    if (studyDefinition.id.Equals(a_studyDefinitionId))
                    {
                        a_studyDefinition = studyDefinition;
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

    public static bool StudyDefinitionExists(StudyDefinition a_studyDefinition)
    {
        var (_, FilePath) = LogPathNameHelper.GetStudyDefPaths(a_studyDefinition);
        return File.Exists(FilePath);
    }

    private static bool TryReadFromCSV<T>(string a_filePath, out T[] a_array)
    {
        try
        {
            if (TryReadFileText(a_filePath, out var fileText))
            {
                a_array = CSVSerializer.Deserialize<T>(CSVSerializer.ParseCSV(fileText, ';'));
                return true;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex);
        }

        a_array = null;
        return false;
    }

    private static bool TryReadFileText(string a_filePath, out string a_fileText)
    {
        a_fileText = "";

        if (!File.Exists(a_filePath))
        {
            Debug.LogWarning(string.Format("Could not read files text. File at {0} does not exist.", a_filePath));
            return false;
        }

        using var fileStream = File.Open(a_filePath, FileMode.Open, FileAccess.Read);
        var read = new StreamReader(fileStream);
        a_fileText = read.ReadToEnd();

        return true;
    }

}
