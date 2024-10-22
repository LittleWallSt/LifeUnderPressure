using System;
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
    [SerializeField] private Vector2 sunLightIntensityRange;
    [SerializeField] private Color gizmoColor;

    private List<IDepthDependant> itemsNotAllowed = new List<IDepthDependant>();

    public string ZoneName => zoneName;
    public float MaxFakeDepth => maxFakeDepth;
    public Vector2Int DepthRange => depthRange;
    public Vector2 SunLightIntensityRange => sunLightIntensityRange;
    public int Level => level;

    private static LevelVolume current;
    public static LevelVolume Current 
    {
        get
        {
            return current;
        }
        private set
        {
            if(Current != value)
            {
                current = value;
                Call_OnCurrentVolumeChanged();
            }
        }
    }
    private static List<LevelVolume> triggering = new List<LevelVolume>();

    private static Action onCurrentVolumeChanged;
    public static List<LevelVolume> List { get; private set; } = new List<LevelVolume>();
    private void OnValidate()
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;
        volumeCollider.center = new Vector3(0f, -(depthRange.x + depthRange.y) / 2f, 0f);
        volumeCollider.size = new Vector3(1000f, Mathf.Abs(depthRange.x - depthRange.y), 1000f);
        depthRange = new Vector2Int(Mathf.Max(0, depthRange.x), Mathf.Max(1, depthRange.y));
    }
    private void Awake()
    {
        List.Add(this);
    }
    private void OnTriggerEnter(Collider other)
    {
        IDepthDependant depthDependant = other.GetComponent<IDepthDependant>();
        if (depthDependant == null) return;

        if (depthDependant.IDD_GetGOInstanceID() == Submarine.Instance.gameObject.GetInstanceID())
        {
            triggering.Add(this);
            Current = triggering[triggering.Count - 1];
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

        if (other.gameObject.GetInstanceID() == Submarine.Instance.gameObject.GetInstanceID())
        {
            triggering.Remove(this);
            Current = triggering[triggering.Count - 1];
        }
        depthDependant.IDD_OnDepthLevelExit(level);
        if (itemsNotAllowed.Contains(depthDependant)) itemsNotAllowed.Remove(depthDependant);
    }
    public static string GetCurrentZoneName()
    {
        if (Cave.Inside) return "Cave";
        return Current != null ? Current.zoneName : string.Empty;
    }
    // Action
    public static void Assign_OnCurrentVolumeChanged(Action action)
    {
        onCurrentVolumeChanged += action;
    }
    public static void Remove_OnCurrentVolumeChanged(Action action)
    {
        onCurrentVolumeChanged -= action;
    }
    private static void Call_OnCurrentVolumeChanged()
    {
        if (onCurrentVolumeChanged != null) onCurrentVolumeChanged();
    }
    private void OnDestroy()
    {
        if (List.Contains(this)) List.Remove(this);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawCube(new Vector3(0f, -(depthRange.x + depthRange.y) / 2f, 0f), new Vector3(1000f, Mathf.Abs(depthRange.x - depthRange.y), 1000f));
    }
}
