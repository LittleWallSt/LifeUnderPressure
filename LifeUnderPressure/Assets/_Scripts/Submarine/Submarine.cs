using FMOD.Studio;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(SubmarineMovement))]
[RequireComponent(typeof(Health))]
public class Submarine : MonoBehaviour, IDepthDependant
{
    // Janko >>
    private EventInstance warningInstance;
    // Janko <<

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
    [SerializeField] private UpgradeTreeCanvas upgradeTreeCanvas = null;
    [SerializeField] private PauseMenu pauseMenu = null;
    [SerializeField] private Encyclopedia encyclopedia = null;
    [SerializeField] private Light sun = null;
    [SerializeField] private Material cracksMaterial = null;
    [SerializeField] private DyingEvent dyingEvent = null;

    [SerializeField] private GameObject submarineBody;
    [SerializeField] private MeshRenderer submarineMeshRenderer = null;

    private Material cracksMaterialInstance = null;

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

        CracksMaterialSetup();

        movement = GetComponent<SubmarineMovement>();
        health = GetComponent<Health>();
    }
    private void CracksMaterialSetup()
    {
        cracksMaterialInstance = Instantiate(cracksMaterial);
        List<Material> mats = new List<Material>(submarineMeshRenderer.materials);
        mats[1] = cracksMaterialInstance;
        submarineMeshRenderer.SetMaterials(mats);
    }
    private void UpdateCracksOnWindshield(float value)
    {
        float fraction = Mathf.Abs(1f - (value / health.MaxHealth));

        cracksMaterialInstance.SetFloat("_Cracks1", fraction > 0.25f ? 1f : 0f);
        cracksMaterialInstance.SetFloat("_Cracks2", fraction > 0.50f ? 1f : 0f);
        cracksMaterialInstance.SetFloat("_Cracks3", fraction > 0.75f ? 1f : 0f);
    }
    private void Start()
    {
        // Janko >>
        warningInstance = AudioManager.instance.CreateInstance(FMODEvents.instance.SFX_Warning);
        warningInstance.setParameterByName("shouldPlay", 0);
        warningInstance.start();
        // Janko <<

        LevelVolume.Assign_OnCurrentVolumeChanged(OnLevelVolumeChanged);
    }

    public void Init()
    {
        warningText.gameObject.SetActive(false);
        if (upgradeCanvas != null) upgradeCanvas.gameObject.SetActive(false);
        if (pauseMenu != null) pauseMenu.EnableMenu(false);
        EnableDockText(false);
        inDeepTime = 0f;

        health.Assign_OnDie(Die);
        health.Assign_OnValueChanged(UpdateCracksOnWindshield);

        foreach (SubmarineUpgrade upgrade in GetComponents<SubmarineUpgrade>())
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
        float depth = -transform.position.y;

        LerpSunIntensity(current, depth);

        depth = FakeDepth(current, depth);

        LCStressCalculation(-transform.position.y);

        if (heightText)
        {
            heightText.text = string.Format("{0:F1}m", depth);
        }
    }
    private void LerpSunIntensity(LevelVolume current, float depth)
    {
        if (sun && current)
        {
            sun.intensity = Mathf.Lerp(current.SunLightIntensityRange.x, current.SunLightIntensityRange.y, (depth - current.DepthRange.x) / (current.DepthRange.y - current.DepthRange.x));
        }
    }

    private float FakeDepth(LevelVolume current, float depth)
    {
        if (current)
        {
            depth = ((-transform.position.y - current.DepthRange.x) / current.DepthRange.y) * current.MaxFakeDepth;
            if (current.Level > 0) depth += LevelVolume.List.Find(x => x.Level == current.Level - 1).MaxFakeDepth;
        }

        return depth;
    }
    private void Update()
    {
        if (docked) return;

        PauseMenuInput();
        //UpgradeCanvasInput();
        EncyclopediaInput();
        UpgradeTreeInput();
    }

    private void PauseMenuInput()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;

        if(currentMenu == null || currentMenu == pauseMenu.gameObject || !currentMenu.activeSelf)
        {
            currentMenu = pauseMenu.EnableMenu(!pauseMenu.gameObject.activeSelf) ? pauseMenu.gameObject : null;
        }
    }

    //Ulia chnanges>>

    private void EncyclopediaInput()
    {
        if (!Input.GetKeyDown(KeyCode.Q)) return;

        if (currentMenu == null || currentMenu == encyclopedia.gameObject || !currentMenu.activeSelf)
        {
            currentMenu = encyclopedia.EnableMenu(!encyclopedia.gameObject.activeSelf, submarineBody) ? encyclopedia.gameObject : null;
        }
    }

    private void UpgradeTreeInput()
    {
        if (!Input.GetKeyDown(KeyCode.Tab)) return;

        if (currentMenu == null || currentMenu == upgradeTreeCanvas.gameObject || !currentMenu.activeSelf)
        {
            currentMenu = upgradeTreeCanvas.EnableMenu(!upgradeTreeCanvas.gameObject.activeSelf, submarineBody) ? upgradeTreeCanvas.gameObject : null;
        }
    }


    //<<
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
        dyingEvent.OnDie(transform.position);
    }

    




    private void LCStressCalculation(float depth)
    {
        stress = (((1000f + (depth / 11000f * 50f)) * 9.81f * depth * radiusOfHull) / (2f * thicknessOfHull)) / 101325f;

        if (stress > 100)
        {
            float diff = stress - 100f;
            health.DealDamage((diff / maxStressTreshold) * health.MaxHealth * Time.fixedDeltaTime * stressDamageModifier);
            warningText.gameObject.SetActive(true);
            warningInstance.setParameterByName("shouldPlay", 1);
        }
        else
        {
            warningText.gameObject.SetActive(false);
            warningInstance.setParameterByName("shouldPlay", 0);
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
        LevelVolume.Remove_OnCurrentVolumeChanged(OnLevelVolumeChanged);
        health.Remove_OnDie(Die);
        health.Remove_OnValueChanged(UpdateCracksOnWindshield);

        // Janko >>
        warningInstance.stop(STOP_MODE.ALLOWFADEOUT);
        // Janko <<
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
    private void OnLevelVolumeChanged()
    {
        zoneText.text = LevelVolume.Current ? LevelVolume.Current.ZoneName : string.Empty;
    }
    // IDepthDependant
    public bool IDD_OnDepthLevelEnter(int level)
    {
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

    public SubmarineMovement getSubmarineMovement()
    {
        return movement;
    }

    public Health getSubmarineHealth()
    {
        return health;
    }
    private void OnGUI()
    {
        // Shows the stress on screen
        //GUI.Label(new Rect(1000, 10, 500, 100), string.Format("LC Stress: {0}", stress), InternalSettings.Get.DebugStyle);
    }
}