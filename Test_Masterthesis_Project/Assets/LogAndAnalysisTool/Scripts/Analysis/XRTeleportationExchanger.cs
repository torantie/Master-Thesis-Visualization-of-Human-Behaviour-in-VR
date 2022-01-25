using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRTeleportationExchanger : MonoBehaviour
{
    [SerializeField]
    private XRInteractionManager m_xrInteractionManager;

    [SerializeField]
    private TeleportationProvider m_teleportationProvider;

    // Start is called before the first frame update
    void Start()
    {
        foreach (var teleportationArea in FindObjectsOfType<TeleportationArea>())
        {
            teleportationArea.teleportationProvider = m_teleportationProvider;
            teleportationArea.interactionManager = m_xrInteractionManager;
        }

        foreach (var teleportationAnchor in FindObjectsOfType<TeleportationAnchor>())
        {
            teleportationAnchor.teleportationProvider = m_teleportationProvider;
            teleportationAnchor.interactionManager = m_xrInteractionManager;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
