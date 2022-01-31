using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataQueryManager : MonoBehaviour
{
    [SerializeField]
    private StudyManager m_studyManager;

    private List<ParticipantTrackingDataPoint> m_participantTrackingDataPoints;

    private List<TransformTrackingDataPoint> m_transformTrackingDataPoints;

    private List<InputActionEventDataPoint> m_inputActionEventDataPoints;

    private List<SpatialAudioEventDataPoint> m_audioEventDataPoints;

    private List<OfflineAudioEventDataPoint> m_offlineAudioEventDataPoints;

    private List<OfflineSpatialAudioEventDataPoint> m_offlineSpatialAudioEventDataPoints;

    private List<SentimentEventDataPoint> m_sentimentEventDataPoints;

    private List<CustomSpatialEventDataPoint> m_customEventDataPoints;

    

    private Dictionary<LoggingDataPointType, int> m_loadedSpatialDataPointCount = new Dictionary<LoggingDataPointType, int>();

    private Dictionary<LoggingDataPointType, int> m_loadedNonSpatialDataPointCount = new Dictionary<LoggingDataPointType, int>();

    public void LoadAllDataPoints()
    {
        foreach (LoggingDataPointType loggingDataPointType in Enum.GetValues(typeof(LoggingDataPointType)))
        {
            GetOrLoadDataPoints<LoggingDataPoint>(loggingDataPointType);
        }
    }


    public Dictionary<string, List<T>> GetOrLoadDataPoints<T>(LoggingDataPointType a_loggingDataPointType, Dictionary<FilterCategory, List<string>> a_filterSelections, GroupingCategory a_grouping) where T : LoggingDataPoint
    {
        IEnumerable<T> dataPoints = new List<T>();
        var groupingPredicate = GetGroupingPredicate<T>(a_grouping);
        var spatialDataPoints = GetOrLoadDataPoints<T>(a_loggingDataPointType, a_filterSelections);
        var data = spatialDataPoints.GroupBy(groupingPredicate).ToDictionary(g => g.Key, g => g.ToList());
        return data;
    }

    public Dictionary<string, List<T>> GetOrLoadDataPoints<T>(LoggingDataPointType a_loggingDataPointType, GroupingCategory a_grouping) where T : LoggingDataPoint
    {
        IEnumerable<T> dataPoints = new List<T>();
        var groupingPredicate = GetGroupingPredicate<T>(a_grouping);
        var spatialDataPoints = GetOrLoadDataPoints<T>(a_loggingDataPointType);
        var data = spatialDataPoints.GroupBy(groupingPredicate).ToDictionary(g => g.Key, g => g.ToList());
        return data;
    }

    public IEnumerable<T> GetOrLoadDataPoints<T>(LoggingDataPointType a_loggingDataPointType, Dictionary<FilterCategory, List<string>> a_filterSelections) where T : LoggingDataPoint
    {
        var filterPredicate = GetFilterPredicate<T>(a_filterSelections);
        IEnumerable<T> dataPoints = GetOrLoadDataPoints<T>(a_loggingDataPointType);
        if (a_filterSelections.TryGetValue(FilterCategory.Sentiment, out var filterSelection))
        {
            dataPoints = dataPoints.Where(filterPredicate).Where(dataPoint =>
            {
                Debug.Log("InWhere");
                if (dataPoint is SentimentEventDataPoint sentimentEventDataPoint)
                {
                    Debug.Log("is SentimentEventDataPoint");
                    Debug.LogFormat("a_filterSelections[FilterCategory.Sentiment]: {0}", string.Join(",", a_filterSelections[FilterCategory.Sentiment])); ;
                    return a_filterSelections[FilterCategory.Sentiment].Contains(sentimentEventDataPoint.sentiment.ToString());
                }
                return false;
            });
        }
        return dataPoints.Where(filterPredicate).ToList();
    }

    public IEnumerable<T> GetOrLoadDataPoints<T>(LoggingDataPointType a_loggingDataPointType) where T : LoggingDataPoint
    {
        IEnumerable<T> dataPoints = a_loggingDataPointType switch
        {
            LoggingDataPointType.ParticipantTrackingData => (IEnumerable<T>)GetOrLoadParticipantData(),
            LoggingDataPointType.InputActionEvent => (IEnumerable<T>)GetOrLoadInputActionEventData(),
            LoggingDataPointType.CustomSpatialEvent => (IEnumerable<T>)GetOrLoadCustomEventData(),
            LoggingDataPointType.TransformTrackingData => (IEnumerable<T>)GetOrLoadTransformData(),
            LoggingDataPointType.OfflineAudioEvent => (IEnumerable<T>)GetOrLoadOfflineAudioEventData(),
            LoggingDataPointType.SpatialAudioEvent => (IEnumerable<T>)GetOrLoadAudioEventData(),
            LoggingDataPointType.OfflineSpatialAudioEvent => (IEnumerable<T>)GetOrLoadOfflineSpatialAudioEventData(),
            LoggingDataPointType.SentimentEvent => (IEnumerable<T>)GetOrLoadSentimentEventData(),
            _ => LoadDataPoints<T>(a_loggingDataPointType),
        };

        return dataPoints;
    }

    private List<T> LoadDataPoints<T>(LoggingDataPointType a_loggingDataPointType) where T : LoggingDataPoint
    {
        var dataPoints = new List<T>();

        foreach (var studySession in m_studyManager.ActiveStudy.studySessions)
        {
            foreach (var studyParticipant in studySession.studyParticipants)
            {
                var textFileReader = new TextFileReader(m_studyManager.ActiveStudy, studySession, studyParticipant);
                if (textFileReader.TryReadLoggingDataPoints<T>(a_loggingDataPointType, out var dataPointArray))
                {
                    dataPoints.AddRange(dataPointArray);
                }
            }
        }

        if (typeof(T).IsSubclassOf(typeof(SpatialLoggingDataPoint)) || typeof(T) == typeof(SpatialLoggingDataPoint))
        {
            m_loadedSpatialDataPointCount[a_loggingDataPointType] = dataPoints.Count;
        }
        else
        {
            m_loadedNonSpatialDataPointCount[a_loggingDataPointType] = dataPoints.Count;
        }

        return dataPoints;
    }

    private static Func<T, string> GetGroupingPredicate<T>(GroupingCategory a_grouping) where T : LoggingDataPoint
    {
        return dataPoint =>
        {
            return a_grouping switch
            {
                GroupingCategory.TaskId => dataPoint.taskId,
                GroupingCategory.ParticipantId => dataPoint.participantId,
                _ => "",
            };
        };
    }

    private static Func<T, bool> GetFilterPredicate<T>(Dictionary<FilterCategory, List<string>> a_filterSelections) where T : LoggingDataPoint
    {
        return dataPoint => a_filterSelections[FilterCategory.Task].Contains(dataPoint.taskId)
                            && a_filterSelections[FilterCategory.Participant].Contains(dataPoint.participantId);
    }



    private List<ParticipantTrackingDataPoint> GetOrLoadParticipantData(bool a_reload = false)
    {
        if (a_reload || m_participantTrackingDataPoints == null)
        {
            m_participantTrackingDataPoints = LoadDataPoints<ParticipantTrackingDataPoint>(LoggingDataPointType.ParticipantTrackingData);
        }

        return m_participantTrackingDataPoints;
    }

    private List<TransformTrackingDataPoint> GetOrLoadTransformData(bool a_reload = false)
    {
        if (a_reload || m_transformTrackingDataPoints == null)
        {
            m_transformTrackingDataPoints = LoadDataPoints<TransformTrackingDataPoint>(LoggingDataPointType.TransformTrackingData);
        }

        return m_transformTrackingDataPoints;
    }

    private List<InputActionEventDataPoint> GetOrLoadInputActionEventData(bool a_reload = false)
    {
        if (a_reload || m_inputActionEventDataPoints == null)
        {
            m_inputActionEventDataPoints = LoadDataPoints<InputActionEventDataPoint>(LoggingDataPointType.InputActionEvent);
        }

        return m_inputActionEventDataPoints;
    }

    private List<SpatialAudioEventDataPoint> GetOrLoadAudioEventData(bool a_reload = false)
    {
        if (a_reload || m_audioEventDataPoints == null)
        {
            m_audioEventDataPoints = LoadDataPoints<SpatialAudioEventDataPoint>(LoggingDataPointType.SpatialAudioEvent);
        }

        return m_audioEventDataPoints;
    }

    private List<OfflineAudioEventDataPoint> GetOrLoadOfflineAudioEventData(bool a_reload = false)
    {
        if (a_reload || m_offlineAudioEventDataPoints == null)
        {
            m_offlineAudioEventDataPoints = LoadDataPoints<OfflineAudioEventDataPoint>(LoggingDataPointType.OfflineAudioEvent);
        }

        return m_offlineAudioEventDataPoints;
    }

    private List<OfflineSpatialAudioEventDataPoint> GetOrLoadOfflineSpatialAudioEventData(bool a_reload = false)
    {
        if (a_reload || m_offlineSpatialAudioEventDataPoints == null)
        {
            m_offlineSpatialAudioEventDataPoints = LoadDataPoints<OfflineSpatialAudioEventDataPoint>(LoggingDataPointType.OfflineSpatialAudioEvent);
        }

        return m_offlineSpatialAudioEventDataPoints;
    }

    private List<SentimentEventDataPoint> GetOrLoadSentimentEventData(bool a_reload = false)
    {
        if (a_reload || m_sentimentEventDataPoints == null)
        {
            m_sentimentEventDataPoints = LoadDataPoints<SentimentEventDataPoint>(LoggingDataPointType.SentimentEvent);
        }

        return m_sentimentEventDataPoints;
    }

    private List<CustomSpatialEventDataPoint> GetOrLoadCustomEventData(bool a_reload = false)
    {
        if (a_reload || m_customEventDataPoints == null)
        {
            m_customEventDataPoints = LoadDataPoints<CustomSpatialEventDataPoint>(LoggingDataPointType.CustomSpatialEvent);
        }

        return m_customEventDataPoints;
    }

    //private List<T> GetOrLoadCustomEventData<T>(LoggingDataPointType a_loggingDataPointType,bool a_reload = false) where T : LoggingDataPoint
    //{
    //    if (a_reload || m_customEventDataPoints == null)
    //    {
    //        m_loggingDataPoints = LoadDataPoints<T>(a_loggingDataPointType);
    //    }

    //    return m_loggingDataPoints;
    //}

    public bool TryGroupBy<T>(List<T> a_listToGroup, Func<T, string> a_keySelector, out Dictionary<string, List<T>> a_groupedCollection) where T : LoggingDataPoint
    {
        try
        {
            a_groupedCollection = a_listToGroup.GroupBy(a_keySelector, v => v).ToDictionary(g => g.Key, g => g.ToList());

            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
        a_groupedCollection = null;
        return false;
    }

    public bool TryGetParticipantData(Func<ParticipantTrackingDataPoint, string> a_keySelector, out Dictionary<string, List<ParticipantTrackingDataPoint>> a_groupedCollection, bool a_reload = false)
    {
        var data = GetOrLoadParticipantData(a_reload);
        if (TryGroupBy(data, a_keySelector, out a_groupedCollection))
            return true;

        return false;

    }

    public bool TryGetTransformsData(Func<TransformTrackingDataPoint, string> a_keySelector, out Dictionary<string, List<TransformTrackingDataPoint>> a_groupedCollection, bool a_reload = false)
    {
        var data = GetOrLoadTransformData(a_reload);
        if (TryGroupBy(data, a_keySelector, out a_groupedCollection))
            return true;

        return false;

    }

    public List<string> GetIds(GroupingCategory a_groupingCategory)
    {
        return a_groupingCategory switch
        {
            GroupingCategory.TaskId => GetStudyTasks(),
            GroupingCategory.ParticipantId => GetStudyParticipants(),
            _ => new List<string>(),
        };
    }

    public List<string> GetStudyParticipants()
    {
        var studyParticipants = new List<string>();
        foreach (var studySession in m_studyManager.ActiveStudy.studySessions)
        {
            foreach (var studyParticipant in studySession.studyParticipants)
            {
                studyParticipants.Add(studyParticipant.id);
            }
        }

        return studyParticipants;
    }

    /// <summary>
    /// Todo
    /// </summary>
    /// <returns></returns>
    public List<string> GetStudyTasks()
    {
        var eventTypes = new HashSet<string>();

        foreach (var studySession in m_studyManager.ActiveStudy.studySessions)
        {
            foreach (var studyParticipant in studySession.studyParticipants)
            {
                TextFileReader textFileReader = new TextFileReader(m_studyManager.ActiveStudy, studySession, studyParticipant);
                if (textFileReader.TryReadFromLoggingInfo(out var participantLoggingInfo))
                {
                    foreach (var taskId in participantLoggingInfo.taskIds)
                    {
                        eventTypes.Add(taskId);
                    }
                }
            }
        }

        return eventTypes.ToList();
    }

    /// <summary>
    /// Todo maybe return names of custom events too
    /// </summary>
    /// <returns></returns>
    public List<string> GetLoadedSpatialEventTypes()
    {
        var eventTypes = new List<string>();
        foreach (var item in m_loadedSpatialDataPointCount)
        {
            if (item.Value != 0)
            {
                eventTypes.Add(item.Key.ToString());
            }
        }
        return eventTypes;
    }

}
