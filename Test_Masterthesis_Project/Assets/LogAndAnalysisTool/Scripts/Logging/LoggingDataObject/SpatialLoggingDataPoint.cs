using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpatialLoggingDataPoint : LoggingDataPoint
{
    public string position;

    public SpatialLoggingDataPoint() : base()
    {
    }

    public SpatialLoggingDataPoint(StudyManager a_studyManager, Guid a_dataPointGuid, DateTime a_pointInTime, Vector3 a_position) : base(a_studyManager, a_dataPointGuid, a_pointInTime)
    {
        position = a_position.ToString();
    }
}
