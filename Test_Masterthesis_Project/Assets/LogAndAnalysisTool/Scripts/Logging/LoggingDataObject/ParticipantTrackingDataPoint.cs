using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ParticipantTrackingDataPoint : SpatialLoggingDataPoint
{
    public override LoggingDataPointType Type => LoggingDataPointType.ParticipantTrackingData;

    public string hmdPosition;

    public string hmdRotation;

    public string gazePosition;

    public string gazeRotation;

    public string focusedObjectName;

    public string leftControllerPosition;

    public string leftControllerRotation;

    public string rightControllerPosition;

    public string rightControllerRotation;

    public string hmdTransformRotation;

    public string leftControllerTransformPosition;

    public string leftControllerTransformRotation;

    public string rightControllerTransformPosition;

    public string rightControllerTransformRotation;


    public ParticipantTrackingDataPoint() : base()
    {
    }

    public ParticipantTrackingDataPoint(StudyManager a_studyManager, Guid a_dataPointGuid, DateTime a_pointInTime, Vector3 position) : base(a_studyManager, a_dataPointGuid, a_pointInTime, position)
    {
    }

}

