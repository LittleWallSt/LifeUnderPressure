using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmarineHull : SubmarineUpgrade
{
    [Header("Hull")]
    [SerializeField] private float[] upgrades = null;

    private Submarine submarine;

    [ContextMenu("SU_EditorSetup")]
    protected override void SU_EditorSetup()
    {
        base.SU_EditorSetup();

        if (upgrades.Length != maxLevel + 1)
        {
            upgrades = new float[maxLevel + 1];
        }
    }
    protected override void Start()
    {
        base.Start();
        submarine.SetThicknessOfHull(upgrades[level]);
    }
    public override void Init(params object[] setList)
    {
        foreach (object setObj in setList)
        {
            if (setObj as Submarine)
            {
                this.submarine = (Submarine)setObj;
            }
        }
    }
    protected override void UpgradeLevel()
    {
        base.UpgradeLevel();

        submarine.SetThicknessOfHull(upgrades[level]);
    }
}
