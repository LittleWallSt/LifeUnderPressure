using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelVolume : MonoBehaviour
{
    [Header("Do NOT Change Transform. Only through this script!")]
    [SerializeField] private int level = -1;
    [SerializeField] private string zoneName = "ZONENAME";
    [SerializeField] private float maxFakeDepth = 100f;
    [SerializeField] private BoxCollider volumeCollider = null;

    [SerializeField] private Vector2Int depthRange;
    [SerializeField] private Color gizmoColor;

    private List<IDepthDependant> itemsNotAllowed = new List<IDepthDependant>();

    public string ZoneName => zoneName;
    public float MaxFakeDepth => maxFakeDepth;
    public static LevelVolume Current { get; private set; } = null;
    private void OnValidate()
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;
        volumeCollider.center = new Vector3(0f, -(depthRange.x + depthRange.y) / 2f, 0f);
        volumeCollider.size = new Vector3(1000f, Mathf.Abs(depthRange.x - depthRange.y), 1000f);
    }
    private void OnTriggerEnter(Collider other)
    {
        IDepthDependant depthDependant = other.GetComponent<IDepthDependant>();
        if (depthDependant == null) return;

        if(depthDependant.IDD_GetGOInstanceID() == Submarine.Instance.gameObject.GetInstanceID())
        {
            Current = this;
        }

        bool allowed = depthDependant.IDD_OnDepthLevelEnter(level);
        if (!allowed) itemsNotAllowed.Add(depthDependant);
    }
    private void OnTriggerStay(Collider other)
    {
        foreach (IDepthDependant depthDependant in itemsNotAllowed)
        {
            if (depthDependant.IDD_GetGOInstanceID() == other.gameObject.GetInstanceID())
            {
                depthDependant.IDD_NotAllowedUpdate(level, Time.fixedDeltaTime);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        IDepthDependant depthDependant = other.GetComponent<IDepthDependant>();
        if (depthDependant == null) return;

        depthDependant.IDD_OnDepthLevelExit(level);

        if (itemsNotAllowed.Contains(depthDependant)) itemsNotAllowed.Remove(depthDependant);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawCube(new Vector3(0f, -(depthRange.x + depthRange.y) / 2f, 0f), new Vector3(1000f, Mathf.Abs(depthRange.x - depthRange.y), 1000f));
    }
}
