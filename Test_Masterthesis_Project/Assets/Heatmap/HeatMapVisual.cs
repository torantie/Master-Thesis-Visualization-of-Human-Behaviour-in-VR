/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Edited
/// </summary>
/// <typeparam name="TGridObject"></typeparam>
public class HeatMapVisual : MonoBehaviour
{
    [SerializeField]
    private float m_cellSize = 0.5f;

    private GridXZ<GridObject> m_grid;

    private Mesh m_mesh;

    private MeshFilter m_meshFilter;

    private bool m_updateMesh;

    private int m_heatmapMaxValue;

    public float CellSize { get => m_cellSize; set => m_cellSize = value; }

    public int HeatmapMaxValue { get => m_heatmapMaxValue; private set => m_heatmapMaxValue = value; }


    private void Awake()
    {
        m_mesh = new Mesh();
        m_meshFilter = GetComponent<MeshFilter>();
        m_meshFilter.mesh = m_mesh;
        m_mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
    }

    private void LateUpdate()
    {
        if (m_updateMesh)
        {
            m_updateMesh = false;
            UpdateHeatMapVisual();
        }
    }

    public void ShowHeatmap<T>(MeshRenderer a_meshToOverlay, IEnumerable<T> a_spatialLoggingDataPoints) where T : SpatialLoggingDataPoint
    {
        if (m_grid == null)
        {
            CreateGrid(a_meshToOverlay);
        }

        FillGrid(a_spatialLoggingDataPoints);

        UpdateHeatMapVisual();
    }

    public GridObject GetGridObject(Vector3 a_vector3)
    {
        return m_grid.GetGridObject(a_vector3);
    }

    private void FillGrid<T>(IEnumerable<T> a_spatialLoggingDataPoints) where T : SpatialLoggingDataPoint
    {
        var currentMax = 0;
        foreach (var spatialLoggingDataPoint in a_spatialLoggingDataPoints)
        {
            var position = spatialLoggingDataPoint.position.StringToVector3();
            var gridObject = m_grid.GetGridObject(position);
            if (gridObject != default)
            {
                gridObject.SpatialLoggingDataPoints.Add(spatialLoggingDataPoint);
                currentMax = Mathf.Max(currentMax, gridObject.Value);
            }
        }
        HeatmapMaxValue = currentMax;

    }

    private void CreateGrid(MeshRenderer a_meshToOverlay)
    {
        var meshToOverlayCenter = a_meshToOverlay.bounds.center;
        var size = a_meshToOverlay.bounds.size;
        var cellSize = 0.5f;
        var x = (int)(size.x / cellSize);
        var z = (int)(size.z / cellSize);
        var originPosition = a_meshToOverlay.transform.position - new Vector3((size.x / 2), 0, (size.z / 2)) + new Vector3(0, 0.1f, 0);

        var grid = new GridXZ<GridObject>(x, z, cellSize, originPosition, (grid, x, y) => new GridObject());
        SetGrid(grid);
    }

    private void SetGrid(GridXZ<GridObject> a_grid)
    {
        m_grid = a_grid;
        UpdateHeatMapVisual();

        a_grid.OnGridObjectChanged += Grid_OnGridObjectChanged;
    }

    private void Grid_OnGridObjectChanged(object a_sender, GridXZ<GridObject>.OnGridObjectChangedEventArgs a_e)
    {
        m_updateMesh = true;
    }

    private void UpdateHeatMapVisual()
    {
        MeshUtils.CreateEmptyMeshArrays(m_grid.GetWidth() * m_grid.GetHeight(), out Vector3[] vertices, out Vector2[] uv, out int[] triangles);

        for (int x = 0; x < m_grid.GetWidth(); x++)
        {
            for (int z = 0; z < m_grid.GetHeight(); z++)
            {
                int index = x * m_grid.GetHeight() + z;
                Vector3 quadSize = new Vector3(1, 0, 1) * m_grid.GetCellSize();


                var gridObject = m_grid.GetGridObject(x, z);
                float gridValueNormalized = (float)gridObject.Value / HeatmapMaxValue;
                Vector2 gridValueUV = new Vector2(gridValueNormalized, 0f);
                var position = m_grid.GetWorldPosition(x, z) + quadSize * .5f;
                MeshUtils.AddToMeshArraysXZ(vertices, uv, triangles, index, position, 0f, quadSize, gridValueUV, gridValueUV);
            }
        }
        m_mesh.vertices = vertices;
        m_mesh.uv = uv;
        m_mesh.triangles = triangles;
    }

}

public class GridObject
{
    public GridObject()
    {
    }

    public GridObject(List<SpatialLoggingDataPoint> a_spatialLoggingDataPoints)
    {
        SpatialLoggingDataPoints = a_spatialLoggingDataPoints;
    }

    public int Value { get { return SpatialLoggingDataPoints.Count; } }

    public List<SpatialLoggingDataPoint> SpatialLoggingDataPoints { get; private set; } = new List<SpatialLoggingDataPoint>();

    public override string ToString()
    {
        return Value.ToString();
    }
}
