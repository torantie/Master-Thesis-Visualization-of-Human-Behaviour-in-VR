using UnityEngine;
using UnityEngine.XR;

public class HMDLogger : ParticipantLogger
{
    [SerializeField]
    private Transform m_cameraTransform;

    void Start()
    {
        if (m_cameraTransform == null)
            Debug.LogErrorFormat("Assign camera Transform.");
    }

    public override void FillDataPoint(ParticipantTrackingDataPoint a_continuousDataPoint)
    {
        TryGetPosition(CommonUsages.centerEyePosition, XRNode.CenterEye, out var position);
        TryGetRotation(CommonUsages.centerEyeRotation, XRNode.CenterEye, out var rotation);
        a_continuousDataPoint.hmdPosition = position.ToString();
        a_continuousDataPoint.hmdRotation = rotation.ToString();
        if (m_cameraTransform != null)
        {
            a_continuousDataPoint.position = m_cameraTransform.position.ToString();
            a_continuousDataPoint.hmdTransformRotation = m_cameraTransform.rotation.ToString();

            var raydirection = new Ray(m_cameraTransform.position, m_cameraTransform.forward);
            if (Physics.Raycast(raydirection, out RaycastHit hit))
            {
                a_continuousDataPoint.focusedObjectName = hit.transform.gameObject.name;
            }
        }
    }

    public Vector3 GetTransfromPosition()
    {
        return m_cameraTransform.position;
    }

    public bool TryGetPosition(InputFeatureUsage<Vector3> inputFeatureUsage, XRNode node, out Vector3 position)
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(node);
        if (device.isValid)
        {
            if (device.TryGetFeatureValue(inputFeatureUsage, out position))
                return true;
        }
        position = default;
        return false;
    }

    public bool TryGetRotation(InputFeatureUsage<Quaternion> inputFeatureUsage, XRNode node, out Quaternion rotation)
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(node);
        if (device.isValid)
        {
            if (device.TryGetFeatureValue(inputFeatureUsage, out rotation))
                return true;
        }
        rotation = default;
        return false;
    }


}
