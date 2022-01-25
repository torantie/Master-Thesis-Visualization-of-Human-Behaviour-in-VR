using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HeatMapRenderer : UngroupedVisualizationRenderer
{
    [SerializeField]
    private List<MeshRenderer> m_meshesToOverlay = new List<MeshRenderer>();

    private readonly List<HeatMapVisual> m_heatMaps = new List<HeatMapVisual>();

    public List<MeshRenderer> MeshesToOverlay { get => m_meshesToOverlay; private set => m_meshesToOverlay = value; }

    public List<(string id, Color color)> ColorMapping { get; private set; } = new List<(string id, Color color)>();

    public override VisualizationType VisualizationType => VisualizationType.HeatMap;


    public override void ShowVisualization<T>(IEnumerable<T> a_spatialLoggingDataPoints, LoggingDataPointType a_loggingDataPointType, Dictionary<FilterCategory, List<string>> a_filterSelections)
    {
        if (MeshesToOverlay == null || MeshesToOverlay.Count == 0)
            FindMeshesToOverlay();

        foreach (var meshToOverlay in MeshesToOverlay)
        {
            var gameObject = Instantiate(Prefab, transform);
            var heatMapVisual = gameObject.GetComponent<HeatMapVisual>();
            heatMapVisual.ShowHeatmap(meshToOverlay, a_spatialLoggingDataPoints);
            m_heatMaps.Add(heatMapVisual);

           
            List<(string id, Color color)> filterUniqueColors = CreateColorMappingFromTexture(gameObject, heatMapVisual);

            ColorMapping = filterUniqueColors;
        }
    }

    public override void UpdateVisualization()
    {
        foreach (var heatmap in m_heatMaps)
        {
            var renderer = heatmap.GetComponent<Renderer>();
            var color = renderer.material.color;
            renderer.material.color = new Color(color.r, color.g, color.b, Alpha);
        }
    }

    public override void ClearVisualization()
    {
        foreach (var heatMap in m_heatMaps)
        {
            Destroy(heatMap.gameObject);
        }
        m_heatMaps.Clear();
    }

    public Dictionary<string, Color> GetReducedColorMapping(int a_nOfColorMappingEntries)
    {
        var d = a_nOfColorMappingEntries - 1;
        var count = ColorMapping.Count;
        var filterNOfColors = new List<(string id, Color color)>();
        if (count > d)
        {
            var n = count / (float)d;
            var sum = 0f;
            for (int i = 0; i < count; i++)
            {
                var (id, color) = ColorMapping[i];
                if (i == Math.Floor(sum) || i + 1 == count)
                {
                    sum += n;
                    filterNOfColors.Add((id, color));
                }
            }
        }
        else
        {
            Debug.LogWarningFormat("Cannot reduce ColorMapping with {0} entries to {1} entries. Returning unreduced ColorMapping", count, a_nOfColorMappingEntries);
            filterNOfColors = ColorMapping;
        }

        return filterNOfColors.ToDictionary(colorMapping => colorMapping.id, colorMapping => colorMapping.color);
    }

    private void FindMeshesToOverlay()
    {
        foreach (var teleportationArea in FindObjectsOfType<TeleportationArea>())
        {
            if (teleportationArea.TryGetComponent<MeshRenderer>(out var meshRenderer) && !MeshesToOverlay.Contains(meshRenderer))
            {
                MeshesToOverlay.Add(meshRenderer);
            }
        }

        foreach (var teleportationAnchor in FindObjectsOfType<TeleportationAnchor>())
        {
            if (teleportationAnchor.TryGetComponent<MeshRenderer>(out var meshRenderer) && !MeshesToOverlay.Contains(meshRenderer))
            {
                MeshesToOverlay.Add(meshRenderer);
            }
        }
    }

    /// <summary>
    /// TODO problem when uv on edge e.g. 64 HeatmapMaxValue 1/64 green with stripes
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="heatMapVisual"></param>
    /// <returns></returns>
    private List<(string id, Color color)> CreateColorMappingFromTexture(GameObject gameObject, HeatMapVisual heatMapVisual)
    {
        var width = gameObject.GetComponent<MeshRenderer>().material.mainTexture.width;
        var pixelColors = GetPixelColors(gameObject.GetComponent<MeshRenderer>());

        //create the mapping between colors and intervalls
        var colorMapping = new List<(double startValue, double stopValue, Color color)>();
        var prevValue = 0f;
        Color color = default;
        for (int i = 0; i < width; i++)
        {
            var normalized = i / (float)width;
            var value = heatMapVisual.HeatmapMaxValue * normalized;
            if ((i != 0 && Math.Floor(value) != Math.Floor(prevValue)) || (prevValue == 0 && value != 0))
            {
                var startValue = Math.Ceiling(prevValue);
                if (prevValue == 0)
                    startValue = prevValue;
                colorMapping.Add((startValue, Math.Floor(value), color));
            }
            prevValue = value;
            color = pixelColors[i];
        }
        //Add the last color at the end with value for the case i == width.
        //In this case the uv coordinate is at the end of the texture.
        //Since the texture should be set to wrap mode clamp the pixel color at that
        //spot should be equal to the last color.
        colorMapping.Add((Math.Ceiling(prevValue), heatMapVisual.HeatmapMaxValue, color));

        //filter out duplicate colors and set the color id to a range if there are duplicates
        Color prevColor = colorMapping[0].color;
        double prevStartValue = colorMapping[0].startValue;
        double prevStopValue = colorMapping[0].stopValue;
        var filterUniqueColors = new List<(string id, Color color)>();
        string stringId;
        for (int i = 1; i < colorMapping.Count; i++)
        {
            var colorKvp = colorMapping[i];
            var startValue = colorKvp.startValue;
            var stopValue = colorKvp.stopValue;
            color = colorKvp.color;
            if (!prevColor.Equals(color))
            {
                stringId = string.Format("{0}-{1}", prevStartValue, prevStopValue);
                if (prevStartValue.Equals(prevStopValue))
                    stringId = prevStartValue.ToString();

                filterUniqueColors.Add((stringId, prevColor));

                prevColor = color;
                prevStartValue = startValue;
                prevStopValue = stopValue;
            }
            else
            {
                prevStopValue = stopValue;
            }
        }
        stringId = string.Format("{0}-{1}", prevStartValue, heatMapVisual.HeatmapMaxValue);
        if (prevStartValue.Equals(heatMapVisual.HeatmapMaxValue))
            stringId = heatMapVisual.HeatmapMaxValue.ToString();

        filterUniqueColors.Add((stringId, prevColor));

        return filterUniqueColors;
    }

    /// <summary>
    /// https://answers.unity.com/questions/1271693/reading-pixel-data-from-materialmaintexture-return.html
    /// </summary>
    /// <param name="a_renderer"></param>
    /// <returns></returns>
    private Color[] GetPixelColors(MeshRenderer a_renderer)
    {
        Texture mainTexture = a_renderer.material.mainTexture;
        Texture2D texture2D = new Texture2D(mainTexture.width, mainTexture.height, TextureFormat.RGBA32, false);

        RenderTexture currentRT = RenderTexture.active;

        RenderTexture renderTexture = new RenderTexture(mainTexture.width, mainTexture.height, 32);
        Graphics.Blit(mainTexture, renderTexture);

        RenderTexture.active = renderTexture;
        texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture2D.Apply();

        Color[] pixels = texture2D.GetPixels();

        RenderTexture.active = currentRT;
        renderTexture.Release();

        return pixels;
    }
}
