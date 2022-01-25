using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class HapticInteractionManager : MonoBehaviour
{
    [SerializeField]
    private XRRayInteractor m_xrRayInteractor;

    [SerializeField]
    private InputActionReference m_select;

    [SerializeField]
    private MenuManager m_menuManager;

    // Start is called before the first frame update
    void Start()
    {
        m_select.action.performed += OnSelect;
    }

    private void OnSelect(InputAction.CallbackContext obj)
    {
        if (m_xrRayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            if (hit.transform.TryGetComponent<VisualDataPoint>(out var visualDataPoint))
            {
                try
                {
                    Debug.Log("Hit VisualDataPoint.");
                    m_menuManager.ShowSelection(visualDataPoint);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
            else if (hit.transform.TryGetComponent<HeatMapVisual>(out var heatMapVisual))
            {
                var gridObject = heatMapVisual.GetGridObject(hit.point);
            }

        }
    }

}
