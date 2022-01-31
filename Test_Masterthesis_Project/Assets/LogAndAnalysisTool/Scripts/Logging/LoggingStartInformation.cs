using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LoggingStartInformation
{
    public string sessionId;

    public string participantId;

    public string taskId;

    public LoggingStartInformation(string a_sessionId, string a_participantId, string a_taskId)
    {
        sessionId = a_sessionId;
        participantId = a_participantId;
        taskId = a_taskId;
    }
}



