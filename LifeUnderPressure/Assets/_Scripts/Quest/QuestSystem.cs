using System;
using System.Collections.Generic;

public static class QuestSystem
{
    private static Quest CurrentQuest;
    private static int[] CurrentValues = null;

    private static Action OnQuestUpdated;

    public static void ScannedFish(string fishName)
    {
        for(int i = 0; i < CurrentQuest.Fishes.Count; i++)
        {
            if (CurrentQuest.Fishes[i].name == fishName)
            {
                CurrentValues[i]++;
            }
        }
        Call_OnQuestUpdated();
    }
    public static void AssignQuest(Quest quest)
    {
        CurrentQuest = quest;
        CurrentValues = new int[quest.Fishes.Count];
        Call_OnQuestUpdated();
    }
    private static void Call_OnQuestUpdated()
    {
        if(OnQuestUpdated != null) OnQuestUpdated();
    }
    public static void Assign_OnQuestUpdated(Action action)
    {
        OnQuestUpdated += action;
    }
    public static void Reset()
    {
        CurrentQuest = null;
        CurrentValues = null;
        OnQuestUpdated = null;
    }
    // Getters
    public static List<Quest.FishAmount> GetQuestReqs()
    {
        return CurrentQuest != null ? CurrentQuest.Fishes : null;
    }
    public static Quest.QuestType GetQuestType()
    {
        return CurrentQuest != null ? CurrentQuest.Type : Quest.QuestType.None;
    }
    public static int GetCurrentValue(int index)
    {
        return CurrentValues[index];
    }
}
