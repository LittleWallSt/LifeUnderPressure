using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeBox : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText = null;
    [SerializeField] private GridLayoutGroup levelGrid = null;
    [SerializeField] private Image levelCirclePrefab = null;
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color boughtColor = Color.white;

    private SubmarineUpgrade upgrade = null;
    private List<Image> levelCircles = new List<Image>();
    private void Setup()
    {
        foreach(Transform child in levelGrid.transform)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < upgrade.MaxLevel; i++)
        {
            Image levelCircle = Instantiate(levelCirclePrefab, levelGrid.transform);
            levelCircle.color = defaultColor;
            levelCircles.Add(levelCircle);
        }
        titleText.text = upgrade.GetType().ToString();
    }
    public void Button_Upgrade()
    {
        upgrade.Debug_UpgradeLevel();
        levelCircles[upgrade.Level - 1].color = boughtColor;
    }
    public void SetUpgrade(SubmarineUpgrade upgrade)
    {
        this.upgrade = upgrade;
        Setup();
    }
}
