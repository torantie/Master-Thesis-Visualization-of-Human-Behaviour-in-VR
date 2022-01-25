using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EventLogger : MonoBehaviour
{
    public abstract void StartListening();

    public abstract void StopListening();
}
