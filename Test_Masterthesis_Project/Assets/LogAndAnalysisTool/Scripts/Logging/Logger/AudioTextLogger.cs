#if (UNITY_STANDALONE_WIN)
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;
using UnityEngine.XR;

/// <summary>
/// Todo Windwos einstellungen nicht vergessen.
/// https://docs.unity3d.com/2021.2/Documentation/ScriptReference/Windows.Speech.DictationRecognizer.html
/// </summary>
public class AudioTextLogger : EventLogger, IDisposable
{
    [SerializeField]
    private LoggingManager m_loggingManager;

    [SerializeField]
    private HMDLogger m_hmdLogger;

    private DictationRecognizer m_dictationRecognizer;

    private bool m_isDisposed;

    //See initialization. Subtract 2 seconds since the Hypothesis event is not thrown immediately
    private DateTime m_speechStartedTime;

    private Vector3 m_startPosition;

    private bool m_speechStarted;

    private float m_waitTime = TimeBetweenAudioEvents;

    private bool m_isWaiting = false;

    private const float TimeBetweenAudioEvents = 5f;

    private (string, ConfidenceLevel) m_text = ("", ConfidenceLevel.Rejected);

    private bool m_isStarted;


    public override void StartListening()
    {
        InitAndStartDictationRecognizer();
    }

    public override void StopListening()
    {
        DisposeAndStopDictationRecognizer();
    }

    //private void StopAndDeinit()
    //{
    //    Debug.LogFormat("Before StopAndDeinit AudioTextLogger: {0}", DateTime.UtcNow);
    //    if (m_dictationRecognizer != null)
    //    {
    //        Debug.LogFormat("Before StopAndDeinit Stop: {0}", DateTime.UtcNow);
    //        m_dictationRecognizer.Stop();
    //        Debug.LogFormat("After StopAndDeinit Stop: {0}", DateTime.UtcNow);
    //        Debug.LogFormat("Before StopAndDeinit Dispose: {0}", DateTime.UtcNow);
    //        //Takes about one second
    //        m_dictationRecognizer.Dispose();
    //        Debug.LogFormat("After StopAndDeinit Dispose: {0}", DateTime.UtcNow);
    //        m_dictationRecognizer = null;
    //    }
    //    m_isStarted = false;
    //    Debug.LogFormat("After StopAndDeinit AudioTextLogger: {0}", DateTime.UtcNow);
    //}

    private void InitAndStartDictationRecognizer()
    {
        InitDictationRecognizer();
        StartDictationRecognizer();
    }

    public void DisposeAndStopDictationRecognizer()
    {
        if (m_dictationRecognizer != null)
        {
            m_dictationRecognizer.DictationHypothesis -= OnDictationHypothesis;
            m_dictationRecognizer.DictationComplete -= OnDictationComplete;
            m_dictationRecognizer.DictationResult -= OnDictationResult;
            m_dictationRecognizer.DictationError -= OnDictationError;
            StopDictationRecognizer();
            Debug.LogFormat("Before DisposeDictationRecognizer Dispose: {0}", DateTime.UtcNow);
            m_dictationRecognizer.Dispose();
            Debug.LogFormat("After DisposeDictationRecognizer Dispose: {0}", DateTime.UtcNow);
            m_dictationRecognizer = null;
        }
    }

    private void StopDictationRecognizer()
    {
        Debug.LogFormat("Before StopDictationRecognizer Stop: {0}", DateTime.UtcNow);
        m_dictationRecognizer.Dispose();
        if (m_dictationRecognizer.Status == SpeechSystemStatus.Running)
        {
            m_dictationRecognizer.Stop();
        }
        m_isStarted = false;
        Debug.LogFormat("Dictation StopDictationRecognizer m_dictationRecognizer.Status: {0}", m_dictationRecognizer.Status);
        Debug.LogFormat("After StopDictationRecognizer Stop: {0}", DateTime.UtcNow);
    }

    private void StartDictationRecognizer()
    {
        if (!m_isStarted)
        {
            Debug.LogFormat("Dictation started.");
            m_dictationRecognizer.Start();
            m_isStarted = true;
            Debug.LogFormat("Dictation StartDictationRecognizer m_dictationRecognizer.Status: {0}", m_dictationRecognizer.Status);
        }
        else
        {
            Debug.LogFormat("Dictation already started.");
        }
    }

    private void InitDictationRecognizer()
    {
        if (m_dictationRecognizer == null)
        {
            Debug.LogFormat("Dictation Recognizer initialized.");
            m_dictationRecognizer = new DictationRecognizer
            {
                AutoSilenceTimeoutSeconds = float.NegativeInfinity,
                InitialSilenceTimeoutSeconds = float.NegativeInfinity,
            };

            m_dictationRecognizer.DictationResult += OnDictationResult;

            m_dictationRecognizer.DictationHypothesis += OnDictationHypothesis;

            m_dictationRecognizer.DictationComplete += OnDictationComplete;

            m_dictationRecognizer.DictationError += OnDictationError;
        }
        else
        {
            Debug.LogFormat("Dictation Recognizer already initialized.");
        }

    }

