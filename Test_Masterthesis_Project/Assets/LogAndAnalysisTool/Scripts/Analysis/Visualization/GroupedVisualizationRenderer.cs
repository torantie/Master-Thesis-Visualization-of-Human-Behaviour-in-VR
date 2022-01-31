using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GroupedVisualizationRenderer : VisualizationRenderer
{
    public ColorMapping ColorMapping { get; set; } = new ColorMapping();

    // Start is called before the first frame update
    void Start()
    {
        if (m_debugMode)
            ShowVisualization(DebugPointCollectionGenerator(), LoggingDataPointType.SpatialAudioEvent);
    }

    public abstract void ShowVisualization<T>(Dictionary<string, List<T>> a_spatialLoggingDataPoints, LoggingDataPointType a_loggingDataPointType) where T : SpatialLoggingDataPoint;

    private Dictionary<string, List<SpatialAudioEventDataPoint>> DebugPointCollectionGenerator()
    {
        Dictionary<string, List<SpatialAudioEventDataPoint>> lines = new Dictionary<string, List<SpatialAudioEventDataPoint>>();
        for (int i = 0; i < 7; i++)
        {
            lines[i.ToString()] = DebugPointGenerator(220, 0.3f);
        }
        return lines;
    }

    private List<SpatialAudioEventDataPoint> DebugPointGenerator(int a_numberOfPoints, float a_steps)
    {
        List<SpatialAudioEventDataPoint> list = new List<SpatialAudioEventDataPoint>();
        float newX = 2;
        float newZ = 1;
        for (int i = 0; i < a_numberOfPoints; i++)
        {
            var randomNumber = Random.value;
            if (randomNumber >= 0.5)
            {
                newX += a_steps;
                newZ -= a_steps;

            }
            else
            {
                newX += a_steps;
                newZ += a_steps;
            }
            var v = new Vector3(newX, 1, newZ);
            list.Add(new SpatialAudioEventDataPoint() { position = v.ToString() });
        }

        return list;
    }
}
