using System;
using System.Collections.Generic;

public static class QuestSystem
{
    private static Quest CurrentQuest;
    private static int[] CurrentValues = null;

    private static Action OnQuestUpdated;
    private static Action OnQuestFinished;

    private static float _TimeLastQuestFinished;

    public static void ScannedFish(string fishName)
    {
        for(int i = 0; i < CurrentQuest.Fishes.Count; i++)
        {
            if (CurrentQuest.Fishes[i].name == fishName)
            {
                CurrentValues[i]++;
                if (CurrentValues[i] > CurrentQuest.Fishes[i].amount) CurrentValues[i] = CurrentQuest.Fishes[i].amount;
            }
        }
        Call_OnQuestUpdated();
        CheckQuestFinish();
    }
    public static void AssignQuest(Quest quest)
    {
        CurrentQuest = quest;
        CurrentValues = new int[quest.Fishes.Count];
        Call_OnQuestUpdated();
    }
    private static void CheckQuestFinish()
    {
        for (int i = 0; i < CurrentValues.Length; i++)
        {
            if (CurrentValues[i] < CurrentQuest.Fishes[i].amount) return;
        }
        CurrentQuest = null;
        _TimeLastQuestFinished = UnityEngine.Time.time;
        Call_OnQuestFinished();
    }
    private static void Call_OnQuestUpdated()
    {
        if(OnQuestUpdated != null) OnQuestUpdated();
    }
    private static void Call_OnQuestFinished()
    {
        if(OnQuestFinished != null) OnQuestFinished();
    }
    public static void Assign_OnQuestUpdated(Action action)
    {
        OnQuestUpdated += action;
    }
    public static void Assign_OnQuestFinished(Action action)
    {
        OnQuestFinished += action;
    }
    public static void Reset()
    {
        CurrentQuest = null;
        CurrentValues = null;
        OnQuestUpdated = null;
        _TimeLastQuestFinished = 0f;
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
    public static bool HasQuest()
    {
        return CurrentQuest != null;
    }
    public static float TimeLastQuestFinished => _TimeLastQuestFinished;
}
