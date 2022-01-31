using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

public class StudyDefiner : MonoBehaviour
{
    [SerializeField]
    private StudyManager m_studyManager;

    [SerializeField]
    private StudyDefinition m_selectedStudyDefinition;

    public List<string> StudyNames { get; set; } = new List<string>();

    public int StudyNamesIdx { get; set; } = 0;

    public StudyManager StudyManager { get => m_studyManager; set => m_studyManager = value; }

    public StudyDefinition SelectedStudyDefinition { get => m_selectedStudyDefinition; set => m_selectedStudyDefinition = value; }

    // Start is called before the first frame update
    void Start()
    {
    }

    public bool TryFillStudyNamesList(out string a_loadFailedMessage)
    {
        a_loadFailedMessage = "";
        if (StudyManager.TryLoadStudies())
        {
            var studyNames = StudyManager.Studies.Select(study => study.studyName);
            StudyNames = studyNames.ToList();
            UpdateSelectionIndex(StudyManager.Studies[0]);
            return true;
        }

        a_loadFailedMessage = "Could not find any studies to load.";
        return false;
    }

    public bool TryCreateStudy(out string a_creationFailedMessage)
    {
        a_creationFailedMessage = "";
        var newStudyDefinition = new StudyDefinition();

        if (TextFileReader.StudyDefinitionExists(newStudyDefinition))
        {
            a_creationFailedMessage = $"Study with name {newStudyDefinition.studyName} already exists.";
            Debug.LogWarning(a_creationFailedMessage);
            return false;
        }

        TextFileWriter.WriteToStudyDefinition(newStudyDefinition, true, true);
        TryFillStudyNamesList(out _);

        UpdateSelectionIndex(newStudyDefinition);
        return true;
    }

    public bool TrySaveStudyEdit(out string a_saveFailedMessage)
    {
        a_saveFailedMessage = "";
        var editedStudyDefinition = SelectedStudyDefinition;
        if (!TextFileReader.TryReadAllStudyDefinitions(out var studyDefinitions))
        {
            a_saveFailedMessage = $"Could not find any studies.";
            return false;
        }

        if (editedStudyDefinition.isActive)
        {
            TryChangeActiveStudy(studyDefinitions);
        }

        if (TextFileReader.TryReadStudyDefinitionWithId(editedStudyDefinition.id, out var oldStudyDefinition))
        {
            if (!TextFileWriter.TryEditStudyDefinition(oldStudyDefinition, editedStudyDefinition))
            {
                a_saveFailedMessage = $"Could not edit directory name from {oldStudyDefinition.studyName} to {editedStudyDefinition.studyName}. Study name may already exist.";
                return false;
            }
        }
        else
        {
            a_saveFailedMessage = $"Could not find study definition with id {editedStudyDefinition.id}.";
            return false;
        }
        TryFillStudyNamesList(out _);

        UpdateSelectionIndex(editedStudyDefinition);

        return true;
    }

    private void TryChangeActiveStudy(List<StudyDefinition> a_studyDefinitions)
    {
        foreach (var study in a_studyDefinitions)
        {
            if (!SelectedStudyDefinition.id.Equals(study.id) && study.isActive)
            {
                study.isActive = false;
                TextFileWriter.WriteToStudyDefinition(study, true, true);
                Debug.LogWarning(string.Format("Active Study changed from {0} to {1}.", study.studyName, SelectedStudyDefinition.studyName));
            }
        }
    }

    private void UpdateSelectionIndex(StudyDefinition a_newStudyDefinition)
    {
        SelectedStudyDefinition = a_newStudyDefinition;
        for (int i = 0; i < StudyNames.Count; i++)
        {
            if (StudyNames[i] == SelectedStudyDefinition.studyName)
            {
                StudyNamesIdx = i;
                break;
            }
        }
    }

    void OnApplicationQuit()
    {
        //TextFileWriter.WriteToStudyDefinitions(m_studies);
    }

}
