using System;

public class OfflineAudioEventDataPoint : LoggingDataPoint
{
    public override LoggingDataPointType Type => LoggingDataPointType.OfflineAudioEvent;

    public long audioPositionTicks;

    public long audioDurationTicks;

    public string audioFileName;

    public DateTime endPointInTimeUtc;

    public string spokenText;

    public float confidenceLevel;

    public OfflineAudioEventDataPoint() : base()
    {
    }

    public OfflineSpatialAudioEventDataPoint ParseToSpatialOfflineAudioEvent(string a_position)
    {
        return new OfflineSpatialAudioEventDataPoint
        {
            position = a_position,
            dataPointGuid = dataPointGuid,
            confidenceLevel = confidenceLevel,
            participantId = participantId,
            sessionId = sessionId,
            taskId = taskId,
            spokenText = spokenText,
            pointInTimeUtc = pointInTimeUtc,
            endPointInTimeUtc = endPointInTimeUtc,


        };
    }
}

