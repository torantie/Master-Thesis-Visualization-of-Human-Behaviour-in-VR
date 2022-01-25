using UnityEngine;

public abstract class ContinuousLogger<T> : MonoBehaviour
{
    public abstract void FillDataPoint(T a_continuousDataPoint);
}
