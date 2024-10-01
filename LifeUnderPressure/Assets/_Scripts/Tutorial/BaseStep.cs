using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseStep : TutorialStep
{
    [SerializeField] Quest quest;

    public override void CheckForCompleting()
    {
       
    }

    public override void StartStep()
    {
        QuestSystem.AssignQuest(quest);
        QuestSystem.Assign_OnQuestFinished(QuestFinished);
    }
    private void QuestFinished()
    {
        CompleteStep();
        Debug.Log("gjhjhhj");
        QuestSystem.Remove_OnQuestFinished(QuestFinished);
    }
}
