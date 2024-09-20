using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmarineScanner : SubmarineUpgrade
{
    [SerializeField] private Scanner scanner = null;
    [SerializeField] private ScannerStruct[] upgrades = null;

    [ContextMenu("SU_EditorSetup")]
    protected override void SU_EditorSetup()
    {
        base.SU_EditorSetup();

        if (upgrades.Length != maxLevel + 1)
        {
            upgrades = new ScannerStruct[maxLevel + 1];
        }
    }

    private void Start()
    {
        upgradeEvents[level].Invoke();
    }
    protected override void UpgradeLevel()
    {
        base.UpgradeLevel();

        scanner.SetScanTimer(upgrades[level].scanTimer);
        scanner.SetScanAnimationSpeed(upgrades[level].scanAnimationSpeed);
        scanner.SetDepletingSpeed(upgrades[level].depletingSpeed);
    }
}
