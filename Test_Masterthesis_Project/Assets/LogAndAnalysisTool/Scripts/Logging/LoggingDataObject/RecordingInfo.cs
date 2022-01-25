using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RecordingInfo
{
    public int amountOfClips;

    public List<RecordingSession> recordingSessions = new List<RecordingSession>();
}

[Serializable]
public class RecordingSession
{
    public long recordingStart;

    public long recordingStop;

    public int recordingFrequency;

    public List<int> clipIds = new List<int>();

    public string taskId;

    public string participantId;

    public string sessionId;

    public DateTime RecordingStart => new DateTime(recordingStart, DateTimeKind.Utc);

    public DateTime RecordingStop => new DateTime(recordingStop, DateTimeKind.Utc);

    public double RecordingLengthInSeconds => (RecordingStop - RecordingStart).TotalSeconds;
}