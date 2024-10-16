
using UnityEngine;

public class SubmarineBoost : SubmarineUpgrade
{
    [Header("Boost")]
    private SubmarineMovement movement = null;

    [ContextMenu("SU_EditorSetup")]
    protected override void SU_EditorSetup()
    {
        base.SU_EditorSetup();

    }
    
}
