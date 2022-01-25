using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ControllerLogger), true)]
public class ControllerLoggerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        //ControllerLogger controllerLogger = (ControllerLogger)target;

        //todo
        /*if (GUILayout.Button("Load Actions"))
        {
            controllerLogger.FillActionList();
        }*/

    }
}
