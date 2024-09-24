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
    [SerializeField] private UpgradeCanvas upgradeCanvas = null;
    [SerializeField] private PauseMenu pauseMenu = null;
    [SerializeField] private Encyclopedia encyclopedia = null;

    [SerializeField] private GameObject submarineBody;

    private GameObject currentMenu = null;
    private List<SubmarineUpgrade> upgrades = new List<SubmarineUpgrade>();

    private Health health;
    private SubmarineMovement movement;

    private float stress = 0f;
    private float inDeepTime = 0f;

    public static Submarine Instance { get; private set; } = null;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        movement = GetComponent<SubmarineMovement>();
        health = GetComponent<Health>();
        warningText.gameObject.SetActive(false);
        if (upgradeCanvas != null) upgradeCanvas.gameObject.SetActive(false);
        if (pauseMenu != null) pauseMenu.EnableMenu(false);
        inDeepTime = 0f;

        health.Assign_OnDie(Die);

        foreach(SubmarineUpgrade upgrade in GetComponents<SubmarineUpgrade>())
        {
            upgrades.Add(upgrade);
            upgrade.Init(this, movement);
        }
    }
    private void Start()
    {
        InternalSettings.EnableCursor(false);
    }
    public void SetThicknessOfHull(float newThickness)
    {
        thicknessOfHull = newThickness;
    }
    private void FixedUpdate()
    {
        float depth = -transform.position.y;
        LCStressCalculation(depth);

        if (heightText) heightText.text = string.Format("Depth: {0:F1}", depth);
    }

    private void Update()
    {
        PauseMenuInput();
        UpgradeCanvasInput();
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
        if (!Input.GetKeyDown(KeyCode.Tab)) return;

        if (currentMenu == null || currentMenu == upgradeCanvas.gameObject || !currentMenu.activeSelf)
        {
            currentMenu = upgradeCanvas.EnableMenu(!upgradeCanvas.gameObject.activeSelf) ? upgradeCanvas.gameObject : null;
        }
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
    // IDepthDependant
    public bool IDD_OnDepthLevelEnter(int level)
    {
        bool allowed = true;
        if (allowed)
        {
            inDeepTime = 0f;
            warningText.gameObject.SetActive(false);
        }
        else
        {
            warningText.gameObject.SetActive(true);
        }
        Debug.Log("entered level " + level);
        return allowed;
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
        GUI.Label(new Rect(1000, 10, 500, 100), string.Format("LC Stress: {0}", stress), InternalSettings.Get.DebugStyle);
    }
}
