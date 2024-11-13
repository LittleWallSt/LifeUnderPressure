using System;
using System.Collections.Generic;
using UnityEngine;

public static class QuestSystem
{
    private static Quest CurrentQuest;
    private static int[] CurrentValues = null;

    private static Action OnQuestUpdated;
    private static Action OnQuestFinished;

    private static float _TimeLastQuestFinished;

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
                    //AudioManager.instance?.PlayOneShot(CurrentQuest.AudioOnProgress[i], Submarine.Instance.transform.position);
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
        //AudioManager.instance?.PlayOneShot(quest.AudioOnAssign, Submarine.Instance.transform.position);

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
    private static void QuestFinish()
    {
        if (CurrentQuest.VoicelineOnEnd)
            VoicelinesUI.Instance.CallVoiceline(CurrentQuest.VoicelineOnEnd);
        //AudioManager.instance?.PlayOneShot(CurrentQuest.AudioOnEnd, Submarine.Instance.transform.position);
        AcquireRewards();

        for (int i = 0; i < CurrentValues.Length; i++)
        {
            DataManager.Remove("QuestCurrentValue_" + i);
        }
        CurrentQuest = null;
        _TimeLastQuestFinished = Time.time;
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
    public static void Reset()
    {
        CurrentQuest = null;
        CurrentValues = null;
        OnQuestUpdated = null;
        _TimeLastQuestFinished = Time.time;
        LoadedQuestData = false;
        Debug.Log("res");
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
    public static float TimeLastQuestFinished => _TimeLastQuestFinished;
}
