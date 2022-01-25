using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TransformLogger))]
public class TransformLoggerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        TransformLogger transformLogger = (TransformLogger)target;
        if (GUILayout.Button("Reload Currently Trackable Transforms"))
        {
            transformLogger.FindTrackableTransforms();
        }

        if (transformLogger.TryGetDuplicateWarning(out var warningText))
        {
            EditorGUILayout.HelpBox(warningText, MessageType.Warning);
        }

    }
}
