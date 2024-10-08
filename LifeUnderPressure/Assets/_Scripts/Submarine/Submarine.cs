using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(SubmarineMovement))]
[RequireComponent(typeof(Health))]
public class Submarine : MonoBehaviour, IDepthDependant
{
    [SerializeField] private float inDeepMaxTime = 10f;
    [SerializeField] private float deepOffset = 0f;
    [SerializeField] private float radiusOfHull = 10f;
    [SerializeField] private float thicknessOfHull = 10f;
    [SerializeField] private float maxStressTreshold = 24f;
    [SerializeField] private float stressDamageModifier = 0.25f;
    [SerializeField] private TMP_Text heightText = null;
    [SerializeField] private TMP_Text warningText = null;
    [SerializeField] private TMP_Text dockText = null;
    [SerializeField] private TMP_Text zoneText = null;
    [SerializeField] private UpgradeCanvas upgradeCanvas = null;
    [SerializeField] private PauseMenu pauseMenu = null;
    [SerializeField] private Encyclopedia encyclopedia = null;

    [SerializeField] private GameObject submarineBody;

    private GameObject currentMenu = null;
    private List<SubmarineUpgrade> upgrades = new List<SubmarineUpgrade>();

    private Health health;
    private SubmarineMovement movement;

    private int money = 0;
    private float stress = 0f;
    private float inDeepTime = 0f;
    private bool docked = false;
    public static Submarine Instance { get; private set; } = null;
    public int Money 
    {
        get 
        { 
            return money; 
        } 
        set
        {
            if (value < 0)
            {
                money = 0;
                Debug.LogError("Money went below zero. Check if that is intended");
            }
            else money = value;

            DataManager.Write("Money", money);
        } 
    }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        movement = GetComponent<SubmarineMovement>();
        health = GetComponent<Health>();
    }
    public void Init()
    {
        warningText.gameObject.SetActive(false);
        if (upgradeCanvas != null) upgradeCanvas.gameObject.SetActive(false);
        if (pauseMenu != null) pauseMenu.EnableMenu(false);
        EnableDockText(false);
        inDeepTime = 0f;

        health.Assign_OnDie(Die);

        foreach(SubmarineUpgrade upgrade in GetComponents<SubmarineUpgrade>())
        {
            upgrades.Add(upgrade);
            upgrade.Init(this, movement);
        }

        DataManager.Assign_OnSaveData(StorePositionData);
        Money = DataManager.Get("Money", 0);

        Vector3 spawnPosition = GameManager.Instance ? GameManager.Instance.InitialSpawnPoint : Vector3.zero;
        spawnPosition.x = DataManager.Get("SpawnPositionX", Mathf.RoundToInt(spawnPosition.x));
        spawnPosition.y = DataManager.Get("SpawnPositionY", Mathf.RoundToInt(spawnPosition.y));
        spawnPosition.z = DataManager.Get("SpawnPositionZ", Mathf.RoundToInt(spawnPosition.z));
        transform.position = spawnPosition;

        Vector3 eulerAngles = GameManager.Instance ? GameManager.Instance.InitialEulerAngles : Vector3.zero;
        transform.eulerAngles = eulerAngles;
    }
    private void FixedUpdate()
    {
        LevelVolume current = LevelVolume.Current;
        float depth = ((-transform.position.y - current.DepthRange.x) / current.DepthRange.y) * current.MaxFakeDepth;
        if (current.Level > 0) depth += LevelVolume.List.Find(x => x.Level == current.Level - 1).MaxFakeDepth;
        
        LCStressCalculation(-transform.position.y);

        if (heightText)
        {
            heightText.text = string.Format("{0:F1}m", depth);
        }
    }

    private void Update()
    {
        if (docked) return;

        PauseMenuInput();
        //UpgradeCanvasInput();
        EncyclopediaInput();
    }

    private void PauseMenuInput()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;

        if(currentMenu == null || currentMenu == pauseMenu.gameObject || !currentMenu.activeSelf)
        {
            currentMenu = pauseMenu.EnableMenu(!pauseMenu.gameObject.activeSelf) ? pauseMenu.gameObject : null;
        }
    }

    //Ulia chnanges**

    private void EncyclopediaInput()
    {
        if (!Input.GetKeyDown(KeyCode.Q)) return;

        if (currentMenu == null || currentMenu == encyclopedia.gameObject || !currentMenu.activeSelf)
        {
            currentMenu = encyclopedia.EnableMenu(!encyclopedia.gameObject.activeSelf, submarineBody) ? encyclopedia.gameObject : null;
        }
    }
    //**
    private void UpgradeCanvasInput()
    {
        if (!docked) return;
        if (!Input.GetKeyDown(KeyCode.Tab)) return;

        if (currentMenu == null || currentMenu == upgradeCanvas.gameObject || !currentMenu.activeSelf)
        {
            currentMenu = upgradeCanvas.EnableMenu(!upgradeCanvas.gameObject.activeSelf) ? upgradeCanvas.gameObject : null;
        }
    }
    public void ForceEnableUpgradeCanvas()
    {
        ForceCloseCurrentMenu();
        currentMenu = upgradeCanvas.EnableMenu(true) ? upgradeCanvas.gameObject : null;
    }
    public void ForceCloseCurrentMenu()
    {
        if (currentMenu == encyclopedia.gameObject)
        {
            encyclopedia.EnableMenu(false, null);
        }
        else if (currentMenu == pauseMenu.gameObject)
        {
            pauseMenu.EnableMenu(false);
        }
        else if (currentMenu == upgradeCanvas.gameObject)
        {
            upgradeCanvas.EnableMenu(false);
        }
    }
    public void ForceCloseUpgradeCanvas()
    {
        upgradeCanvas.EnableMenu(false);
        if (currentMenu == upgradeCanvas) currentMenu = null;
    }
    private void Die()
    {
        Debug.Log("Submarine died");
        transform.position = new Vector3(0f, -2f, 0f);
        transform.rotation = Quaternion.identity;
        movement.ResetMovement();
        health.ResetHealth();
    }

    private void LCStressCalculation(float depth)
    {
        stress = (((1000f + (depth / 11000f * 50f)) * 9.81f * depth * radiusOfHull) / (2f * thicknessOfHull)) / 101325f;

        if (stress > 100)
        {
            float diff = stress - 100f;
            health.DealDamage((diff / maxStressTreshold) * health.MaxHealth * Time.fixedDeltaTime * stressDamageModifier);
            warningText.gameObject.SetActive(true);
        }
        else
        {
            warningText.gameObject.SetActive(false);
        }
    }
    public void UpgradeSubmarine(System.Type upgradeType)
    {
        foreach(SubmarineUpgrade upgrade in upgrades)
        {
            if(upgrade.GetType() == upgradeType)
            {
                upgrade.UpgradeLevel();
            }
        }
    }
    private void StorePositionData()
    {
        DataManager.Write("SpawnPositionX", Mathf.RoundToInt(transform.position.x));
        DataManager.Write("SpawnPositionY", Mathf.RoundToInt(transform.position.y));
        DataManager.Write("SpawnPositionZ", Mathf.RoundToInt(transform.position.z));
    }
    private void OnDestroy()
    {
        DataManager.Remove_OnSaveData(StorePositionData);
    }
    // Setters
    public void SetDocked(bool state)
    {
        docked = state;
    }
    public void EnableDockText(bool state)
    {
        dockText?.gameObject.SetActive(state);
    }
    public void EnableMovement(bool state)
    {
        movement.enabled = state;
    }
    public void SetThicknessOfHull(float newThickness)
    {
        thicknessOfHull = newThickness;
    }
    public void AddMoney(int amount)
    {
        Money += amount;
    }
    // IDepthDependant
    public bool IDD_OnDepthLevelEnter(int level)
    {
        zoneText.text = LevelVolume.Current ? LevelVolume.Current.ZoneName : string.Empty;
        return true;
    }
    public void IDD_NotAllowedUpdate(int level, float deltaTime)
    {
        inDeepTime += deltaTime;
        if(inDeepTime > inDeepMaxTime)
        {
            inDeepTime = 0f;
            Die();
        }
    }

    public void IDD_OnDepthLevelExit(int level)
    {
        Debug.Log("exit level " + level);
    }
    public int IDD_GetGOInstanceID()
    {
        return gameObject.GetInstanceID();
    }

    // Gizmos
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + new Vector3(0f, deepOffset, 0f), 0.1f);
    }
    private void OnGUI()
    {
        // Shows the stress on screen
        //GUI.Label(new Rect(1000, 10, 500, 100), string.Format("LC Stress: {0}", stress), InternalSettings.Get.DebugStyle);
    }
}
