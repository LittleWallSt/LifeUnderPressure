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

    public QuestType Type => type;
    public List<FishAmount> Fishes => new List<FishAmount>(fishes);
    public List<Reward> Rewards => new List<Reward>(rewards);
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
