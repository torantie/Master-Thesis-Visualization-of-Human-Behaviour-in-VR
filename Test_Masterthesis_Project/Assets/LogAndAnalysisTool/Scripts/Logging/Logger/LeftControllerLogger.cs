using UnityEngine;
using UnityEngine.XR;

public class LeftControllerLogger : ControllerLogger
{
    public override string ControllerName => "Left";

    public override void FillDataPoint(ParticipantTrackingDataPoint a_continuousDataPoint)
    {
        InputDevice left = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        (Vector3 position, Quaternion rotation) = GetControllerData(left);
        a_continuousDataPoint.leftControllerPosition = position.ToString();
        a_continuousDataPoint.leftControllerRotation = rotation.ToString(); 
        
        if (m_controllerTransform != null)
        {
            a_continuousDataPoint.leftControllerTransformPosition = m_controllerTransform.position.ToString();
            a_continuousDataPoint.leftControllerTransformRotation = m_controllerTransform.rotation.ToString();
        }
    }
}
