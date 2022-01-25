using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorMapper : MonoBehaviour
{
    [SerializeField]
    private DataQueryManager m_dataQueryManager;

    private Dictionary<GroupingCategory, ColorMapping> m_colorMappingsCollection { get; set; } = new Dictionary<GroupingCategory, ColorMapping>();



    public void SetColorMappings()
    {
        m_colorMappingsCollection.Clear();
        foreach (GroupingCategory groupingCategory in Enum.GetValues(typeof(GroupingCategory)))
        {
            var colorMappings = ColorGenerator.GetColorMappings(m_dataQueryManager.GetIds(groupingCategory));
            m_colorMappingsCollection[groupingCategory] = colorMappings;
        }
    }

    public bool TryGetColorMapping(GroupingCategory a_grouping, out ColorMapping a_colorMapping)
    {
        if (m_colorMappingsCollection.Count == 0)
            SetColorMappings();

        if (!m_colorMappingsCollection.TryGetValue(a_grouping, out a_colorMapping))
        {
            Debug.LogWarningFormat("No ColorMapping found for GroupingCategory {0}.", a_grouping);
            return false;
        }

        return true;
    }
}

public class ColorMapping : Dictionary<string, Color>
{
}