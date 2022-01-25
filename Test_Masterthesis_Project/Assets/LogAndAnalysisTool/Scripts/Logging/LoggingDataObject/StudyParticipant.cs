using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StudyParticipant
{
    public string id;

    public string hmd;

    public string GetName()
    {
        return GetNamePrefix() + id;
    }

    public static string GetNamePrefix()
    {
        return "Particpant_";
    }
}
