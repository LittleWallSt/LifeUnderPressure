using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmarineScanner : SubmarineUpgrade
{

    [ContextMenu("SU_EditorSetup")]
    protected override void SU_EditorSetup()
    {
        base.SU_EditorSetup();
    }

    private void Start()
    {
        upgradeEvents[level].Invoke();
    }
}
