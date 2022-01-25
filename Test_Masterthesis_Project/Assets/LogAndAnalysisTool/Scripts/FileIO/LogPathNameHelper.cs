using System.IO;

public static class LogPathNameHelper
{
    public static string GetDirectoryPathByString(string a_studyName = "", string a_sessionName = "", string a_participantName = "", string a_directoryName = "")
    {
        return Path.Combine(LogDirectoryDefines.LogDirectoryPath, a_studyName, a_sessionName, a_participantName, a_directoryName);
    }

    public static string GetDirectoryPath(StudyDefinition a_studyDefinition = null, StudySession a_studySession = null, StudyParticipant a_studyParticipant = null, string a_directoryName = "")
    {
        return Path.Combine(LogDirectoryDefines.LogDirectoryPath, a_studyDefinition?.studyName ?? "", a_studySession?.GetName() ?? "", a_studyParticipant?.GetName() ?? "", a_directoryName);
    }

    public static (string DirectoryPath, string FilePath) GetLoggingInfoPaths(StudyDefinition a_studyDefinition, StudySession a_studySession, StudyParticipant a_studyParticipant)
    {
        var fileName = LogDirectoryDefines.LoggingInfoFileName;
        var directoryPath = GetDirectoryPath(a_studyDefinition, a_studySession, a_studyParticipant, LogDirectoryDefines.CSVDirectoryName);
        var filePath = Path.Combine(directoryPath, fileName);

        return (directoryPath, filePath);
    }

    public static (string DirectoryPath, string FilePath) GetRecordingInfoPaths(StudyDefinition a_studyDefinition, StudySession a_studySession, StudyParticipant a_studyParticipant)
    {
        var fileName = LogDirectoryDefines.RecordingInfoFileName;
        var directoryPath = GetDirectoryPath(a_studyDefinition, a_studySession, a_studyParticipant, LogDirectoryDefines.AudioDirectoryName);
        var filePath = Path.Combine(directoryPath, fileName);

        return (directoryPath, filePath);
    }

    public static (string DirectoryPath, string FilePath) GetCSVPaths(LoggingDataPointType a_loggingDataObjectType, StudyDefinition a_studyDefinition, StudySession a_studySession, StudyParticipant a_studyParticipant)
    {
        var fileName = LogDirectoryDefines.GetCSVFileName(a_loggingDataObjectType);
        var directoryPath = GetDirectoryPath(a_studyDefinition, a_studySession, a_studyParticipant, LogDirectoryDefines.CSVDirectoryName);
        var filePath = Path.Combine(directoryPath, fileName);

        return (directoryPath, filePath);
    }

    public static (string DirectoryPath, string FilePath) GetNLPCalcPaths(StudyDefinition a_studyDefinition, StudySession a_studySession, StudyParticipant a_studyParticipant)
    {
        var fileName = LogDirectoryDefines.NlpCalcFileName;
        var directoryPath = GetDirectoryPath(a_studyDefinition, a_studySession, a_studyParticipant, LogDirectoryDefines.NLPCalcDirectoryName);
        var filePath = Path.Combine(directoryPath, fileName);

        return (directoryPath, filePath);
    }

    public static (string DirectoryPath, string FilePath) GetStudyDefPaths(StudyDefinition a_studyDefinition)
    {
        var fileName = LogDirectoryDefines.StudyDefFileName;
        var directoryPath = GetDirectoryPath(a_studyDefinition);
        var filePath = Path.Combine(directoryPath, fileName);

        return (directoryPath, filePath);
    }

    public static string GetAudioDirectoryPath(StudyDefinition a_studyDefinition, StudySession a_studySession, StudyParticipant a_studyParticipant)
    {
        return GetDirectoryPath(a_studyDefinition, a_studySession, a_studyParticipant, LogDirectoryDefines.AudioDirectoryName);
    }

    public static string GetAudioFilePath(int a_audioClipNumber, StudyDefinition a_studyDefinition, StudySession a_studySession, StudyParticipant a_studyParticipant)
    {
        var directoryPath = GetAudioDirectoryPath(a_studyDefinition, a_studySession, a_studyParticipant);
        var fileName = LogDirectoryDefines.GetAudioFileName(a_audioClipNumber);
        return Path.Combine(directoryPath, fileName);
    }


}
