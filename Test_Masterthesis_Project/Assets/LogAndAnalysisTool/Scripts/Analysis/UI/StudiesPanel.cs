using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StudiesPanel : MenuPanel
{
    [SerializeField]
    private StudyDefiner m_studyDefiner;

    [SerializeField]
    private TMP_Dropdown m_studieSelectDropdown;

    // Start is called before the first frame update
    void Start()
    {
        FillStudiesSelection();
    }

    public void FillStudiesSelection()
    {
        m_studieSelectDropdown.ClearOptions();

        if (m_studyDefiner.StudyManager.TryLoadStudies())
        {
            foreach (var study in m_studyDefiner.StudyManager.Studies)
            {
                m_studieSelectDropdown.options.Add(new TMP_Dropdown.OptionData(study.studyName));
            }
            var activeStudyName = m_studyDefiner.StudyManager.ActiveStudy.studyName;
            m_studieSelectDropdown.value = m_studieSelectDropdown.options.FindIndex(option => option.text == activeStudyName);
        }
        else
        {
            m_messageBox.text = "Could not load studies.";
        }
    }

    public void OnStudyLoad()
    {
        FillStudiesSelection();
    }

    public void OnSetActive()
    {
        var selected = m_studieSelectDropdown.options[m_studieSelectDropdown.value].text;

        m_messageBox.text = $"Trying to load option {selected}.";
        var selectedStudy = m_studyDefiner.StudyManager.Studies.FirstOrDefault((study) => study.studyName.Equals(selected, StringComparison.InvariantCultureIgnoreCase));
        var message = $"{Environment.NewLine}";
        if (selectedStudy == default)
        {
            message = $"Could not find selected study.";
        }
        else
        {
            selectedStudy.isActive = true;
            if (!m_studyDefiner.TrySaveStudyEdit(out var saveFailedMessage))
            {
                message = $"Save failed message: {saveFailedMessage}.";
            }
            else
            {
                message = $"Set study to active. Studydefinition:{Environment.NewLine}{JsonUtility.ToJson(selectedStudy, true)}.";
            }
        }
        m_messageBox.text += message;
    }
}
