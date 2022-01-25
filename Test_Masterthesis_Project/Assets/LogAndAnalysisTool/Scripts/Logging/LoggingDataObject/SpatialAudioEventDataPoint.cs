using System;
using UnityEngine;

[Serializable]
public class SpatialAudioEventDataPoint : SpatialLoggingDataPoint
{
    public override LoggingDataPointType Type => LoggingDataPointType.SpatialAudioEvent;

    public DateTime endPointInTimeUtc;

    public string spokenText;

    public float confidenceLevel;

    public Sentiment averageSentiment;

    public string endPosition;

    public SpatialAudioEventDataPoint() : base()
    {
    }

    public SpatialAudioEventDataPoint(StudyManager a_studyManager, Guid a_dataPointGuid, DateTime a_pointInTime, Vector3 position) : base(a_studyManager, a_dataPointGuid, a_pointInTime, position)
    {
    }

}
