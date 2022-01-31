using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class LoggingManager : MonoBehaviour
{
    [SerializeField]
    private StudyManager m_studyManager;

    [SerializeField]
    private List<ParticipantLogger> m_participantLoggers;

    [SerializeField]
    private List<EventLogger> m_eventLoggers;

    [SerializeField]
    private TransformLogger m_transformLogger;

    [SerializeField]
    private AudioRecorder m_audioRecorder;

    [SerializeField]
    private int m_logPerSecond;

    [SerializeField, ReadOnly]
    private int m_loggingIntervallInMs;

    [SerializeField, ReadOnly]
    private int m_loggingIntervallInMsCorrection = 1;

    [Header("Debug options")]

    [SerializeField]
    private bool m_debugMode;

    [SerializeField]
    private int m_debugLoggingTimeInSeconds;

    [SerializeField]
    private LoggingStartInformation m_debugLoggingStartInformation;

    [SerializeField, ReadOnly]
    private float m_updateUpdateCountPerSecond;

    [SerializeField, ReadOnly]
    private float m_logCountPerSecond;

    private float m_updateCount = 0;

    private float m_logCount = 0;

    private bool m_cycleRunning;


    private CustomSpatialEventLogger m_customSpatialEventLogger;

    private readonly List<LoggingDataPoint> m_eventDataPoints = new List<LoggingDataPoint>();

    private readonly List<ParticipantTrackingDataPoint> m_continuousDataPoints = new List<ParticipantTrackingDataPoint>();

    private TextFileWriter m_textFileWriter;

    private bool m_isLogging = false;

    private int m_elapsed;


    public StudyManager StudyManager { get => m_studyManager; private set => m_studyManager = value; }

    public bool IsLogging { get => m_isLogging; private set => m_isLogging = value; }

    private static LoggingManager m_instance;

    public static LoggingManager Instance { get { return m_instance; } }


    private void Awake()
    {
        if (m_instance != null && m_instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            m_instance = this;
        }

        if (m_debugMode)
            StartCoroutine(Loop());

        foreach (var eventLogger in m_eventLoggers)
        {
            if (eventLogger is CustomSpatialEventLogger customEventLogger)
            {
                m_customSpatialEventLogger = customEventLogger;
                break;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_loggingIntervallInMs = (1000 / m_logPerSecond) - m_loggingIntervallInMsCorrection;
    }


    IEnumerator Loop()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            m_updateUpdateCountPerSecond = m_updateCount;
            m_logCountPerSecond = m_logCount;
            m_updateCount = 0;
            m_logCount = 0;
        }
    }

    private void Update()
    {
        if (m_debugMode && StudyManager.ActiveStudy != null)
        {
            if (!m_cycleRunning)
            {
                m_cycleRunning = true;

                StartCoroutine(StartAndStopLogging());
            }
        }
    }


    void LateUpdate()
    {
        if (m_debugMode)
            m_updateCount++;

        if (IsLogging)
        {
            var currentMsSinceStart = (int)(Time.realtimeSinceStartup * 1000);

            if (currentMsSinceStart > m_elapsed)
            {
                m_elapsed = currentMsSinceStart + m_loggingIntervallInMs;
                LogContinuousData();

                if (m_debugMode)
                    m_logCount++;
            }
        }
    }

    public void StartLogging(LoggingStartInformation a_loggingStartInformation)
    {
        try
        {
            if (!IsLogging)
            {
                StudyManager.SetStudySessionInformation(a_loggingStartInformation);

                m_textFileWriter = new TextFileWriter(StudyManager);
                UpdateParticipantLoggingInformation(a_loggingStartInformation);

                m_audioRecorder?.StartRecording();
                m_eventLoggers?.ForEach(eventLogger =>
                {
                    try
                    {
                        eventLogger?.StartListening();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex);
                    }
                });
                IsLogging = true;
            }
            else
            {
                Debug.Log("Logging already started.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    public void StopLogging()
    {
        try
        {
            if (IsLogging)
            {
                Debug.LogFormat("Before stop event loggers: {0}", DateTime.UtcNow);
                m_eventLoggers.ForEach(eventLogger => eventLogger?.StopListening());
                Debug.LogFormat("After stop event loggers: {0}", DateTime.UtcNow);
                m_audioRecorder?.SaveAndStopRecording(true);
                IsLogging = false;
            }
            else
            {
                Debug.Log("Logging already stopped.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    public void InvokeCustomSpatialEvent(CustomSpatialEventArgs a_customEventArgs)
    {
        if (m_customSpatialEventLogger != null)
        {
            m_customSpatialEventLogger.InvokeCustomSpatialEvent(a_customEventArgs);
        }
        else
        {
            Debug.LogWarning("Could not log custom spatial event. Custom spatial event logger not initialized.");
        }
    }

    public void OnEventTriggered<T>(T a_event) where T : LoggingDataPoint
    {
        if (IsLogging)
        {
            m_textFileWriter.WriteToCSV(a_event);
            m_eventDataPoints.Add(a_event);
        }
        else
        {
            Debug.Log("Can not log events. Logging not started.");
        }
    }

    private void UpdateParticipantLoggingInformation(LoggingStartInformation a_loggingStartInformation)
    {
        var textFileReader = new TextFileReader(StudyManager);
        if (textFileReader.TryReadFromLoggingInfo(out var participantLoggingInfo) && !participantLoggingInfo.taskIds.Contains(a_loggingStartInformation.taskId))
        {
            participantLoggingInfo.taskIds.Add(a_loggingStartInformation.taskId);
        }
        else
        {
            participantLoggingInfo = new ParticipantLoggingInfo()
            {
                logPerSecond = m_logPerSecond,
                taskIds = new List<string>() { a_loggingStartInformation.taskId }
            };
        }
        m_textFileWriter.WriteToLoggingInfo(participantLoggingInfo, true, true);
    }

    private void LogContinuousData()
    {
        var now = DateTime.UtcNow;
        var guid = Guid.NewGuid();

        ParticipantTrackingDataPoint continuousDataPoint = new ParticipantTrackingDataPoint(
            StudyManager,
            guid,
            now,
            Vector3.zero);

        m_participantLoggers?.ForEach(logger =>
        {
            try
            {
                logger?.FillDataPoint(continuousDataPoint);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }
        );

        m_textFileWriter.WriteToCSV(continuousDataPoint);

        m_continuousDataPoints.Add(continuousDataPoint);

        List<TransformTrackingDataPoint> transformDataPoints = m_transformLogger?.GetDataPoints(StudyManager, guid, now);

        m_textFileWriter.WriteToCSV(transformDataPoints, LoggingDataPointType.TransformTrackingData);
    }

    private IEnumerator StartAndStopLogging()
    {
        yield return new WaitForSeconds(2);
        StartLogging(m_debugLoggingStartInformation);
        yield return new WaitForSeconds(m_debugLoggingTimeInSeconds);
        StopLogging();
        m_cycleRunning = false;
    }
}
