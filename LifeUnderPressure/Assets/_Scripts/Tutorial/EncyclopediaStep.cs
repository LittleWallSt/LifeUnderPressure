using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncyclopediaStep : TutorialStep
{
    bool opened;
    public override void CheckForCompleting()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            opened = true;
        }

        if(opened) CompleteStep();
    }

    public override void StartStep()
    {
        opened = false;
    }

}
