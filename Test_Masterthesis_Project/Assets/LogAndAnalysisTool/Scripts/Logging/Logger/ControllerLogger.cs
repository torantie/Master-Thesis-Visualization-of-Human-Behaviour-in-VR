using UnityEngine;

public abstract class ControllerLogger : ParticipantLogger
{
    [SerializeField]
    protected Transform m_controllerTransform;

    [SerializeField]
    protected LoggingManager m_loggingManager;


    public abstract string ControllerName
    {
        get;
    }

    void Start()
    {
        if (m_controllerTransform == null)
            Debug.LogErrorFormat("Assign controller Transform for controller {0}.", ControllerName);
    }

    protected (Vector3 position, Quaternion rotation) GetControllerData(UnityEngine.XR.InputDevice a_hand)
    {
        var allSuccess = false;
        Vector3 position = default;
        Quaternion rotation = default;

        if (a_hand.isValid)
        {
            allSuccess =
                a_hand.TryGetFeatureValue(UnityEngine.XR.CommonUsages.devicePosition, out position)
                && a_hand.TryGetFeatureValue(UnityEngine.XR.CommonUsages.deviceRotation, out rotation);

        }

#if UNITY_EDITOR
        if (!allSuccess)
            Debug.Log("Could not aquire all Controller Data Points for Controller: " + a_hand.name);

#endif
        return (position, rotation);
    }

}
