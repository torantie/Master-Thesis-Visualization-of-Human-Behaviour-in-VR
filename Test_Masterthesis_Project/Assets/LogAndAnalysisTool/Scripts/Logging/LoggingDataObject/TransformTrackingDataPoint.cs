using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformTrackingDataPoint : SpatialLoggingDataPoint
{
    public override LoggingDataPointType Type => LoggingDataPointType.TransformTrackingData;

    public string transformId;

    public string rotation;

    public string scale;

    public TransformTrackingDataPoint() : base()
    {
    }

    public TransformTrackingDataPoint(StudyManager a_studyManager, Guid a_dataPointGuid, DateTime a_pointInTime, Vector3 position) : base(a_studyManager, a_dataPointGuid, a_pointInTime, position)
    {
    }

}
