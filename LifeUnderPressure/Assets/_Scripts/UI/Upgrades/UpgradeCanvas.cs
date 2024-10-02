using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpgradeCanvas : MonoBehaviour
{
    [SerializeField] private Transform gridTransform = null;
    [SerializeField] private UpgradeBox upgradeBoxPrefab = null;
    [SerializeField] private TMP_Text moneyText = null;

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
        UpgradeBox.Assign_OnUpgrade(UpdateMoneyUI);
    }
    public bool EnableMenu(bool state)
    {
        gameObject.SetActive(state);
        Time.timeScale = state ? 0f : 1f;
        InternalSettings.EnableCursor(gameObject.activeSelf);
        UpdateMoneyUI();
        return state;
    }
    private void UpdateMoneyUI()
    {
        moneyText.text = "Money: " + submarine.Money;
    }
    private void OnDestroy()
    {
        UpgradeBox.Remove_OnUpgrade(UpdateMoneyUI);
    }
}
