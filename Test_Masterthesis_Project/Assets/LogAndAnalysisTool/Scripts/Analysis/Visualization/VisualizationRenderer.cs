using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VisualizationRenderer : MonoBehaviour
{
    [SerializeField]
    protected bool m_debugMode;

    [SerializeField]
    private GameObject m_prefab;

    [SerializeField, Range(0.1f, 1f)]
    private float m_alpha = 0.3f;

    public float Alpha
    {
        get => m_alpha;
        set
        {
            var oldAlpha = m_alpha;
            m_alpha = value;

            if (oldAlpha != m_alpha)
                UpdateVisualization();
        }
    }

    public GameObject Prefab { get => m_prefab; set => m_prefab = value; }

    public abstract VisualizationType VisualizationType { get; }

    void OnValidate()
    {
        UpdateVisualization();
    }

    public abstract void ClearVisualization();

    public abstract void UpdateVisualization();

    public void HideVisualization()
    {
        gameObject.SetActive(false);
    }

    protected bool TryReactivateVisualization()
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            Debug.LogFormat("Reactivate visualization {0}.", VisualizationType);
            return true;
        }
        return false;
    }

    protected void ShowLabels(GameObject a_renderedObject, char a_labelLetter, Color a_color = default)
    {
        foreach (var textMesh in a_renderedObject.GetComponentsInChildren<TextMesh>())
        {
            if (textMesh.gameObject.CompareTag("Label"))
            {
                textMesh.text = a_labelLetter.ToString();
                textMesh.gameObject.GetComponent<MeshRenderer>().enabled = true;
                if (a_color != default)
                    textMesh.color = a_color;

            }
        }
    }

    protected void MoveLabels(GameObject a_renderedObject, Vector3 a_position)
    {
        foreach (var textMesh in a_renderedObject.GetComponentsInChildren<TextMesh>())
        {
            if (textMesh.gameObject.CompareTag("Label"))
            {
                textMesh.transform.position = a_position;
            }
        }
    }

    protected string GetRenderedEntityName(string a_id)
    {
        return Prefab.name + " " + a_id;
    }

    protected char GetLableLetter(LoggingDataPointType a_loggingDataPointType)
    {
        char lableLetter = default;
        switch (a_loggingDataPointType)
        {
            case LoggingDataPointType.ParticipantTrackingData:
                lableLetter = 'P';
                break;
            case LoggingDataPointType.SpatialAudioEvent:
            case LoggingDataPointType.OfflineSpatialAudioEvent:
                lableLetter = 'A';
                break;
            case LoggingDataPointType.InputActionEvent:
                lableLetter = 'I';
                break;
            case LoggingDataPointType.CustomSpatialEvent:
                lableLetter = 'C';
                break;
            case LoggingDataPointType.TransformTrackingData:
                lableLetter = 'T';
                break;
            default:
                break;
        }

        return lableLetter;
    }
}
