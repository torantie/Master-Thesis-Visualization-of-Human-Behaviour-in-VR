using UnityEngine;
using UnityEngine.XR;

public class GazeLogger : ParticipantLogger
{
    public override void FillDataPoint(ParticipantTrackingDataPoint a_continuousDataPoint)
    {
        //TODO
    }

    bool TryGetEyes(out Eyes eyes)
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(XRNode.CenterEye);
        if (device.isValid)
        {
            Debug.Log(string.Format("name:{0}, manufacturer:{2}, characteristics:{3}, serialNumber:{4},",
                device.name, device.manufacturer, device.characteristics, device.serialNumber));
            if (device.TryGetFeatureValue(CommonUsages.eyesData, out eyes))
                return true;
        }

        eyes = default;
        return false;
    }

}
