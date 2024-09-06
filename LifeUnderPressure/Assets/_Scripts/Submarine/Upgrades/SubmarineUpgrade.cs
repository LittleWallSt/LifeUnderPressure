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

    protected virtual void SU_EditorSetup()
    {
        if (upgradeEvents.Length != maxLevel + 1)
        {
            upgradeEvents = new UnityEvent[maxLevel + 1];
        }
    }
    public virtual void Init(params object[] setList)
    {
        // initialize in subscripts
    }

    protected virtual void UpgradeLevel()
    {
        if(level + 1 > maxLevel)
        {
            throw new System.Exception("Level up is exceeding the max level limit");
        }
        level++;
        upgradeEvents[level].Invoke();
    }
}
