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
    }
    private void UpdateUI()
    {
        List<Quest.FishAmount> questReqs = QuestSystem.GetQuestReqs();
        if (questReqs == null) return;
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
    private void OnDestroy()
    {
        questReqTexts.Clear();
    }
}
