using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ParticipantLogger : MonoBehaviour
{
    public abstract void FillDataPoint(ParticipantTrackingDataPoint a_participantDataPoint);
}