    private void Restart()
    {
        Debug.LogFormat("Dictation restart.");
        DisposeAndStopDictationRecognizer();
        InitDictationRecognizer();
        StartDictationRecognizer();
    }

    private void OnDictationResult(string a_text, ConfidenceLevel a_confidence)
    {
        try
        {
            Debug.LogFormat("Dictation result: {0}, {1}", a_text, a_confidence);

            if (m_speechStarted)
            {
                var endTime = DateTime.UtcNow;
                var endPosition = m_hmdLogger?.GetTransfromPosition().ToString();
                var dataPointGuid = Guid.NewGuid();

                var audioEventDataPoint = new SpatialAudioEventDataPoint(m_loggingManager.StudyManager, dataPointGuid, m_speechStartedTime, m_startPosition)
                {
                    endPointInTimeUtc = endTime,
                    spokenText = a_text,
                    confidenceLevel = ConfidenceLevelToFloat(a_confidence),
                    endPosition = endPosition,
                };
                m_loggingManager.OnEventTriggered(audioEventDataPoint);
                m_speechStarted = false;
                Debug.LogFormat("Speech DictationResult at {0}.", endTime);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    private void OnDictationError(string a_error, int a_hresult)
    {
        Debug.LogErrorFormat("Dictation error: {0}; HResult = {1}.", a_error, a_hresult);
        //InitAndStart();
        Restart();
    }

    private void OnDictationComplete(DictationCompletionCause a_completionCause)
    {
        Debug.LogFormat("Dictation completed. Cause: {0}.", a_completionCause);
        if (a_completionCause != DictationCompletionCause.Complete)
        {
            Debug.LogWarningFormat("Dictation failed. Writing rejected audio event.");

            var time = DateTime.UtcNow;
            var position = m_hmdLogger ? m_hmdLogger.GetTransfromPosition() : Vector3.zero;
            var dataPointGuid = Guid.NewGuid();
            var audioEventDataPoint = new SpatialAudioEventDataPoint(m_loggingManager.StudyManager, dataPointGuid, time, position)
            {
                endPointInTimeUtc = time,
                spokenText = "",
                confidenceLevel = ConfidenceLevelToFloat(ConfidenceLevel.Rejected),
                endPosition = position.ToString(),
            };
            m_loggingManager.OnEventTriggered(audioEventDataPoint);
            m_speechStarted = false;
        }

        //InitAndStart();
        Debug.LogFormat("Dictation completed. Cause: {0}. m_dictationRecognizer.Status: {1}", a_completionCause, m_dictationRecognizer.Status);
        Restart();
    }

    private void OnDictationHypothesis(string a_text)
    {
        try
        {
            Debug.LogFormat("Dictation hypothesis: {0}", a_text);

            if (!m_speechStarted)
            {
                //Subtract 2 seconds since the Hypothesis event is not thrown immediately
                m_speechStartedTime = DateTime.UtcNow.Subtract(new TimeSpan(0, 0, 2));
                m_startPosition = m_hmdLogger.GetTransfromPosition();
                m_speechStarted = true;
                m_isWaiting = true;
                Debug.LogFormat("Speech Started at {0}.", m_speechStartedTime);
            }

        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    public float ConfidenceLevelToFloat(ConfidenceLevel a_confidenceLevel)
    {
        return a_confidenceLevel switch
        {
            ConfidenceLevel.High => 1f,
            ConfidenceLevel.Medium => 0.5f,
            ConfidenceLevel.Low => 0.1f,
            _ => 0f,
        };
    }

    void Update()
    {
        if (!m_isStarted || m_dictationRecognizer == null)
            return;

        if (m_dictationRecognizer.Status != SpeechSystemStatus.Running)
        {
            Debug.LogFormat("Dictation restarted in update. m_DictationRecognizer.Status: {0}", m_dictationRecognizer.Status);
            m_dictationRecognizer.Start();
            //InitAndStart();
        }

        if (m_isWaiting)
        {
            m_waitTime -= Time.deltaTime;

            if (m_waitTime <= 0.0f)
            {
                m_waitTime = TimeBetweenAudioEvents;
                m_isWaiting = false;
            }
        }
    }

    public void Dispose()
    {
        if (!m_isDisposed)
        {
            //Takes about one second
            //m_dictationRecognizer.Dispose();
            //m_dictationRecognizer = null;
            DisposeAndStopDictationRecognizer();
            m_isDisposed = true;
        }
    }


    private void OnApplicationQuit()
    {
        DisposeAndStopDictationRecognizer();
    }
}

#endif