using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseStep : TutorialStep
{
    [SerializeField] Quest quest;
    [SerializeField] string Areaname;

    public override void CheckForCompleting()
    {
       
    }

    public override void StartStep()
    {
        QuestSystem.AssignQuest(quest);
        QuestSystem.Assign_OnQuestFinished(QuestFinished);
        if (QuestSystem.GetQuestType() == Quest.QuestType.Location) { 
            TutorialUtility.Instance.beaconZone.TurnOnPing(true);
            TutorialUtility.Instance.beaconZone.setAreaName(Areaname);
        }
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
