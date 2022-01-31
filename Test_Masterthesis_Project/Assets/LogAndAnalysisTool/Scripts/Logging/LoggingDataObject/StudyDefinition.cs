using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StudyDefinition
{
    [ReadOnly]
    public string id;

    public string studyName;

    public bool isCollaborative;

    public bool isActive;

    [HideInInspector]
    public List<StudySession> studySessions = new List<StudySession>();

    public StudyDefinition(string a_studyName = "Default Study Name", bool a_isCollaborative = false, bool a_isActive = false)
    {
        studyName = a_studyName;
        isCollaborative = a_isCollaborative;
        isActive = a_isActive;
        id = Guid.NewGuid().ToString();
    }

    public void AddStudySession(StudySession a_studySession)
    {
        studySessions.Add(a_studySession);
    }

}


