using UnityEditor;
using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;
using System.Linq;

[CustomEditor(typeof(StudyDefiner))]
public class StudyDefinerEditor : Editor
{
    private string m_helpBoxMessage;

    private MessageType m_helpBoxMessageType;

    private bool m_showHelpBox;


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        StudyDefiner studyDefiner = (StudyDefiner)target;

        ShowDropDown(studyDefiner);
        ShowButtons(studyDefiner);

        if (m_showHelpBox)
        {
            EditorGUILayout.HelpBox(m_helpBoxMessage, m_helpBoxMessageType);
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(studyDefiner);
            EditorUtility.SetDirty(studyDefiner.StudyManager);
        }
    }

    private void ShowDropDown(StudyDefiner a_studyDefiner)
    {
        GUIContent list = new GUIContent("Selected Study");

        var newIndex = EditorGUILayout.Popup(list, a_studyDefiner.StudyNamesIdx, a_studyDefiner.StudyNames.ToArray());
        if (a_studyDefiner.StudyNamesIdx != newIndex)
        {
            a_studyDefiner.StudyNamesIdx = newIndex;

            if (a_studyDefiner.StudyManager != null
                && a_studyDefiner.StudyManager.Studies != null)
            {
                if (a_studyDefiner.StudyManager.Studies.Count != 0)
                {
                    //var newSelection = a_studyDefiner.StudyManager.Studies.FirstOrDefault((study) => study.studyName == a_studyDefiner.StudyNames[a_studyDefiner.StudyNamesIdx]);
                    //if (newSelection != default
                    //    && !newSelection.Equals(a_studyDefiner.SelectedStudyDefinition))
                    //    a_studyDefiner.SelectedStudyDefinition = newSelection;
                    a_studyDefiner.SelectedStudyDefinition = a_studyDefiner.StudyManager.Studies[a_studyDefiner.StudyNamesIdx];
                }
                else
                {
                    a_studyDefiner.StudyNames.Clear();
                }
            }
        }

    }

    private void ShowButtons(StudyDefiner a_studyDefiner)
    {
        LoadStudyButton(a_studyDefiner);
        SaveEditStudyButton(a_studyDefiner);
        CreateStudyButton(a_studyDefiner);
    }

    private void LoadStudyButton(StudyDefiner a_studyDefiner)
    {
        if (GUILayout.Button("Load Studies"))
        {
            if (!a_studyDefiner.TryFillStudyNamesList(out var loadFailedMessage))
            {
                ActivateHelpBox(loadFailedMessage, MessageType.Warning);
            }
            else
            {
                m_showHelpBox = false;
            }

        }
    }

    private void SaveEditStudyButton(StudyDefiner a_studyDefiner)
    {
        if (GUILayout.Button("Save Edit Study"))
        {
            if (!a_studyDefiner.TrySaveStudyEdit(out var saveFailedMessage))
            {
                ActivateHelpBox(saveFailedMessage, MessageType.Warning);
            }
            else
            {
                m_showHelpBox = false;
            }
        }
    }

    private void CreateStudyButton(StudyDefiner a_studyDefiner)
    {
        if (GUILayout.Button("Create Study"))
        {
            if (!a_studyDefiner.TryCreateStudy(out var creationFailedMessage))
            {
                ActivateHelpBox(creationFailedMessage, MessageType.Warning);
            }
            else
            {
                m_showHelpBox = false;
            }
        }
    }

    private void ActivateHelpBox(string a_message, MessageType a_messageType)
    {
        m_helpBoxMessage = a_message;
        m_helpBoxMessageType = a_messageType;
        m_showHelpBox = true;
    }
}