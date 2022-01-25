using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FilterPanel : MenuPanel
{
    [SerializeField]
    private VisualizationPanel m_visualizationPanel;

    [SerializeField]
    private FilterPanelContent m_filterPanelContent;

    [SerializeField]
    private DataQueryManager m_dataQueryManager;

    void Awake()
    {
        if (m_filterPanelContent == null)
        {
            Debug.LogError("No FilterPanelContent script attached to children.");
        }
    }

    private void OnEnable()
    {
        UpdateFilterCategories();
    }

    /// <summary>
    /// Activate and deactivate filter categories based on the selected visualizations.
    /// </summary>
    public void UpdateFilterCategories()
    {
        m_filterPanelContent.HideAllFilterCategories();

        foreach (var visualizationSelection in m_visualizationPanel.GetVisualizationSelection())
        {
            FillCategoryObjectsContent(visualizationSelection);
        }
    }

    public void FillCategoryObjectsContent(VisualizationType a_visualizationType)
    {
        var filterCategories = Enum.GetValues(typeof(FilterCategory));
        var filterSelections = new Dictionary<FilterCategory, List<string>>();
        foreach (FilterCategory filterCategory in filterCategories)
        {
            List<string> values = filterCategory switch
            {
                FilterCategory.Task => m_dataQueryManager.GetStudyTasks(),
                FilterCategory.Participant => m_dataQueryManager.GetStudyParticipants(),
                FilterCategory.EventType => m_dataQueryManager.GetLoadedSpatialEventTypes(),
                FilterCategory.Sentiment => a_visualizationType.Equals(VisualizationType.HeatMap) ? 
                Enum.GetNames(typeof(Sentiment)).ToList() : new List<string>(),
                _ => new List<string>(),
            };
            filterSelections[filterCategory] = values;
        }
        m_filterPanelContent.AddFilterCategories(a_visualizationType, filterSelections);
    }

    public void RemoveCategoryObjectsContent(VisualizationType a_visualizationType)
    {
        m_filterPanelContent.RemoveFilterCategories(a_visualizationType);
    }

    public List<VisualizationFilterSelection> GetSelection()
    {
        return m_filterPanelContent.GetVisualizationFilterSelection();
    }
}
