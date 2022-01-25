using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TrackableTransform : MonoBehaviour
{
    [SerializeField]
    private string m_transformId;

    public string TransformId { get => m_transformId; private set => m_transformId = value; }

}
