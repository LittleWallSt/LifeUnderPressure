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
    [SerializeField] private TMP_Text heightText = null;
    [SerializeField] private TMP_Text warningText = null;
    [SerializeField] private UpgradeCanvas upgradeCanvas = null;

    private List<SubmarineUpgrade> upgrades = new List<SubmarineUpgrade>();

    private Health health;
    private SubmarineMovement movement;

    private double stress = 0f;
    private float inDeepTime = 0f;
    private void Awake()
    {
        movement = GetComponent<SubmarineMovement>();
        health = GetComponent<Health>();
        warningText.gameObject.SetActive(false);
        upgradeCanvas.gameObject.SetActive(false);
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
        stress = (((1000f + (depth / 11000f * 50f)) * 9.81f * depth * radiusOfHull) / (2f * thicknessOfHull)) / 101325f;
        
        if (heightText) heightText.text = string.Format("Depth: {0:F1}", depth);
    }
    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Tab)) return;

        upgradeCanvas.gameObject.SetActive(!upgradeCanvas.gameObject.activeSelf);
        InternalSettings.EnableCursor(upgradeCanvas.gameObject.activeSelf);
    }
    private void Die()
    {
        Debug.Log("Submarine died");
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        movement.ResetMovement();
        health.ResetHealth();
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
        GUI.Label(new Rect(1000, 10, 500, 100), string.Format("Stress: {0}", stress), InternalSettings.Get.DebugStyle);
    }
}
