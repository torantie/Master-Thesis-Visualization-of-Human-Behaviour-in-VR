using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Logger
{
    public static void StartLogging(LoggingStartInformation a_loggingStartInformation)
    {
        LoggingManager.Instance.StartLogging(a_loggingStartInformation);
    }

    public static void StopLogging()
    {
        LoggingManager.Instance.StopLogging();
    }

    public static void InvokeCustomSpatialEvent(CustomSpatialEventArgs a_customEventArgs)
    {
        LoggingManager.Instance.InvokeCustomSpatialEvent(a_customEventArgs);
    }
}
