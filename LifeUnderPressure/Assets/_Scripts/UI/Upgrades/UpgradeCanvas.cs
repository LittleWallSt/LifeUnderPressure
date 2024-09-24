using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeCanvas : MonoBehaviour
{
    [SerializeField] private Transform gridTransform = null;
    [SerializeField] private UpgradeBox upgradeBoxPrefab = null;

    private Submarine submarine = null;
    public void SetupCanvas(Submarine submarine)
    {
        this.submarine = submarine; 
        foreach (Transform child in gridTransform)
        {
            Destroy(child.gameObject);
        }
        foreach (SubmarineUpgrade upgrade in submarine.GetComponents<SubmarineUpgrade>())
        {
            UpgradeBox box = Instantiate(upgradeBoxPrefab, gridTransform);
            box.SetUpgrade(upgrade);
        }
    }
    public bool EnableMenu(bool state)
    {
        gameObject.SetActive(state);
        Time.timeScale = state ? 0f : 1f;
        InternalSettings.EnableCursor(gameObject.activeSelf);
        return state;
    }
}
