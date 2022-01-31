using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRCameraDisabler : MonoBehaviour
{
    [SerializeField]
    private XRRig m_xrRigToExclude;

    [SerializeField]
    private XRInteractionManager m_xrInteractionManagerToExclude;

    // Start is called before the first frame update
    void Start()
    {
        foreach (var xrRig in FindObjectsOfType<XRRig>())
        {
         
                xrRig.gameObject.SetActive(false);
            
        }

        foreach (var xrInteractionManager in FindObjectsOfType<XRInteractionManager>())
        {
            
                xrInteractionManager.gameObject.SetActive(false);
            
        }
        m_xrRigToExclude.gameObject.SetActive(true);
        m_xrInteractionManagerToExclude.gameObject.SetActive(true);
    }
}
