using System;

[Serializable]
public class LoggingDataPoint
{
    public virtual LoggingDataPointType Type { get; }

    public Guid dataPointGuid = Guid.NewGuid();

    public string taskId;

    public string participantId;

    public string sessionId;

    public DateTime pointInTimeUtc = DateTime.UtcNow;

    public LoggingDataPoint()
    {
    }

    public LoggingDataPoint(StudyManager a_studyManager, Guid a_dataPointGuid, DateTime a_pointInTime)
    {
        taskId = a_studyManager.TaskId;
        participantId = a_studyManager.LocalStudyParticipant.id;
        sessionId = a_studyManager.CurrentStudySession.id;
        dataPointGuid = a_dataPointGuid;
        pointInTimeUtc = a_pointInTime;
    }
}
