using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class RightControllerLogger : ControllerLogger
{
    public override string ControllerName => "Right";

    public override void FillDataPoint(ParticipantTrackingDataPoint a_continuousDataPoint)
    {
        InputDevice right = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        (Vector3 position, Quaternion rotation) = GetControllerData(right);
        a_continuousDataPoint.rightControllerPosition = position.ToString();
        a_continuousDataPoint.rightControllerRotation = rotation.ToString();

        if (m_controllerTransform != null)
        {
            a_continuousDataPoint.rightControllerTransformPosition = m_controllerTransform.position.ToString();
            a_continuousDataPoint.rightControllerTransformRotation = m_controllerTransform.rotation.ToString();
        }
    }
}
