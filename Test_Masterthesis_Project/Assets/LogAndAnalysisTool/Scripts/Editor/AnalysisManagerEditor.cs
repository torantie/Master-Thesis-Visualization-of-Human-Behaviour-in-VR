using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AnalysisManager))]
public class AnalysisManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var analysisManager = (AnalysisManager)target;

        if (GUILayout.Button("Calculate Offline Spatial Audio Events."))
        {
            analysisManager.MapPositionToAudioEvent(true);
        }
    }
}
