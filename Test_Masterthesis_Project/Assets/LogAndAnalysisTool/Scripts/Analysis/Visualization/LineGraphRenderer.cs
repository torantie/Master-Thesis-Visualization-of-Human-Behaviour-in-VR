using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LineGraphRenderer : GroupedVisualizationRenderer
{
    [SerializeField, Range(0.1f, 1f)]
    private float m_lineWidth;

    private readonly Dictionary<string, LineRenderer> m_lines = new Dictionary<string, LineRenderer>();


    public float LineWidth
    {
        get => m_lineWidth;
        set
        {
            var lineWidth = m_lineWidth;
            m_lineWidth = value;

            if (lineWidth != m_lineWidth)
                UpdateVisualization();
        }
    }

    public override VisualizationType VisualizationType => VisualizationType.LineGraph;


    public override void ShowVisualization<T>(Dictionary<string, List<T>> a_spatialLoggingDataPoints, LoggingDataPointType a_loggingDataPointType)
    {
        char lableLetter = GetLableLetter(a_loggingDataPointType);
        foreach (var line in a_spatialLoggingDataPoints)
        {
            ShowPath(line.Key, line.Value.Select(dataPoint => dataPoint.position.StringToVector3()), lableLetter);
        }
    }

    public void ShowVisualizationDataPoint(Dictionary<string, List<ParticipantTrackingDataPoint>> a_pointCollections, char a_labelLetter = default)
    {
        ClearAllPaths();

        foreach (var line in a_pointCollections)
        {
            ShowPath(line.Key, line.Value.Select(continuousDataPoint => continuousDataPoint.position.StringToVector3()).ToList(), a_labelLetter);
        }
    }

    public override void ClearVisualization()
    {
        ClearAllPaths();
    }

    public override void UpdateVisualization()
    {
        Debug.Log("UpdateVisualization m_lines.Count: " + m_lines.Count);

        foreach (var keyValue in m_lines)
        {
            var line = keyValue.Value;
            line.startWidth = LineWidth;
            line.endWidth = LineWidth;

            var color = line.material.color;
            line.material.color = new Color(color.r, color.g, color.b, Alpha);

        }
    }

    private void ShowPath(string a_id, IEnumerable<Vector3> a_points, char a_labelLetter = default)
    {
        if (!TryCreateLine(a_id, out LineRenderer line))
            return;
        line.positionCount = a_points.Count();
        var pointsArray = a_points.ToArray();
        line.SetPositions(pointsArray);
        //Todo maybe to computational expensive maybe use label as interaction point
        //CreateLineCollider(line);

        m_lines[a_id] = line;

        if (a_labelLetter != default)
        {
            ShowLabels(line.gameObject, a_labelLetter);
            MoveLabels(line.gameObject, pointsArray[0]);
        }

        Debug.Log(string.Format("Created Line number of points: {0}.", line.positionCount));
    }

    private static void CreateLineCollider(LineRenderer line)
    {
        //https://stackoverflow.com/questions/61216229/making-line-drawn-with-linerender-clickable
        Mesh lineBakedMesh = new Mesh(); //Create a new Mesh (Empty at the moment)
        line.BakeMesh(lineBakedMesh, true); //Bake the line mesh to our mesh variable
        line.gameObject.AddComponent<MeshCollider>();
        line.GetComponent<MeshCollider>().sharedMesh = lineBakedMesh; //Set the baked mesh to the MeshCollider
    }

    private bool TryCreateLine(string a_id, out LineRenderer a_line)
    {
        var lineGameObject = Instantiate(Prefab, new Vector3(0, 0, 0), Quaternion.identity, transform);
        lineGameObject.name = GetRenderedEntityName(a_id);

        var color = ColorMapping[a_id];
        color.a = Alpha;

        if (lineGameObject.TryGetComponent<LineRenderer>(out var line))
        {
            line.material.color = color;
            line.startWidth = LineWidth;
            line.endWidth = LineWidth;
            a_line = line;
            Debug.Log(string.Format("Created Line color: {0}", color));
            return true;
        }
        else
        {
            a_line = null;
            Debug.LogError("Could not create line. Component LineRenderer is missing.");
            return false;
        }

    }

    private void ClearAllPaths()
    {
        foreach (var line in m_lines)
        {
            Destroy(line.Value.gameObject);
        }
        m_lines.Clear();
    }
}
