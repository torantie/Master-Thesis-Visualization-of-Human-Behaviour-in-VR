using System;
using UnityEngine;

[Serializable]
public class CustomSpatialEventDataPoint : SpatialLoggingDataPoint
{
    public string eventName;

    public string message;

    public string value;

    public string valueType;

    public CustomSpatialEventDataPoint() : base()
    {
    }

    public CustomSpatialEventDataPoint(StudyManager a_studyManager, Guid a_dataPointGuid, DateTime a_pointInTime, Vector3 position) : base(a_studyManager, a_dataPointGuid, a_pointInTime, position)
    {
    }

    public override LoggingDataPointType Type => LoggingDataPointType.CustomSpatialEvent;
}
