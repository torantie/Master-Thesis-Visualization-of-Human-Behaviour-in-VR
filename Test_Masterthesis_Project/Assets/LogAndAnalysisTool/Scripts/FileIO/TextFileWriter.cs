using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class TextFileWriter : FileIOObject
{

    public TextFileWriter(StudyDefinition a_studyDefinition, StudySession a_studySession, StudyParticipant a_studyParticipant) : base(a_studyDefinition, a_studySession, a_studyParticipant)
    {

    }

    public TextFileWriter(StudyManager a_studyManager) : base(a_studyManager)
    {

    }

    public void WriteToLoggingInfo(ParticipantLoggingInfo a_participantLoggingInfo, bool a_prettyPrint, bool a_overwrite)
    {
        var paths = LogPathNameHelper.GetLoggingInfoPaths(StudyDefinition, StudySession, StudyParticipant);
        WriteToJSONFile(paths, a_participantLoggingInfo, a_prettyPrint, a_overwrite);
    }

    public void WriteToRecordingInfo(RecordingInfo a_recordingInfo, bool a_prettyPrint, bool a_overwrite)
    {
        var paths = LogPathNameHelper.GetRecordingInfoPaths(StudyDefinition, StudySession, StudyParticipant);
        WriteToJSONFile(paths, a_recordingInfo, a_prettyPrint, a_overwrite);
    }

    public void WriteToNLPCalc(NLPCalcResponse[] a_nlpCalcResponses, bool a_prettyPrint = false)
    {

        var paths = LogPathNameHelper.GetNLPCalcPaths(StudyDefinition, StudySession, StudyParticipant);
        WriteToJSONFile(paths, a_nlpCalcResponses, a_prettyPrint);
    }

    public void WriteToCSV(IEnumerable<LoggingDataPoint> a_loggingDataObjects, LoggingDataPointType a_loggingDataObjectType, bool a_overwrite = false)
    {
        var paths = LogPathNameHelper.GetCSVPaths(a_loggingDataObjectType, StudyDefinition, StudySession, StudyParticipant);
        var exists = File.Exists(paths.FilePath);
        var csvString = "";
        foreach (var loggingDataObject in a_loggingDataObjects)
        {
            if (csvString == "" && (!exists || a_overwrite))
            {
                var namesAndValues = SerializationHelper.FieldNamesToCsvString(loggingDataObject) + Environment.NewLine + SerializationHelper.FieldValuesToCsvString(loggingDataObject);
                csvString = namesAndValues;
            }
            else
            {
                if (csvString == "")
                    csvString = SerializationHelper.FieldValuesToCsvString(loggingDataObject);
                else
                    csvString = string.Join(Environment.NewLine, csvString, SerializationHelper.FieldValuesToCsvString(loggingDataObject));
            }
        }
        WriteToFile(paths, csvString, a_overwrite);
    }

    public void WriteToCSV(LoggingDataPoint a_loggingDataObject, bool a_overwrite = false)
    {
        var paths = LogPathNameHelper.GetCSVPaths(a_loggingDataObject.Type, StudyDefinition, StudySession, StudyParticipant);
        WriteToCSVFile(paths, a_loggingDataObject, a_overwrite);
    }

    public static bool TryEditStudyDefinition(StudyDefinition a_oldStudyDefinition, StudyDefinition a_newStudyDefinition)
    {
        var oldDirectoryPath = LogPathNameHelper.GetDirectoryPath(a_oldStudyDefinition);
        var newDirectoryPath = LogPathNameHelper.GetDirectoryPath(a_newStudyDefinition);

        if (!oldDirectoryPath.Equals(newDirectoryPath) && Directory.Exists(newDirectoryPath))
        {
            return false;
        }
        else if (!oldDirectoryPath.Equals(newDirectoryPath))
        {
            try
            {
                Directory.Move(oldDirectoryPath, newDirectoryPath);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return false;
            }
        }

        WriteToStudyDefinition(a_newStudyDefinition, true, true);

        return true;
    }

    public static void WriteToStudyDefinition(StudyDefinition a_studyDefinition, bool a_prettyPrint, bool a_overwrite)
    {
        var paths = LogPathNameHelper.GetStudyDefPaths(a_studyDefinition);
        WriteToJSONFile(paths, a_studyDefinition, a_prettyPrint, a_overwrite);
    }

    private static void WriteToJSONFile<T>((string DirectoryPath, string FilePath) a_paths, T a_serializableObject, bool a_prettyPrint = false, bool a_overwrite = false)
    {
        try
        {
            string csvString = JsonUtility.ToJson(a_serializableObject, a_prettyPrint);

            WriteToFile(a_paths, csvString, a_overwrite);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    private static void WriteToJSONFile<T>((string DirectoryPath, string FilePath) a_paths, T[] a_serializableObject, bool a_prettyPrint = false)
    {
        try
        {
            string csvString = JsonArrayHelper.ToJson(a_serializableObject, a_prettyPrint);

            WriteToFile(a_paths, csvString);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    private static void WriteToCSVFile<T>((string DirectoryPath, string FilePath) a_paths, T a_serializableObject, bool a_overwrite = false)
    {
        try
        {
            string csvString = GetCSVStringToWrite(a_paths, a_serializableObject, a_overwrite);

            WriteToFile(a_paths, csvString, a_overwrite);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    private static string GetCSVStringToWrite<T>((string DirectoryPath, string FilePath) a_paths, T a_serializableObject, bool a_overwrite = false)
    {
        string csvString;
        if (File.Exists(a_paths.FilePath) && !a_overwrite)
        {
            csvString = SerializationHelper.FieldValuesToCsvString(a_serializableObject);
        }
        else
        {
            csvString = SerializationHelper.FieldNamesToCsvString(a_serializableObject) + Environment.NewLine + SerializationHelper.FieldValuesToCsvString(a_serializableObject);
        }

        return csvString;
    }

    private static void WriteToFile((string DirectoryPath, string FilePath) a_paths, string a_stringToWrite, bool a_overwrite = false)
    {
        if (!Directory.Exists(a_paths.DirectoryPath))
            Directory.CreateDirectory(a_paths.DirectoryPath);

        if (a_overwrite)
        {
            using var streamWriter = File.CreateText(a_paths.FilePath);
            streamWriter.WriteLine(a_stringToWrite);
        }
        else
        {
            using var streamWriter = File.AppendText(a_paths.FilePath);
            streamWriter.WriteLine(a_stringToWrite);
        }

#if UNITY_EDITOR
        Debug.LogFormat("Wrote file with text: {0} to: {1}", a_stringToWrite, a_paths.FilePath);
#endif
    }


}
