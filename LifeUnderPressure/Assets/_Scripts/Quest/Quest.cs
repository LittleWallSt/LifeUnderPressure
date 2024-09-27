using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "LUP/New Quest")]
public class Quest : ScriptableObject
{
    [SerializeField] private QuestType type;
    [SerializeField] private List<FishAmount> fishes = null;
    [SerializeField] private List<Reward> rewards = null;

    [SerializeField] private EventReference audioOnAssign;
    [SerializeField] private EventReference[] audioOnProgress;
    [SerializeField] private EventReference audioOnEnd;

    public QuestType Type => type;
    public List<FishAmount> Fishes => new List<FishAmount>(fishes);
    public List<Reward> Rewards => new List<Reward>(rewards);

    public EventReference AudioOnAssign => audioOnAssign;
    public EventReference[] AudioOnProgress => audioOnProgress;
    public EventReference AudioOnEnd => audioOnEnd;
    public enum QuestType
    {
        Scan,
        Capture,
        None
    }
    [Serializable]
    public struct FishAmount
    {
        public string name;
        public int amount;
    }
    public enum RewardType
    {
        Money,
        Bool
    }
    [Serializable]
    public struct Reward
    {
        public RewardType type;
        public string boolName;
        public int value;
    }
}
