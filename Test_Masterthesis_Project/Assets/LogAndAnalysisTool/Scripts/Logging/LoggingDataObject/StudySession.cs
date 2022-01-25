using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StudySession
{
    public string id;

    public List<StudyParticipant> studyParticipants = new List<StudyParticipant>();

    public string GetName()
    {
        return GetNamePrefix() + id;
    }

    public static string GetNamePrefix()
    {
        return "Study_Session_";
    }

}
