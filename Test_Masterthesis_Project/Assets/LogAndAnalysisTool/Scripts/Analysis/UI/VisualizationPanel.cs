using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VisualizationPanel : MenuPanel
{
    [SerializeField]
    private VisualizationManager m_visualizationManager;

    [SerializeField]
    private FilterPanel m_filterPanel;

    [SerializeField]
    private GroupingPanel m_groupingPanel;

    [SerializeField]
    private CheckboxScrollViewContentHandler m_visualizationSelection;

    [SerializeField]
    private GameObject m_content;

    [SerializeField]
    private GameObject m_legendPrefab;

    [SerializeField]
    private ColorMapper m_colorMapper;

    private Dictionary<string, UILegend> m_legends = new Dictionary<string, UILegend>();

    private List<VisualizationType> m_prevSelectedVisualizations = new List<VisualizationType>();

    private void Start()
    {
        m_visualizationSelection.OnToggleChange.AddListener(OnVisualizationSelectionChanged);
    }

    private void OnEnable()
    {
        FillVisualizationSelection();
    }

    /// <summary>
    /// Fill the options of visualizations.
    /// </summary>
    public void FillVisualizationSelection()
    {
        List<string> visualizationTypeNames = new List<string>();
        foreach (VisualizationType visualizationType in Enum.GetValues(typeof(VisualizationType)))
        {
            if (!visualizationType.Equals(VisualizationType.None))
                visualizationTypeNames.Add(visualizationType.ToFriendlyString());
        }
        m_visualizationSelection.Fill(visualizationTypeNames);
    }

    public List<VisualizationType> GetVisualizationSelection()
    {
        List<VisualizationType> visualizationTypes = new List<VisualizationType>();
        foreach (var selectedItem in m_visualizationSelection.GetSelectedItems())
        {
            visualizationTypes.Add(selectedItem.ParseToVisualizationType());
        }
        return visualizationTypes;
    }

    /// <summary>
    /// Show Visualizations based on the filter selections.
    /// </summary>
    public void OnShowVisualization()
    {
        var filterSelections = m_filterPanel.GetSelection();
        var groupingSelection = m_groupingPanel.GetSelection();
        m_visualizationManager.ShowVisualizations(filterSelections, groupingSelection);
        ShowLegends(filterSelections, groupingSelection);
    }
    /// <summary>
    /// Show the legends for the visualizations.
    /// </summary>
    /// <param name="a_filterSelections"></param>
    private void ShowLegends(List<VisualizationFilterSelection> a_filterSelections, Dictionary<VisualizationType, GroupingCategory> a_groupingSelection)
    {
        foreach (var legend in m_legends)
        {
            Destroy(legend.Value.gameObject);
        }
        m_legends.Clear();

        foreach (var visualizationRendererKvp in m_visualizationManager.VisualizationRenderers)
        {
            var renderer = visualizationRendererKvp.Value;
            switch (renderer.VisualizationType)
            {
                case VisualizationType.HeatMap:
                    var heatMapRenderer = renderer as HeatMapRenderer;
                    CreateLegendObject(heatMapRenderer, heatMapRenderer.GetReducedColorMapping(5));
                    break;
                case VisualizationType.LineGraph:
                case VisualizationType.PointCloud:
                    if (a_groupingSelection.TryGetValue(renderer.VisualizationType, out var grouping))
                    {
                        if (m_colorMapper.TryGetColorMapping(grouping, out var colorMapping))
                        {
                            CreateLegendObject(renderer, colorMapping);
                        }
                    }
                    break;
                default:
                    Debug.LogErrorFormat("No legend defined for {0}.", renderer.VisualizationType);
                    break;
            }
        }
    }

    private void CreateLegendObject(VisualizationRenderer a_visualizationRenderer, Dictionary<string, Color> a_colorMapping)
    {
        var legendPrefab = Instantiate(m_legendPrefab, m_content.transform);
        var uiLegend = legendPrefab.GetComponent<UILegend>();
        uiLegend.SetContent(a_visualizationRenderer.name, a_colorMapping);
        m_legends[a_visualizationRenderer.name] = uiLegend;
    }

    /// <summary>
    /// Update Groupings and FilterCategories on selected visualization changed.
    /// </summary>
    public void OnVisualizationSelectionChanged(GameObject a_gameObject, string a_text, bool a_on)
    {
        m_groupingPanel.UpdateGroupings();
        m_filterPanel.UpdateFilterCategories();
    }
}
