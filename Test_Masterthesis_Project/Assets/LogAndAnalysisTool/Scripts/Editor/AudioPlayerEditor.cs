using UnityEditor;
using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;
using System.Linq;

[CustomEditor(typeof(AudioPlayer))]
public class AudioPlayerEditor : Editor
{
    public override async void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        AudioPlayer audioPlayer = (AudioPlayer)target;

        if (GUILayout.Button("Play Audio."))
        {
            await audioPlayer.Play();
        }
    }

}