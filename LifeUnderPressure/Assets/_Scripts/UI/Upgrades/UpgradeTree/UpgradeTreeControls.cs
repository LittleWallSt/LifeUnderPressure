using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeTreeControls : MonoBehaviour
{

    //Test script to test stuff
    

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1)) {
            UpgradeTreeCanvas.Instance.UnlockQuestNode(UpgradeType.Hull);
            Debug.Log("hull unlocked");
        }

        if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            UpgradeTreeCanvas.Instance.UnlockQuestNode(UpgradeType.Motor);
            Debug.Log("motor unlocked");
        }
    }

}
