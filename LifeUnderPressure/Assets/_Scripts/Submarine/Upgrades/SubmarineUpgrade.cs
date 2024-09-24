using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class SubmarineUpgrade : MonoBehaviour
{
    [Header("General")]
    [SerializeField] protected int maxLevel = 1;
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
    protected virtual void SU_EditorSetup()
    {
        if (upgradeEvents == null || upgradeEvents.Length != maxLevel + 1)
        {
            upgradeEvents = new UnityEvent[maxLevel + 1];
        }
    }
    protected virtual void Start()
    {
        upgradeEvents[level].Invoke();
    }
    public virtual void Init(params object[] setList)
    {
        // initialize in subscripts
    }

    protected virtual void UpgradeLevel()
    {
        Level++;
    }

    // Debug
    public void Debug_UpgradeLevel()
    {
        UpgradeLevel();
    }
}
