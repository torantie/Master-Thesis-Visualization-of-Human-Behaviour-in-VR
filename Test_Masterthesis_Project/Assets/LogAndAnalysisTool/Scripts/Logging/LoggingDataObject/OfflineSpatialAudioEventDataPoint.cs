using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfflineSpatialAudioEventDataPoint : SpatialAudioEventDataPoint
{
    public override LoggingDataPointType Type => LoggingDataPointType.OfflineSpatialAudioEvent;

    public double startOffsetInMs;

    public double endOffsetInMs;
}
