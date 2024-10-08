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
        base.Init(setList);
        foreach (object setObj in setList)
        {
            if (setObj as SubmarineMovement)
            {
                this.movement = (SubmarineMovement)setObj;
            }
        }
        movement.SetMovementVector(upgrades[level]);
    }
    private void Update()
    {
        // Debug
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.P))
        {
            movement.SetMovementVector(new MovementVector(20, 12, 10, 10));
        }
#endif
    }
    public override void UpgradeLevel()
    {
        base.UpgradeLevel();

        movement.SetMovementVector(upgrades[level]);
    }
}

