using System;
using System.Collections.Generic;
using UnityEngine;

public static class QuestSystem
{
    private static Quest CurrentQuest;
    private static int[] CurrentValues = null;

    private static Action OnQuestUpdated;
    private static Action OnQuestFinished;

    public static float TimeLastQuestFinished { get; private set; }

    private static bool LoadedQuestData;

    public static void ScannedFish(FishInfo fish)
    {
        if (CurrentQuest == null) return;

        for(int i = 0; i < CurrentQuest.Fishes.Count; i++)
        {
            if (CurrentQuest.Fishes[i].fish == fish && CurrentValues[i] < CurrentQuest.Fishes[i].amount)
            {
                CurrentValues[i]++;
                if (CurrentValues[i] == CurrentQuest.Fishes[i].amount)
                {
                    if (CurrentQuest.VoicelineOnProgress.Length > i && CurrentQuest.VoicelineOnProgress[i])
                        VoicelinesUI.Instance.CallVoiceline(CurrentQuest.VoicelineOnProgress[i]);
                }
            }
        }
        Call_OnQuestUpdated();
        CheckQuestFinish();
    }
    public static void AssignQuest(Quest quest)
    {
        CurrentQuest = quest;
        if(quest.VoicelineOnAssign) 
            VoicelinesUI.Instance.CallVoiceline(quest.VoicelineOnAssign);

        CurrentValues = new int[quest.Fishes.Count];

        if (!LoadedQuestData)
        {
            LoadedQuestData = true;
            LoadQuestData();
        }

        Call_OnQuestUpdated();
    }
    private static void CheckQuestFinish()
    {
        for (int i = 0; i < CurrentValues.Length; i++)
        {
            if (CurrentValues[i] < CurrentQuest.Fishes[i].amount) return;
        }
        QuestFinish();
    }
    public static void ResetFishProgress(FishInfo fish)
    {
        if (!CurrentQuest) return;

        for(int i = 0; i < CurrentValues.Length; i++)
        {
            if (CurrentQuest.Fishes[i].fish == fish)
            {
                CurrentValues[i] = 0;
            }
        }
        Call_OnQuestUpdated();
    }
    private static void QuestFinish()
    {
        if (CurrentQuest.VoicelineOnEnd)
            VoicelinesUI.Instance.CallVoiceline(CurrentQuest.VoicelineOnEnd);

        AcquireRewards();

        for (int i = 0; i < CurrentValues.Length; i++)
        {
            DataManager.Remove("QuestCurrentValue_" + i);
        }
        CurrentQuest = null;
        TimeLastQuestFinished = Time.time;
        Call_OnQuestFinished();
    }

    private static void AcquireRewards()
    {
        foreach (var reward in CurrentQuest.Rewards)
        {
            switch (reward.type)
            {
                case Quest.RewardType.Money:
                    Submarine.Instance.AddMoney(reward.value);
                    break;
                case Quest.RewardType.Bool:
                    GameManager.Instance.ProcessWriteBool(reward.boolName, reward.value);
                    break;
            }
        }
    }
    public static void SaveCurrentQuest()
    {
        if (CurrentQuest == null) return;

        for(int i = 0; i < CurrentValues.Length; i++)
        {
            DataManager.Write("QuestCurrentValue_" + i, CurrentValues[i]);
        }
    }
    private static void LoadQuestData()
    {
        for (int i = 0; i < CurrentValues.Length; i++)
        {
            CurrentValues[i] = DataManager.Get("QuestCurrentValue_" + i, 0);
        }
    }
    public static void InQuestLocation()
    {
        QuestFinish();
    }
    public static void ForceCompleteQuest()
    {
        QuestFinish();
    }
    public static void ForceScanFish(int number)
    {
        if (number <= 0) return;
        for(int i = 0; i < CurrentValues.Length; i++)
        {
            if (CurrentValues[i] == 1) continue;
            GameManager.Instance.ScannedFish(CurrentQuest.Fishes[i].fish);
            if (--number <= 0) return;
        }
        foreach(var f in CurrentQuest.Fishes)
        {
        }
    }
    // Actions
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
    public static void Remove_OnQuestUpdated(Action action)
    {
        OnQuestUpdated -= action;
    }
    public static void Remove_OnQuestFinished(Action action)
    {
        OnQuestFinished -= action;
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
    public static Quest.Location GetQuestLocation()
    {
        return CurrentQuest._Location;
    }
    public static bool HasQuest()
    {
        return CurrentQuest != null;
    }
    public static void Reset()
    {
        CurrentQuest = null;
        CurrentValues = null;
        OnQuestUpdated = null;
        TimeLastQuestFinished = Time.time;
        LoadedQuestData = false;
    }
}
