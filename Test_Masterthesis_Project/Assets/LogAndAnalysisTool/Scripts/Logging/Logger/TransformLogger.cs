using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TransformLogger : MonoBehaviour
{
    [SerializeField]
    private List<TrackableTransform> m_trackableTransforms = new List<TrackableTransform>();

    void Start()
    {
        FindTrackableTransforms();
    }


    public List<TransformTrackingDataPoint> GetDataPoints(StudyManager a_studyManager, System.Guid a_guid, System.DateTime a_now)
    {
        var dataPoints = new List<TransformTrackingDataPoint>();
        foreach (var trackableTransform in m_trackableTransforms)
        {
            dataPoints.Add(new TransformTrackingDataPoint(a_studyManager, a_guid, a_now, trackableTransform.transform.position)
            {
                transformId = trackableTransform.TransformId,
                rotation = trackableTransform.transform.rotation.ToString(),
                scale = trackableTransform.transform.localScale.ToString(),
            });
        }

        return dataPoints;
    }

    public void FindTrackableTransforms()
    {
        m_trackableTransforms.Clear();
        var trackableTransforms = FindObjectsOfType<TrackableTransform>();
        foreach (var trackableTransform in trackableTransforms)
        {
            m_trackableTransforms.Add(trackableTransform);
        }
    }

    public bool TryGetDuplicateWarning(out string a_warningText)
    {
        a_warningText = "";
        if (!TryGetIdDuplicates(out var duplicates))
        {
            return false;
        }
        List<string> list = new List<string>();
        foreach (var duplicate in duplicates)
        {
            var duplicateText = $"transform id: {duplicate.Key}, gameObjects: ({string.Join(",", duplicate.Value.Select(trackableTransform => trackableTransform.gameObject.name))})";
            list.Add(duplicateText);
        }
        a_warningText = $"Trackable transform id duplicates: {string.Join(";", list)}";


        Debug.LogWarning(a_warningText);

        return true;
    }

    public bool TryGetIdDuplicates(out IEnumerable<KeyValuePair<string, List<TrackableTransform>>> a_duplicates)
    {
        var dictionary = new Dictionary<string, List<TrackableTransform>>();
        m_trackableTransforms.ForEach(trackableTransform =>
        {
            if (!dictionary.ContainsKey(trackableTransform.TransformId) || dictionary[trackableTransform.TransformId] == null)
            {
                dictionary[trackableTransform.TransformId] = new List<TrackableTransform>();
            }
            dictionary[trackableTransform.TransformId].Add(trackableTransform);
        });

        a_duplicates = dictionary.Where(keyValuePair => keyValuePair.Value.Count > 1);


        if (a_duplicates.Count() != 0)
        {
            return true;
        }

        return false;
    }

    private void OnValidate()
    {
        FindTrackableTransforms();
    }


}
