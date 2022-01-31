using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputActionEventDataPoint : SpatialLoggingDataPoint
{
    public override LoggingDataPointType Type => LoggingDataPointType.InputActionEvent;

    public string inputActionName;

    public string inputActionControlPath;

    public double duration;

    public double startTime;

    public double time;

    public string value;

    public string valueType;

    public InputActionPhase phase;

    public InputActionEventDataPoint() : base()
    {
    }

    public InputActionEventDataPoint(StudyManager a_studyManager, Guid a_dataPointGuid, DateTime a_pointInTime, Vector3 position) : base(a_studyManager, a_dataPointGuid, a_pointInTime, position)
    {
    }

}
