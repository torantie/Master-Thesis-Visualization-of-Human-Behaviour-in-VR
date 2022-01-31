using System.Collections.Generic;
using UnityEngine;

public abstract class UngroupedVisualizationRenderer : VisualizationRenderer
{
    public abstract void ShowVisualization<T>(IEnumerable<T> a_spatialLoggingDataPoints, LoggingDataPointType a_loggingDataPointType, Dictionary<FilterCategory, List<string>> a_filterSelections) where T : SpatialLoggingDataPoint;

}

