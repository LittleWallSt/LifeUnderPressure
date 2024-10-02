using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeBox : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText = null;
    [SerializeField] private TMP_Text upgradeCostText = null;
    [SerializeField] private GridLayoutGroup levelGrid = null;
    [SerializeField] private Image levelCirclePrefab = null;
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color boughtColor = Color.white;

    private SubmarineUpgrade upgrade = null;
    private List<Image> levelCircles = new List<Image>();

    private static Action OnUpgrade;
    private void Setup()
    {
        foreach(Transform child in levelGrid.transform)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < upgrade.MaxLevel; i++)
        {
            Image levelCircle = Instantiate(levelCirclePrefab, levelGrid.transform);
            levelCircle.color = upgrade.Level > i ? boughtColor : defaultColor;
            levelCircles.Add(levelCircle);
        }
        titleText.text = upgrade.GetType().ToString();
        UpdateUI();
    }
    public void Button_Upgrade()
    {
        Submarine submarine = Submarine.Instance;
        bool upgraded = upgrade.TryUpgradeLevel(submarine.Money);
        if (!upgraded) return;

        // Janko >> 
        AudioManager.instance.PlayOneShot(FMODEvents.instance.upgradeFX, Camera.main.transform.position);
        // Janko <<

        levelCircles[upgrade.Level - 1].color = boughtColor;
        submarine.Money -= upgrade.GetLevelUpgradeCost();
        UpdateUI();
        Call_OnUpgrade();
    }
    private void UpdateUI()
    {
        upgradeCostText.text = upgrade.Level < upgrade.MaxLevel ? "Cost: " + upgrade.UpgradeCost[upgrade.Level + 1].ToString() : "Fully Upgraded";
    }
    // Setters
    public void SetUpgrade(SubmarineUpgrade upgrade)
    {
        this.upgrade = upgrade;
        Setup();
    }
    // Actions
    public static void Assign_OnUpgrade(Action action)
    {
        OnUpgrade += action;
    }
    private static void Call_OnUpgrade()
    {
        if (OnUpgrade != null) OnUpgrade();
    }
    public static void Remove_OnUpgrade(Action action)
    {
        OnUpgrade -= action;
    }
}
