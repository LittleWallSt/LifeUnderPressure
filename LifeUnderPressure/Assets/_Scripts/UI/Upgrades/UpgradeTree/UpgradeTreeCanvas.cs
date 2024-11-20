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

    public Action<UpgradeType> onUnlock;

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

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
            EnableMenu(false);
    }



    public bool EnableMenu(bool state)
    {
        gameObject.SetActive(state);
        Submarine.Instance.getSubmarineMovement().enabled = !state;
        UpdateMoneyUI();
        Time.timeScale = state ? 0f : 1f;
        InternalSettings.EnableCursor(gameObject.activeSelf);
        return state;
    }

    private float hoverMenuWidth = 400f;
    private float screenWidth = 1920f;
    private float borderOff = 20f;

    public void SetHoverMenu(bool state, string description = "", string req = "", RectTransform buttonPos = null)
    {
        hoverPanel.SetActive(state);
        
        if (state) {
            int sign = (buttonPos.position.x + offset + -hoverMenuWidth * 0.5f - borderOff) >= screenWidth * 0.5 ? 1 : -1;
            _hoverPanel.position = buttonPos.position + new Vector3(sign * offset, 0, 0); }
        upgradeDescription.text = "Description: " + description;
        upgradeRequirement.text = req;

    }



    public void UpdateMoneyUI()
    {
        XP.text = "XP: " + Submarine.Instance.Money;
    }

    

    public bool EnableMenu(bool state, GameObject _submarineBody)
    {
        Submarine.Instance.getSubmarineMovement().StopSubmarine();
        Submarine.Instance.getSubmarineMovement().enabled = !state;
        gameObject.SetActive(state);
        UpdateMoneyUI();
        InternalSettings.EnableCursor(gameObject.activeSelf);
        return state;
    }

    

    public void UnlockQuestNode(UpgradeType type)
    {
        Debug.Log(upgradeNodes.Length);
        foreach (var upgrade in upgradeNodes)
        {
            if (upgrade.skillNode.upgradeType==type) 
            {
                onUnlock.Invoke(type);
                upgrade.UnlockQuestNode();
            } 
        }
    }

    



}


