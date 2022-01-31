using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CustomSpatialEventLogger : EventLogger
{
    [SerializeField]
    private LoggingManager m_loggingManager;

    private readonly UnityEvent<CustomSpatialEventArgs> m_customEvent = new UnityEvent<CustomSpatialEventArgs>();

    public override void StartListening()
    {
        m_customEvent.AddListener(OnCustomEventReceived);
    }

    public override void StopListening()
    {
        m_customEvent.RemoveAllListeners();
    }

    public void InvokeCustomSpatialEvent(CustomSpatialEventArgs a_customEventArgs)
    {
        m_customEvent.Invoke(a_customEventArgs);
    }

    private void OnCustomEventReceived(CustomSpatialEventArgs a_customEventArgs)
    {

        var customEventDataPoint = new CustomSpatialEventDataPoint(
            m_loggingManager.StudyManager,
            Guid.NewGuid(),
            DateTime.UtcNow,
            a_customEventArgs.Position)
        {
            eventName = a_customEventArgs.EventName,
            message = a_customEventArgs.Message,
            value = a_customEventArgs.Value,
            valueType = a_customEventArgs.ValueType.ToString(),
        };

        m_loggingManager.OnEventTriggered(customEventDataPoint);
    }

}

public class CustomSpatialEventArgs
{
    public CustomSpatialEventArgs(string a_eventName, string a_message, string a_value, Type a_valueType, Vector3 a_position)
    {
        EventName = a_eventName;
        Message = a_message;
        Value = a_value;
        ValueType = a_valueType;
        Position = a_position;
    }

    public string EventName { get; set; }

    public string Message { get; set; }

    public string Value { get; set; }

    public Type ValueType { get; set; }

    public Vector3 Position { get; set; }
}
