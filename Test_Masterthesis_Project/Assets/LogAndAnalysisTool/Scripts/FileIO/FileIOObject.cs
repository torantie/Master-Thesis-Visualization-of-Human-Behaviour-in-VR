using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FileIOObject
{
    public StudyDefinition StudyDefinition { get; }

    public StudySession StudySession { get; }

    public StudyParticipant StudyParticipant { get; }

    
    public FileIOObject(StudyDefinition a_studyDefinition, StudySession a_studySession, StudyParticipant a_studyParticipant)
    {
        StudyDefinition = a_studyDefinition;
        StudySession = a_studySession;
        StudyParticipant = a_studyParticipant;
    }

    public FileIOObject(StudyManager a_studyManager)
    {
        StudyDefinition = a_studyManager.ActiveStudy;
        StudySession = a_studyManager.CurrentStudySession;
        StudyParticipant = a_studyManager.LocalStudyParticipant;
    }
}
