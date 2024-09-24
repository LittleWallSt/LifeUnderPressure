using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmarineMotor : SubmarineUpgrade
{
    [Header("Motor")]
    [SerializeField] private MovementVector[] upgrades = null;

    private SubmarineMovement movement = null;

    [ContextMenu("SU_EditorSetup")]
    protected override void SU_EditorSetup()
    {
        base.SU_EditorSetup();

        if (upgrades.Length != maxLevel + 1)
        {
            upgrades = new MovementVector[maxLevel + 1];
        }
    }
    public override void Init(params object[] setList)
    {
        foreach(object setObj in setList)
        {
            if (setObj as SubmarineMovement)
            {
                this.movement = (SubmarineMovement)setObj;
            }
        }
    }
    protected override void Start()
    {
        base.Start();
        movement.SetMovementVector(upgrades[level]);
    }
    protected override void UpgradeLevel()
    {
        base.UpgradeLevel();

        movement.SetMovementVector(upgrades[level]);
    }
}

