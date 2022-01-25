using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GroupingPanel : MenuPanel
{
    [SerializeField]
    private VisualizationPanel m_visualizationPanel;

    [SerializeField]
    private GameObject m_uiGroupingPrefab;

    [SerializeField]
    private GameObject m_groupingsContent;

    /// <summary>
    /// Collection of all UIGrouping elements associated with a VisualizationType.
    /// </summary>
    public Dictionary<VisualizationType, UIGrouping> GroupingCollection { get; private set; } = new Dictionary<VisualizationType, UIGrouping>();


    private void OnEnable()
    {
        UpdateGroupings();
    }

    /// <summary>
    /// Activate and deactivate groupings based on the selected visualizations.
    /// </summary>
    public void UpdateGroupings()
    {
        foreach (var groupingKvp in GroupingCollection)
        {
            groupingKvp.Value.gameObject.SetActive(false);
        }

        foreach (var visualizationSelection in m_visualizationPanel.GetVisualizationSelection())
        {
            AddOrActivateGrouping(visualizationSelection);
        }
    }

    /// <summary>
    /// Add Grouping to panel depending on the visualization.
    /// </summary>
    /// <param name="a_visualizationType">VisualizationType associated with grouping.</param>
    public void AddOrActivateGrouping(VisualizationType a_visualizationType)
    {
        switch (a_visualizationType)
        {
            case VisualizationType.PointCloud:
            case VisualizationType.LineGraph:
                CreateOrActivateGrouping(a_visualizationType);
                break;
            case VisualizationType.None:
            case VisualizationType.HeatMap:
            default:
                break;
        }
    }

    /// <summary>
    /// Get selected grouping category for visualization.
    /// </summary>
    /// <returns>Dictionary of selected grouping category for visualization.</returns>
    public Dictionary<VisualizationType, GroupingCategory> GetSelection()
    {
        Dictionary<VisualizationType, GroupingCategory> selection = new Dictionary<VisualizationType, GroupingCategory>();
        foreach (var groupingKvp in GroupingCollection)
        {
            if (Enum.TryParse<GroupingCategory>(groupingKvp.Value.GetSingleSelection().selectedItem, true, out var groupingCategory))
            {
                selection[groupingKvp.Key] = groupingCategory;
            }
        }

        return selection;
    }

    /// <summary>
    /// Create or activate a ui grouping.
    /// </summary>
    /// <param name="a_visualizationType">The visualization that is associated with the grouping settings.</param>
    private void CreateOrActivateGrouping(VisualizationType a_visualizationType)
    {
        var groupingCategories = GroupingCategoriesToString();
        if (!GroupingCollection.ContainsKey(a_visualizationType))
        {
            var uiGrouping = Instantiate(m_uiGroupingPrefab, m_groupingsContent.transform).GetComponent<UIGrouping>();
            uiGrouping.ContentFiller.OnlyOneToggle = true;
            uiGrouping.SetContent(a_visualizationType.ToFriendlyString(), groupingCategories);
            GroupingCollection.Add(a_visualizationType, uiGrouping);
        }
        else
        {
            GroupingCollection[a_visualizationType].gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Parse grouping categories to string list.
    /// </summary>
    /// <returns>Grouping categories as string list.</returns>
    private List<string> GroupingCategoriesToString()
    {
        var groupingCategoriesCollection = new List<string>();

        foreach (GroupingCategory groupingCategory in Enum.GetValues(typeof(GroupingCategory)))
        {
            if (!groupingCategory.Equals(GroupingCategory.None))
                groupingCategoriesCollection.Add(groupingCategory.ToString());
        }

        return groupingCategoriesCollection;
    }

}
