using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class SubmarineUpgrade : MonoBehaviour
{
    [Header("General")]
    [SerializeField] protected int maxLevel = 1;
    [SerializeField] private int[] upgradeCost;
    [SerializeField] protected UnityEvent[] upgradeEvents = null;

    protected int level = 0;

    public int Level
    {
        get
        {
            return level;
        }
        private set
        {
            if (value > maxLevel)
            {
                Debug.Log("Overupgrading " + name);
            }
            else if (value < 0)
            {
                throw new System.Exception("Level cannot be less than 0");
            }
            else
            {
                level = value;
                upgradeEvents[level].Invoke();
            }
        }
    }
    public int MaxLevel => maxLevel;
    public int[] UpgradeCost => upgradeCost;
    protected virtual void SU_EditorSetup()
    {
        if (upgradeEvents == null || upgradeEvents.Length != maxLevel + 1)
        {
            upgradeEvents = new UnityEvent[maxLevel + 1];
        }
        if (upgradeCost == null || upgradeCost.Length != maxLevel + 1)
        {
            upgradeCost = new int[maxLevel + 1];
        }
    }
    public virtual void Init(params object[] setList)
    {
        level = DataManager.Get("Upgrade_" + GetType().ToString(), 0);
        upgradeEvents[level].Invoke();
    }

    protected virtual void UpgradeLevel()
    {
        Level++;
        DataManager.Write("Upgrade_" + GetType().ToString(), level);
    }
    public bool TryUpgradeLevel(int money)
    {
        if (level >= maxLevel) return false;

        int cost = upgradeCost[level + 1];
        if(money >= cost)
        {
            UpgradeLevel();
            return true;
        }
        return false;
    }
    // Getters
    public int GetLevelUpgradeCost()
    {
        return upgradeCost[level];
    }
}
