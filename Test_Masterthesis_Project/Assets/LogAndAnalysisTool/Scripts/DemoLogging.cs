using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoLogging : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var sessionId = "1";
        var participantId = "1";
        var taskId = "Running Task 1";
        var startInformation = new LoggingStartInformation(sessionId, participantId, taskId);
        Logger.StartLogging(startInformation);

        var eventName = "EventName";
        var message = "Message";
        var value = 1f;
        var valueType = value.GetType();
        var position = transform.position;
        var customEventArgs = new CustomSpatialEventArgs(eventName, message, value.ToString(), valueType, position);
        Logger.InvokeCustomSpatialEvent(customEventArgs);
        Logger.StopLogging();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
