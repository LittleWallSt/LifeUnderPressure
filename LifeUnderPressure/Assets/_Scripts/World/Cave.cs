using System;
using System.Collections;
using UnityEngine;

public class Cave : MonoBehaviour, IDistanceLoad
{
    [SerializeField] private float updateTimer = 1f;
    [SerializeField] private float distanceLoadOffset = 20f;
    [SerializeField] private float collapseDelay = 3f;
    [SerializeField] private GameObject entrance = null;
    [SerializeField] private GameObject exit = null;
    [SerializeField] private Animator animator = null;

    private bool collapsed = false;
    private Vector3 position;

    private float updateTime = 0f;

    private static Action OnCaveInsideChanged;

    private static bool inside = false;
    public static bool Inside 
    {
        get => inside; 
        private set
        {
            if (inside != value)
            {
                inside = value;
                Call_OnInsideChanged();
            }
        }
    }
    private void Start()
    {
        IDL_AssignToGameManager();
        Assign_OnInsideChanged(UpdateInside);
        GameManager.Instance.Assign_OnDataLoaded(OnDataLoaded);
        SetPosition();
    }

    private void SetPosition()
    {
        foreach (var coll in GetComponents<Collider>())
        {
            position += coll.bounds.center;
        }
        position /= GetComponents<Collider>().Length;
    }

    private void OnDataLoaded()
    {
        collapsed = DataManager.Get("CaveCollapsed", 0) == 1 ? true : false;
        animator.SetFloat("Offset", collapsed ? 0.95f : 0);
        UpdateCollapsed();
        GameManager.Instance.Remove_OnDataLoaded(OnDataLoaded);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y)) // debug
        {
            animator.SetFloat("Offset", 0f);
            StartCoroutine(Collapse(collapseDelay));
        }

        updateTime += Time.fixedDeltaTime;
        if (updateTime < updateTimer) return;

        updateTime = 0f;
        Inside = GameManager.Instance.IsUnderground(Submarine.Instance.transform.position);
        Submarine.Instance.UpdateZoneText();
    }
    private void UpdateCollapsed()
    {
        entrance.gameObject.SetActive(collapsed);

        if (Inside) exit.gameObject.SetActive(!collapsed);
        else exit.gameObject.SetActive(collapsed);

        animator.SetBool("Collapsed", collapsed);
    }
    public IEnumerator Collapse(float delay)
    {
        yield return new WaitForSeconds(delay);

        collapsed = true;
        DataManager.Write("CaveCollapsed", 1);
        UpdateCollapsed();
    }
    private void UpdateInside()
    {
        if(!Inside && collapsed)
        {
            Debug.Log("outside when collapsed"); 
            exit.gameObject.SetActive(collapsed);
        }
    }
    private void OnDisable()
    {
        if(Inside && collapsed) // player died in cave
        {
            
        }
        Inside = GameManager.Instance.IsUnderground(Submarine.Instance.transform.position);
        Submarine.Instance.UpdateZoneText();
    }
    private void OnDestroy()
    {
        Remove_OnInsideChanged(UpdateInside);
    }
    // IDL
    public void IDL_OffDistance()
    {
        enabled = false;
    }
    public void IDL_InDistance()
    {
        enabled = true;
    }
    public void IDL_AssignToGameManager()
    {
        GameManager.Instance.AssignIDL(this);
    }
    public Vector3 IDL_GetPosition(out float distanceOffset)
    {
        distanceOffset = distanceLoadOffset;
        return position;
    }

    // Action
    public static void Assign_OnInsideChanged(Action action)
    {
        OnCaveInsideChanged += action;
    }
    public static void Call_OnInsideChanged()
    {
        if(OnCaveInsideChanged != null) OnCaveInsideChanged();
    }
    public static void Remove_OnInsideChanged(Action action)
    {
        OnCaveInsideChanged -= action;
    }
}
