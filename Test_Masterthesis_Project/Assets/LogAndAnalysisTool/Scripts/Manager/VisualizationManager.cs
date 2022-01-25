using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VisualizationManager : MonoBehaviour
{
    [SerializeField]
    private DataQueryManager m_dataQueryManager;

    [SerializeField]
    private List<PrefabVisualizationMapping> m_prefabVisualizationMapping = new List<PrefabVisualizationMapping>();

    [SerializeField]
    private ColorMapper m_colorMapper;

    private readonly Dictionary<string, VisualizationRenderer> m_visualizationRenderers = new Dictionary<string, VisualizationRenderer>();

    public Dictionary<string, VisualizationRenderer> VisualizationRenderers => m_visualizationRenderers;


    /// <summary>
    /// Get all renderers of the specified VisualizationType.
    /// </summary>
    /// <typeparam name="T">VisualizationRenderer</typeparam>
    /// <param name="a_visualizationType">type thats used to find visualizations.</param>
    /// <param name="a_renderers">Found renderers.</param>
    /// <returns>True if renderers were found for the specified VisualizationType.</returns>
    public bool TryGetRenderers<T>(VisualizationType a_visualizationType, out List<T> a_renderers) where T : VisualizationRenderer
    {
        a_renderers = new List<T>();

        foreach (var visualizationRenderersKvp in VisualizationRenderers)
        {
            var renderer = visualizationRenderersKvp.Value;
            if (renderer.VisualizationType.Equals(a_visualizationType))
            {
                a_renderers.Add((T)renderer);
            }
        }

        if (a_renderers.Count == 0)
            return false;

        return true;
    }

    public void ShowVisualizations(List<VisualizationFilterSelection> a_visualizationFilterSelections, Dictionary<VisualizationType, GroupingCategory> a_groupingSelection)
    {
        foreach (var visualizationRenderers in VisualizationRenderers)
        {
            visualizationRenderers.Value.ClearVisualization();
            Destroy(visualizationRenderers.Value.gameObject);
        }
        VisualizationRenderers.Clear();

        foreach (var visualizationFilterSelection in a_visualizationFilterSelections)
        {
            if (!a_groupingSelection.TryGetValue(visualizationFilterSelection.Visualization, out var grouping))
            {
                Debug.LogFormat("No grouping for visualization {0}.", visualizationFilterSelection.Visualization);
            }

            SelectAndShowVisualization(visualizationFilterSelection, grouping);
        }
    }

    private void SelectAndShowVisualization(VisualizationFilterSelection a_visualizationFilterSelection, GroupingCategory a_grouping)
    {
        IEnumerable<SpatialLoggingDataPoint> ungroupedData;
        Dictionary<string, List<SpatialLoggingDataPoint>> groupedData;
        var visualizationType = a_visualizationFilterSelection.Visualization;
        var filterSelections = a_visualizationFilterSelection.FilterSelections;
        var loggingDataPointType = a_visualizationFilterSelection.LoggingDataPointType;

        switch (visualizationType)
        {
            case VisualizationType.HeatMap:

                ungroupedData = m_dataQueryManager.GetOrLoadDataPoints<SpatialLoggingDataPoint>(loggingDataPointType, filterSelections);
                var heatMapRenderer = CreateOrGetVisualizationRenderer<HeatMapRenderer>(VisualizationType.HeatMap, loggingDataPointType);
                heatMapRenderer.ShowVisualization(ungroupedData, loggingDataPointType, filterSelections);
                break;
            case VisualizationType.PointCloud:
            case VisualizationType.LineGraph:
                if (m_colorMapper.TryGetColorMapping(a_grouping, out var colorMapping))
                {
                    groupedData = m_dataQueryManager.GetOrLoadDataPoints<SpatialLoggingDataPoint>(loggingDataPointType, filterSelections, a_grouping);
                    var groupedVisualizationRenderer = CreateOrGetVisualizationRenderer<GroupedVisualizationRenderer>(visualizationType, loggingDataPointType);
                    groupedVisualizationRenderer.ColorMapping = colorMapping;
                    groupedVisualizationRenderer.ShowVisualization(groupedData, loggingDataPointType);
                }
                break;
            case VisualizationType.None:
            default:
                Debug.LogError("No visualization selected to show.");
                break;
        }
    }

    private T CreateOrGetVisualizationRenderer<T>(VisualizationType a_visualizationType, LoggingDataPointType a_loggingDataPointType) where T : VisualizationRenderer
    {
        var visualizationName = string.Format("{0} {1}", a_visualizationType.ToFriendlyString(), a_loggingDataPointType.ToString());

        if (VisualizationRenderers.ContainsKey(visualizationName))
        {
            Debug.LogFormat("Visualization with name {0} already exists.", visualizationName);
        }
        else
        {
            T instantiatedRenderer = CreateVisualizationRenderer<T>(a_visualizationType, visualizationName);
            VisualizationRenderers[visualizationName] = instantiatedRenderer;
        }

        return (T)VisualizationRenderers[visualizationName];
    }


    private T CreateVisualizationRenderer<T>(VisualizationType a_visualizationType, string a_visualizationName) where T : VisualizationRenderer
    {
        var mapping = m_prefabVisualizationMapping.FirstOrDefault(mapping => mapping.VisualizationType.Equals(a_visualizationType));
        var instantiatedRenderer = Instantiate(mapping.Prefab, transform).GetComponent<T>();
        instantiatedRenderer.name = a_visualizationName;
        return instantiatedRenderer;
    }

}

[Serializable]
public class PrefabVisualizationMapping
{
    [SerializeField]
    private GameObject prefab;

    [SerializeField]
    private VisualizationType visualizationType;

    public GameObject Prefab { get => prefab; set => prefab = value; }

    public VisualizationType VisualizationType { get => visualizationType; set => visualizationType = value; }
}
