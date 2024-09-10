using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeCanvas : MonoBehaviour
{
    [SerializeField] private Submarine submarine = null;
    [SerializeField] private Transform gridTransform = null;
    [SerializeField] private UpgradeBox upgradeBoxPrefab = null;

    private void Start()
    {
        foreach(Transform child in gridTransform)
        {
            Destroy(child.gameObject);
        }
        foreach(SubmarineUpgrade upgrade in submarine.GetComponents<SubmarineUpgrade>())
        {
            UpgradeBox box = Instantiate(upgradeBoxPrefab, gridTransform);
            box.SetUpgrade(upgrade);
        }
    }
}
