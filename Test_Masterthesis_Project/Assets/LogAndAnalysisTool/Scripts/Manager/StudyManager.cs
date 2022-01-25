using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StudyManager : MonoBehaviour
{
    [SerializeField, ReadOnly]
    private StudyDefinition m_activeStudy;

    public List<StudyDefinition> Studies { get; private set; } = new List<StudyDefinition>();

    public StudyDefinition ActiveStudy { get => m_activeStudy; private set => m_activeStudy = value; }

    public StudySession CurrentStudySession { get; private set; }

    public StudyParticipant LocalStudyParticipant { get; private set; }

    public string TaskId { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        TryLoadStudies();
    }

    public bool TryLoadStudies()
    {
        if (TextFileReader.TryReadAllStudyDefinitions(out var studyDefinitions))
        {
            Studies.Clear();
            Studies = studyDefinitions;
            SetActiveStudyFromStudies();
            SetActiveStudiesSessions();
            return true;
        }
        else
        {
            Studies.Clear();
            return false;
        }
    }

    public void SetStudySessionInformation(LoggingStartInformation a_loggingStartInformation)
    {
        var studySession = new StudySession() { id = a_loggingStartInformation.sessionId };
        var studyParticpant = new StudyParticipant() { id = a_loggingStartInformation.participantId };
        studySession.studyParticipants.Add(studyParticpant);
        CurrentStudySession = studySession;
        LocalStudyParticipant = studyParticpant;
        TaskId = a_loggingStartInformation.taskId;
    }

    /// <summary>
    /// Set sessions of active study, by reading the directory of the active study.
    /// </summary>
    public void SetActiveStudiesSessions()
    {
        if (ActiveStudy.studySessions == null || ActiveStudy.studySessions.Count == 0)
        {
            Debug.LogWarning("No Study Sessions defined in StudyDefinition. Reading directly from directory.");
            TextFileReader.TryReadStudySessionsFromDirectory(ActiveStudy, out var studySessions);
            ActiveStudy.studySessions = studySessions;
        }
    }

    private void SetActiveStudyFromStudies()
    {
        var activeStudy = Studies.FirstOrDefault(study => study.isActive);

        if (activeStudy != default)
        {
            ActiveStudy = activeStudy;
        }
        else
        {
            Debug.Log("No Active Study found.");
        }
    }
}
