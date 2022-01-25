using System.IO;
using UnityEngine;

public class LogDirectoryDefines
{
    public const string LogDirectoryName = "VRAnalysisTool_LogData";

    public const string NLPCalcDirectoryName = "NLPCalc";

    public const string CSVDirectoryName = "CSV";

    public const string AudioDirectoryName = "Audio";

    public const string AudioFileType = "wav";

    public const string NlpCalcFileName = "NLPCalc.json";

    public const string StudyDefFileName = "StudyDefinition.json";

    public const string RecordingInfoFileName = "RecordingInfo.json";

    public const string LoggingInfoFileName = "ParticipantLoggingInfo.json";

    public static string GetAudioFileName(int a_audioClipNumber) => string.Format("audio_{0}.{1}", a_audioClipNumber, AudioFileType);

    public static string LogDirectoryPath = Path.Combine(Application.persistentDataPath, LogDirectoryName);

    public static string GetCSVFileName(LoggingDataPointType a_loggingDataObjectType)
    {
        switch (a_loggingDataObjectType)
        {
            case LoggingDataPointType.ParticipantTrackingData:
                return "ParticipantTrackingData.csv";
            case LoggingDataPointType.CustomSpatialEvent:
                return "CustomSpatialEventData.csv";
            case LoggingDataPointType.InputActionEvent:
                return "InputActionEventData.csv";
            case LoggingDataPointType.TransformTrackingData:
                return "TransformData.csv";
            case LoggingDataPointType.SpatialAudioEvent:
                return "SpatialAudioData.csv";
            case LoggingDataPointType.OfflineAudioEvent:
                return "OfflineAudioData.csv";
            case LoggingDataPointType.OfflineSpatialAudioEvent:
                return "OfflineSpatialAudioData.csv";
            case LoggingDataPointType.SentimentEvent:
                return "SentimentEventData.csv";
            default:
                break;
        }

        return "";
    }
    /*
    public static string CSVDirectory = Path.Combine(LogDirectory, CSVDirectoryName);

    public static string AudioDirectory = Path.Combine(LogDirectory, AudioDirectoryName);

    public static string NLPCalcDirectory = Path.Combine(LogDirectory, NLPCalcDirectoryName);
   */


}
