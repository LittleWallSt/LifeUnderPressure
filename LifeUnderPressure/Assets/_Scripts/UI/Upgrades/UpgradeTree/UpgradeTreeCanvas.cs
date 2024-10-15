using System;
using System.Collections;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class UpgradeTreeCanvas : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI XP;

    [Header("Hover panel")]
    [SerializeField] GameObject hoverPanel;
    [SerializeField] TextMeshProUGUI upgradeDescription;
    [SerializeField] TextMeshProUGUI upgradeRequirement;
    [SerializeField] float offset = -40f;
    public bool hoverActive = false;

    RectTransform _hoverPanel;

    private Submarine submarine = null;
    [HideInInspector] public UpgradeNode[] upgradeNodes;

    public static UpgradeTreeCanvas Instance { get; private set; } = null;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; } 
    }

    private void Start()
    {
        _hoverPanel = hoverPanel.GetComponent<RectTransform>();
        upgradeNodes = FindObjectsOfType<UpgradeNode>();
        gameObject.SetActive(false); 

    }



    public bool EnableMenu(bool state)
    {
        gameObject.SetActive(state);
        Time.timeScale = state ? 0f : 1f;
        InternalSettings.EnableCursor(gameObject.activeSelf);
        UpdateMoneyUI();
        return state;
    }

    

    public void SetHoverMenu(bool state, string description = "", string req = "", RectTransform buttonPos = null)
    {
        hoverPanel.SetActive(state);
        if (state)_hoverPanel.position = buttonPos.position + new Vector3(offset, 0, 0); 
        upgradeDescription.text = "Description: " + description;
        upgradeRequirement.text = req;

    }



    private void UpdateMoneyUI()
    {
        XP.text = "XP: " + submarine.Money;
    }

    

    public bool EnableMenu(bool state, GameObject _submarineBody)
    {
        gameObject.SetActive(state);
        InternalSettings.EnableCursor(gameObject.activeSelf);
        return state;
    }

    

    public void UnlockUpgrades()
    {
        foreach(var upgrade in upgradeNodes)
        {
            upgrade.UnlockUpgrade();
        }
    }

    public void UnlockQuestNode(UpgradeType type)
    {
        
        Debug.Log(upgradeNodes.Length);
        foreach (var upgrade in upgradeNodes)
        {
            if (upgrade.skillNode.upgradeType==type) 
            {
                upgrade.UnlockQuestNode();
            } 
        }
    }

    



}


