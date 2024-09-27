using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestUI : MonoBehaviour
{
    [SerializeField] private Transform gridTransform = null;
    [SerializeField] private TMP_Text questReqPrefab = null;

    private List<TMP_Text> questReqTexts = new List<TMP_Text>();
    private void Start()
    {
        UpdateUI();
        QuestSystem.Assign_OnQuestUpdated(UpdateUI);
        QuestSystem.Assign_OnQuestFinished(ClearUI);
    }
    private void UpdateUI()
    {
        if (QuestSystem.GetQuestType() == Quest.QuestType.Location)
        {
            RebuildQuestReqTexts(0);
            TMP_Text text = Instantiate(questReqPrefab, gridTransform);
            text.text = string.Format("Go to {0}", QuestSystem.GetQuestLocation().name);
            questReqTexts.Add(text);
        }
        else
        {
            List<Quest.FishAmount> questReqs = QuestSystem.GetQuestReqs();
            if (questReqs == null)
            {
                RebuildQuestReqTexts(0);
                return;
            }
            if (questReqTexts.Count != questReqs.Count)
            {
                RebuildQuestReqTexts(questReqs.Count);
            }
            for (int i = 0; i < questReqs.Count; i++)
            {
                questReqTexts[i].text = string.Format("{0} {1} - {2}/{3}",
                    QuestSystem.GetQuestType().ToString(),
                    questReqs[i].name,
                    QuestSystem.GetCurrentValue(i),
                    questReqs[i].amount);
            }
        }
    }
    private void RebuildQuestReqTexts(int amount)
    {
        foreach(Transform child in gridTransform)
        {
            Destroy(child.gameObject);
        }
        questReqTexts.Clear();
        for(int i = 0; i < amount; i++)
        {
            questReqTexts.Add(Instantiate(questReqPrefab, gridTransform));
        }
    }
    private void ClearUI()
    {
        RebuildQuestReqTexts(0);
    }
    private void OnDestroy()
    {
        questReqTexts.Clear();
    }
}
