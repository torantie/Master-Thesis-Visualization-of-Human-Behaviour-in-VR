using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VisualizationType
{
    None, HeatMap, PointCloud, LineGraph
}
public static class VisualizationTypeExtensions
{
    private const string HeatMap = "Heat Map";

    private const string PointCloud = "Point Cloud";

    private const string LineGraph = "Line Graph";

    public static string ToFriendlyString(this VisualizationType a_visualizationType)
    {
        return a_visualizationType switch
        {
            VisualizationType.HeatMap => HeatMap,
            VisualizationType.PointCloud => PointCloud,
            VisualizationType.LineGraph => LineGraph,
            _ => "",
        };
    }

    public static VisualizationType ParseToVisualizationType(this string a_friendlyString)
    {
        return a_friendlyString switch
        {
            HeatMap => VisualizationType.HeatMap,
            PointCloud => VisualizationType.PointCloud,
            LineGraph => VisualizationType.LineGraph,
            _ => VisualizationType.None,
        };
    }
}