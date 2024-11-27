using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(SubmarineMovement))]
[RequireComponent(typeof(Health))]
public class Submarine : MonoBehaviour, IDepthDependant
{
    // Deprecated
    [HideInInspector][SerializeField] private float inDeepMaxTime = 10f;
    [HideInInspector][SerializeField] private float deepOffset = 0f;
    [HideInInspector][SerializeField] private UpgradeCanvas upgradeCanvas = null;
    [HideInInspector][SerializeField] private TMP_Text heightText = null;
    [HideInInspector][SerializeField] private TMP_Text warningText = null;
    [HideInInspector][SerializeField] private TMP_Text dockText = null;

    private static readonly int WarningHash = Animator.StringToHash("Warning");

    [Header("Submarine Parameters")]
    [SerializeField] private float radiusOfHull = 10f;
    [SerializeField] private float thicknessOfHull = 10f;
    [SerializeField] private float maxStressTreshold = 24f;
    [SerializeField] private float stressDamageModifier = 0.25f;

    private int money = 0;
    private float stress = 0f;
    private float inDeepTime = 0f;
    private bool docked = false;
    private float currDepth = 0f;

    [Header("Dependencies")]
    [SerializeField] private UpgradeTreeCanvas upgradeTreeCanvas = null;
    [SerializeField] private PauseMenu pauseMenu = null;
    [SerializeField] private Encyclopedia encyclopedia = null;
    [SerializeField] private Light sun = null;

    [Header("Windshield Cracks")]
    [SerializeField] private Material cracksMaterial = null;
    [SerializeField] private float crackLevel1 = 0.25f;
    [SerializeField] private float crackLevel2 = 0.50f;
    [SerializeField] private float crackLevel3 = 0.75f;

    private Material cracksMaterialInstance = null;

    [Header("Depth Meter")]
    [SerializeField] private Material depthMeterMaterial = null;
    [SerializeField] private MeshRenderer depthMeterMeshRenderer = null;

    private Material depthMeterMaterialInstance = null;

    [Header("UI")]
    [SerializeField] private TMP_Text zoneText = null;

    private GameObject currentMenu = null;

    [Header("Submarine Components")]
    [SerializeField] private GameObject submarineBody;
    [SerializeField] private MeshRenderer submarineMeshRenderer = null;
    [SerializeField] private Animator redlightAnimator = null;
    [SerializeField] private DyingEvent dyingEvent = null;

    private List<SubmarineUpgrade> upgrades = new List<SubmarineUpgrade>();
    private Health health;
    private SubmarineMovement movement;
    private Rigidbody rb;

    [Header("Audio")]
    [SerializeField] private Voiceline[] Collision = null;
    [SerializeField] private Voiceline[] TooDeep = null;

    [SerializeField] private EventReference cookieSHark;

