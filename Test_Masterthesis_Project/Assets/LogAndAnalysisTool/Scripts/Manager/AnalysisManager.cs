using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnalysisManager : MonoBehaviour
{
    [SerializeField]
    private StudyManager m_studyManager;

    [SerializeField]
    private ReplayManager m_replayManager;

    [SerializeField]
    private DataQueryManager m_dataQueryManager;

    public StudyManager StudyManager { get => m_studyManager; private set => m_studyManager = value; }

    private static AnalysisManager m_instance;

    public static AnalysisManager Instance { get { return m_instance; } }

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
    }

    // Start is called before the first frame update
    void Start()
    {
        m_studyManager.SetActiveStudiesSessions();

        MapPositionToAudioEvent();

        var audioEventDataPoints = new List<SpatialAudioEventDataPoint>();
        var nlpCalcResponses = new List<NLPCalcResponse>();

        foreach (var studySession in m_studyManager.ActiveStudy.studySessions)
        {
            foreach (var studyParticipant in studySession.studyParticipants)
            {
                var textFileReader = new TextFileReader(StudyManager.ActiveStudy, studySession, studyParticipant);
                var textFileWriter = new TextFileWriter(StudyManager.ActiveStudy, studySession, studyParticipant);

                if (textFileReader.TryReadLoggingDataPoints<OfflineSpatialAudioEventDataPoint>(LoggingDataPointType.OfflineSpatialAudioEvent, out var audioEventDataPointsArray))
                {
                    audioEventDataPoints.AddRange(audioEventDataPointsArray);
                }
                if (!textFileReader.TryReadFromNLPCalc(out var nlpCalcResponsesArray))
                {
                    var sentimentCalcRequester = new NLPCalcRequester();
                    StartCoroutine(sentimentCalcRequester.Post(audioEventDataPointsArray, textFileWriter));
                }
                else
                {
                    nlpCalcResponses.AddRange(nlpCalcResponsesArray);
                }
            }
        }

        if (nlpCalcResponses.Count == 0)
        {
            foreach (var studySession in m_studyManager.ActiveStudy.studySessions)
            {
                foreach (var studyParticipant in studySession.studyParticipants)
                {
                    var textFileReader = new TextFileReader(StudyManager.ActiveStudy, studySession, studyParticipant);
                    if (textFileReader.TryReadFromNLPCalc(out var nlpCalcResponsesArray))
                    {
                        nlpCalcResponses.AddRange(nlpCalcResponsesArray);
                    }
                }
            }
        }
        m_dataQueryManager.LoadAllDataPoints();

    }


    /// <summary>
    /// Maps a position from ParticipantTrackingDataPoints to OfflineAudioEventDataPoints to create OfflineSpatialAudioEventDataPoint.
    /// This is only done once for every participant.
    /// </summary>
    /// <param name="a_overwrite">In case the OfflineSpatialAudioEventDataPoint need to be recalculated set to true.</param>
    public void MapPositionToAudioEvent(bool a_overwrite = false)
    {
        m_studyManager.SetActiveStudiesSessions();

        foreach (var studySession in m_studyManager.ActiveStudy.studySessions)
        {
            foreach (var studyParticipant in studySession.studyParticipants)
            {
                var textFileReader = new TextFileReader(m_studyManager.ActiveStudy, studySession, studyParticipant);
                var textFileWriter = new TextFileWriter(m_studyManager.ActiveStudy, studySession, studyParticipant);
                if (!textFileReader.LoggingDataPointsExist(LoggingDataPointType.OfflineSpatialAudioEvent) || a_overwrite)
                {
                    if (textFileReader.TryReadLoggingDataPoints<ParticipantTrackingDataPoint>(LoggingDataPointType.ParticipantTrackingData, out var participantTrackingDataPointArray))
                    {
                        if (textFileReader.TryReadLoggingDataPoints<OfflineAudioEventDataPoint>(LoggingDataPointType.OfflineAudioEvent, out var offlineAudioEventDataPointArray))
                        {
                            List<OfflineSpatialAudioEventDataPoint> offlineSpatialAudioEventDataPoints = new List<OfflineSpatialAudioEventDataPoint>();

                            foreach (var offlineAudioEventData in offlineAudioEventDataPointArray)
                            {
                                ParticipantTrackingDataPoint prevParticipant = null;
                                OfflineSpatialAudioEventDataPoint offlineSpatialAudioEventDataPoint = null;

                                foreach (var participant in participantTrackingDataPointArray)
                                {
                                    if (offlineSpatialAudioEventDataPoint == null)
                                    {
                                        if (participant.pointInTimeUtc.Equals(offlineAudioEventData.pointInTimeUtc)
                                            || (prevParticipant != null && participant.pointInTimeUtc >= offlineAudioEventData.pointInTimeUtc))
                                        {
                                            var leftMsDistance = (prevParticipant.pointInTimeUtc - offlineAudioEventData.pointInTimeUtc).TotalMilliseconds;
                                            var rightMsDistance = (participant.pointInTimeUtc - offlineAudioEventData.pointInTimeUtc).TotalMilliseconds;
                                            double timeOfPositionOffset;
                                            string position;
                                            if (Math.Abs(leftMsDistance) > rightMsDistance)
                                            {
                                                timeOfPositionOffset = rightMsDistance;
                                                position = participant.position;
                                            }
                                            else
                                            {
                                                timeOfPositionOffset = leftMsDistance;
                                                position = prevParticipant.position;
                                            }
                                            offlineSpatialAudioEventDataPoint = offlineAudioEventData.ParseToSpatialOfflineAudioEvent(position);
                                            offlineSpatialAudioEventDataPoint.startOffsetInMs = timeOfPositionOffset;
                                        }
                                    }
                                    else
                                    {
                                        if (participant.pointInTimeUtc.Equals(offlineAudioEventData.endPointInTimeUtc)
                                            || (prevParticipant != null && participant.pointInTimeUtc >= offlineAudioEventData.endPointInTimeUtc))
                                        {
                                            var leftMsDistance = (prevParticipant.pointInTimeUtc - offlineAudioEventData.endPointInTimeUtc).TotalMilliseconds;
                                            var rightMsDistance = (participant.pointInTimeUtc - offlineAudioEventData.endPointInTimeUtc).TotalMilliseconds;
                                            double timeOfPositionOffset;
                                            string position;
                                            if (Math.Abs(leftMsDistance) > rightMsDistance)
                                            {
                                                timeOfPositionOffset = rightMsDistance;
                                                position = participant.position;
                                            }
                                            else
                                            {
                                                timeOfPositionOffset = leftMsDistance;
                                                position = prevParticipant.position;
                                            }
                                            offlineSpatialAudioEventDataPoint.endPosition = position;
                                            offlineSpatialAudioEventDataPoint.endOffsetInMs = timeOfPositionOffset;
                                            offlineSpatialAudioEventDataPoints.Add(offlineSpatialAudioEventDataPoint);
                                            break;
                                        }
                                    }
                                    prevParticipant = participant;
                                }
                            }

                            textFileWriter.WriteToCSV(offlineSpatialAudioEventDataPoints, LoggingDataPointType.OfflineSpatialAudioEvent, a_overwrite);
                        }
                    }
                }
                else
                {
                    Debug.Log("OfflineSpatialAudioEvent already calculated.");
                }
            }
        }

    }


}
