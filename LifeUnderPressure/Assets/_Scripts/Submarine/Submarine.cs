using TMPro;
using UnityEngine;

[RequireComponent(typeof(SubmarineMovement))]
[RequireComponent(typeof(Health))]
public class Submarine : MonoBehaviour, IDepthDependant
{
    [SerializeField] private int submarineLevel = 0;
    [SerializeField] private float inDeepMaxTime = 10f;
    [SerializeField] private float deepOffset = 0f;
    [SerializeField] private TMP_Text heightText = null;
    [SerializeField] private TMP_Text warningText = null;

    private Health health;
    private SubmarineMovement movement;

    private float inDeepTime = 0f;
    private void Awake()
    {
        movement = GetComponent<SubmarineMovement>();
        health = GetComponent<Health>();
        warningText.gameObject.SetActive(false);
        inDeepTime = 0f;

        health.Assign_OnDie(Die);
    }
    private void FixedUpdate()
    {
        float depth = -transform.position.y;
        if (heightText) heightText.text = string.Format("Depth: {0:F1}", depth);
    }
    private void Die()
    {
        Debug.Log("Submarine died");
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        movement.ResetMovement();
    }

    // IDepthDependant
    public bool IDD_OnDepthLevelEnter(int level)
    {
        bool allowed = submarineLevel >= level;
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
}
