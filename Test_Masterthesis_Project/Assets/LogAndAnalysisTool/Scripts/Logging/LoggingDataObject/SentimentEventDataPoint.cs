using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentimentEventDataPoint : SpatialLoggingDataPoint
{
    public override LoggingDataPointType Type => LoggingDataPointType.SentimentEvent;

    public string sentence;

    public Sentiment sentiment;

    public SentimentEventDataPoint() : base()
    {
    }

    public SentimentEventDataPoint(SpatialAudioEventDataPoint a_spatialAudioEventDataPoint) : base()
    {
        dataPointGuid = a_spatialAudioEventDataPoint.dataPointGuid;
        taskId = a_spatialAudioEventDataPoint.taskId;
        sessionId = a_spatialAudioEventDataPoint.sessionId;
        participantId = a_spatialAudioEventDataPoint.participantId;
        position = a_spatialAudioEventDataPoint.position;
        pointInTimeUtc = a_spatialAudioEventDataPoint.pointInTimeUtc;
    }

    public SentimentEventDataPoint(StudyManager a_studyManager, Guid a_dataPointGuid, DateTime a_pointInTime, Vector3 position) : base(a_studyManager, a_dataPointGuid, a_pointInTime, position)
    {
    }

}
