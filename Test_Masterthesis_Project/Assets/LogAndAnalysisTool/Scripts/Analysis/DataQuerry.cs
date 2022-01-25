using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// deprecated Test
/// </summary>
/// <typeparam name="T"></typeparam>
public class DataQuerry<T> where T : LoggingDataPoint
{
    [SerializeField]
    private StudyManager m_studyManager;

    private List<T> m_loggingDataPoints;

    public List<T> LoadDataPoints(LoggingDataPointType a_loggingDataPointType)
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
        return dataPoints;
    }

    public Dictionary<string, List<T>> GetDataPoints(LoggingDataPointType a_loggingDataPointType, Dictionary<FilterCategory, List<string>> a_filterSelections, GroupingCategory a_grouping) 
    {
        IEnumerable<T> dataPoints = new List<T>();
        var groupingPredicate = GetGroupingPredicate(a_grouping);
        var spatialDataPoints = GetDataPoints(a_loggingDataPointType, a_filterSelections);
        var data = spatialDataPoints.GroupBy(groupingPredicate).ToDictionary(g => g.Key, g => g.ToList());
        return data;
    }

    public Dictionary<string, List<T>> GetDataPoints(LoggingDataPointType a_loggingDataPointType, GroupingCategory a_grouping)
    {
        IEnumerable<T> dataPoints = new List<T>();
        var groupingPredicate = GetGroupingPredicate(a_grouping);
        var spatialDataPoints = GetDataPoints(a_loggingDataPointType);
        var data = spatialDataPoints.GroupBy(groupingPredicate).ToDictionary(g => g.Key, g => g.ToList());
        return data;
    }


    public IEnumerable<T> GetDataPoints(LoggingDataPointType a_loggingDataPointType, Dictionary<FilterCategory, List<string>> a_filterSelections)
    {
        var filterPredicate = GetFilterPredicate(a_filterSelections);
        IEnumerable<T> dataPoints = GetDataPoints(a_loggingDataPointType);
        return dataPoints.Where(filterPredicate);
    }

    public IEnumerable<T> GetDataPoints(LoggingDataPointType a_loggingDataPointType, bool a_reload = false)
    {
        //IEnumerable<T> dataPoints = a_loggingDataPointType switch
        //{
        //    LoggingDataPointType.ParticipantTrackingData => (IEnumerable<T>)GetParticipantData(),
        //    LoggingDataPointType.InputActionEvent => (IEnumerable<T>)GetInputActionEventData(),
        //    LoggingDataPointType.CustomSpatialEvent => (IEnumerable<T>)GetOrLoadCustomEventData(),
        //    LoggingDataPointType.TransformTrackingData => (IEnumerable<T>)GetOrLoadTransformData(),
        //    LoggingDataPointType.OfflineAudioEvent => (IEnumerable<T>)GetOrLoadOfflineAudioEventData(),
        //    LoggingDataPointType.SpatialAudioEvent => (IEnumerable<T>)GetOrLoadAudioEventData(),
        //    LoggingDataPointType.OfflineSpatialAudioEvent => (IEnumerable<T>)GetOrLoadOfflineSpatialAudioEventData(),
        //    _ => LoadDataPoints<T>(a_loggingDataPointType),
        //};
        if (a_reload || m_loggingDataPoints == null)
        {
            m_loggingDataPoints = LoadDataPoints(a_loggingDataPointType);
        }

        return m_loggingDataPoints;
    }

    private static Func<T, string> GetGroupingPredicate(GroupingCategory a_grouping)
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

    private static Func<T, bool> GetFilterPredicate(Dictionary<FilterCategory, List<string>> a_filterSelections)
    {
        return dataPoint => a_filterSelections[FilterCategory.Task].Contains(dataPoint.taskId)
                            && a_filterSelections[FilterCategory.Participant].Contains(dataPoint.participantId);
    }
}