    private EventInstance warningInstance;

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
        // Singleton initialization
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        DepthMeterMaterialSetup();
        CracksMaterialSetup();
        AssignSubmarineComponents();
    }
    private void AssignSubmarineComponents()
    {
        movement = GetComponent<SubmarineMovement>();
        health = GetComponent<Health>();
        rb = GetComponent<Rigidbody>();
        if (!movement || !health || !rb) throw new System.Exception("CRUCIAL COMPONENT MISSING");
    }
    private void DepthMeterMaterialSetup()
    {
        depthMeterMaterialInstance = Instantiate(depthMeterMaterial);

        UpdateDepthMeterMaterial(false);
        List<Material> mats = new List<Material>(depthMeterMeshRenderer.materials);
        mats[1] = depthMeterMaterialInstance;
        depthMeterMeshRenderer.SetMaterials(mats);
    }
    private void CracksMaterialSetup()
    {
        cracksMaterialInstance = Instantiate(cracksMaterial);

        ResetCracksOnWindshield();

        List<Material> mats = new List<Material>(submarineMeshRenderer.materials);
        mats[1] = cracksMaterialInstance;
        submarineMeshRenderer.SetMaterials(mats);
    }
    private void UpdateDepthMeterMaterial(bool warningOn)
    {
        depthMeterMaterialInstance.SetInt("_On", warningOn ? 1 : 0);
    }
    private void UpdateCracksOnWindshield(float value)
    {
        if (value >= health.MaxHealth)
        {
            ResetCracksOnWindshield();
            return;
        }
        float fraction = Mathf.Abs(1f - (value / health.MaxHealth));

        bool cracked = false;
        if (cracksMaterialInstance.GetFloat("_Cracks1") < 1f)
        {
            cracked = fraction > crackLevel1;
            cracksMaterialInstance.SetFloat("_Cracks1", cracked ? 1f : 0f);
        }
        else if (cracksMaterialInstance.GetFloat("_Cracks2") < 1f)
        {
            cracked = fraction > crackLevel2;
            cracksMaterialInstance.SetFloat("_Cracks2", cracked ? 1f : 0f);
        }
        else if (cracksMaterialInstance.GetFloat("_Cracks3") < 1f)
        {
            cracked = fraction > crackLevel3;
            cracksMaterialInstance.SetFloat("_Cracks3", cracked ? 1f : 0f);
        }
        if (cracked) AudioManager.instance.PlayOneShot(FMODEvents.instance.SFX_Cracking, transform.position);
    }
    private void ResetCracksOnWindshield()
    {
        cracksMaterialInstance.SetFloat("_Cracks1", 0f);
        cracksMaterialInstance.SetFloat("_Cracks2", 0f);
        cracksMaterialInstance.SetFloat("_Cracks3", 0f);
    }
    private void Start()
    {
        currDepth = -transform.position.y;
        // Janko >>
        getSubmarineHealth().Assign_OnDie(OnDie);
        getSubmarineHealth().Assign_OnRespawn(OnRespawn);

        warningInstance = AudioManager.instance.CreateInstance(FMODEvents.instance.SFX_Warning);
        warningInstance.setParameterByName("shouldPlay", 0);
        warningInstance.start();
        // Janko <<

        LevelVolume.Assign_OnCurrentVolumeChanged(OnLevelVolumeChanged);
    }

    public void Init(Vector3 position, Vector3 euler)
    {
        warningText.gameObject.SetActive(false);
        if (upgradeCanvas != null) upgradeCanvas.gameObject.SetActive(false);
        if (pauseMenu != null) pauseMenu.EnableMenu(false);
        // EnableDockText(false); // deprecated
        inDeepTime = 0f;

        health.Assign_OnDie(Die);
        health.Assign_OnValueChanged(UpdateCracksOnWindshield);

        foreach(SubmarineUpgrade upgrade in GetComponents<SubmarineUpgrade>())
        {
            upgrades.Add(upgrade);
            upgrade.Init(this, movement);
        }

        DataManager.Assign_OnSaveData(StorePositionData);
        Money = DataManager.Get("Money", 0);

        //Vector3 spawnPosition = GameManager.Instance ? GameManager.Instance.InitialSpawnPoint : Vector3.zero;
        //spawnPosition.x = DataManager.Get("SpawnPositionX", (int)GameManager.Instance.InitialSpawnPoint.x);
        //spawnPosition.y = DataManager.Get("SpawnPositionY", (int)GameManager.Instance.InitialSpawnPoint.y);
        //spawnPosition.z = DataManager.Get("SpawnPositionZ", (int)GameManager.Instance.InitialSpawnPoint.z);
        rb.position = position;

        rb.rotation = Quaternion.Euler(euler);

        AudioManager.instance.AssignSubmarineEvents(this);
    }
    private void FixedUpdate()
    {
        LevelVolume current = LevelVolume.Current;
        float depth = -transform.position.y;

        LerpSunIntensity(current, depth);

        depth = FakeDepth(current);

        currDepth = depth;

        LCStressCalculation(-transform.position.y);

        // Deprecated
        //if (heightText)
        //{
        //    heightText.text = string.Format("{0:F1}m", depth);
        //}
    }
    private void LerpSunIntensity(LevelVolume current, float depth)
    {
        if (sun && current)
        {
            sun.intensity = Mathf.Lerp(current.SunLightIntensityRange.x, current.SunLightIntensityRange.y, (depth - current.DepthRange.x) / (current.DepthRange.y - current.DepthRange.x));
        }
    }

    private float FakeDepth(LevelVolume current)
    {
        float depth = -transform.position.y;
        if (current)
        {
            depth = ((depth - current.DepthRange.x) / (current.DepthRange.y - current.DepthRange.x)) * current.MaxFakeDepth;
            for(int i = 0; i < current.Level; i++)
            {
                depth += LevelVolume.List.Find(x => x.Level == i).MaxFakeDepth;
            }
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

    private void Die(Vector3 direction, DamageType damageType)
    {
        dyingEvent.OnDie(transform.position, direction, damageType);
    }

    private void LCStressCalculation(float depth)
    {
        stress = (((1000f + (depth / 11000f * 50f)) * 9.81f * depth * radiusOfHull) / (2f * thicknessOfHull)) / 101325f;

        bool warning = false;
        if (!Cave.Inside && stress > 100)
        {
            warning = true;
            health.DealDamage(((stress - 100f) / maxStressTreshold) * health.MaxHealth * Time.fixedDeltaTime * stressDamageModifier, Vector3.down, DamageType.Depth);
            if (TooDeep != null && TooDeep.Length > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, TooDeep.Length);
                VoicelinesUI.Instance?.CallCDVoiceline(TooDeep[randomIndex], 6f); 
            }
        }
        UpdateDepthMeterMaterial(warning);
        EnableRedlights(warning);
    }

    public void DamageSubmarine(float damage, DamageType damageType)
    {
        health.DealDamage(damage * health.MaxHealth * Time.fixedDeltaTime, Vector3.zero, damageType);
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
    public void UpdateZoneText()
    {
        if (zoneText.text != LevelVolume.GetCurrentZoneName())
        {
            zoneText.gameObject.SetActive(false);
            zoneText.enabled = true;
            zoneText.gameObject.SetActive(true);
        }
        zoneText.text = LevelVolume.GetCurrentZoneName();
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
        health.Remove_OnValueChanged(UpdateCracksOnWindshield);
        health.Remove_OnDie(Die);
        // Janko >>
        warningInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT); 

        getSubmarineHealth().Remove_OnDie(OnDie);
        getSubmarineHealth().Remove_OnRespawn(OnRespawn);
        // Janko <<
    }
    // Events
    private void OnLevelVolumeChanged()
    {
        UpdateZoneText();
    }
        // Janko >>
    private void OnDie(Vector3 direction, DamageType type)
    {
        warningInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    private void OnRespawn()
    {
        warningInstance.start();
    }
        // Janko <<
    // Setters
    public void ForceSetPosition(Vector3 pos)
    {
        rb.position = pos;
    }
    public void ForceSetEuler(Vector3 euler)
    {
        rb.rotation = Quaternion.Euler(euler);
    }
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
    public void EnableRedlights(bool state)
    {
        redlightAnimator.SetBool(WarningHash, state);
        warningInstance.setParameterByName("shouldPlay", state ? 1 : 0);
    }
    public void SetThicknessOfHull(float newThickness)
    {
        thicknessOfHull = newThickness;
    }
    public void AddMoney(int amount)
    {
        Money += amount;
    }
    // IDepthDependant [deprecated]
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
            Die(Vector3.down, health.GetLastGamageType());
        }
    }

    public void IDD_OnDepthLevelExit(int level)
    {

    }
    public int IDD_GetGOInstanceID()
    {
        return gameObject.GetInstanceID();
    }

    // Getters
    public SubmarineMovement getSubmarineMovement()
    {
        return movement;
    }

    public Health getSubmarineHealth()
    {
        return health;
    }

    public Encyclopedia GetEncyclopedia()
    {
        return encyclopedia;
    }

    public float GetSubmarineDepth()
    {
        return currDepth;
    }

    public Voiceline[] getCollisionVoicelines()
    {
        return Collision;
    }

    public EventReference getCookieVoicelines()
    {
        return cookieSHark;
    }
}