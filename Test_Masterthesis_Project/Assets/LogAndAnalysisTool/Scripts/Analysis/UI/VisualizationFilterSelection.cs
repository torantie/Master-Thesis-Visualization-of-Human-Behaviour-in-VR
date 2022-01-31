using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualizationFilterSelection
{
    public VisualizationFilterSelection(VisualizationType a_visualization, Dictionary<FilterCategory, List<string>> a_filterSelections, LoggingDataPointType a_loggingDataPointType)
    {
        Visualization = a_visualization;
        FilterSelections = a_filterSelections;
        LoggingDataPointType = a_loggingDataPointType;
    }

    public LoggingDataPointType LoggingDataPointType { get; private set; }

    public VisualizationType Visualization { get; private set; }

    public Dictionary<FilterCategory, List<string>> FilterSelections { get; private set; }
}
