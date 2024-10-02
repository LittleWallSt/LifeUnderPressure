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
        TutorialUtility.Instance.beaconZone.TurnOnPing(true);
        GameObject newGo = new GameObject();
        newGo.transform.position = QuestSystem.GetQuestLocation().position;
        if (TutorialUtility.Instance.beaconZone!=null) TutorialUtility.Instance.beaconZone.setPingTransform(newGo.transform);
    }
    private void QuestFinished()
    {
        CompleteStep();
        Debug.Log("gjhjhhj");
        QuestSystem.Remove_OnQuestFinished(QuestFinished);
    }
}
