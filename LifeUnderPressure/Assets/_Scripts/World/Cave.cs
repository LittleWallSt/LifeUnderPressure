using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cave : MonoBehaviour, IDistanceLoad
{
    [SerializeField] private float triggerStayUpdateTime = 1f;

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
            }
        }
    }
    private void Start()
    {
        foreach(var coll in GetComponents<Collider>())
        {
            position += coll.bounds.center;
        }
        position /= GetComponents<Collider>().Length;
    }
    private void Update()
    {
        updateTime += Time.fixedDeltaTime;
        if (updateTime < triggerStayUpdateTime) return;

        updateTime = 0f;
        Inside = GameManager.Instance.IsUnderground(Submarine.Instance.transform.position);
        Submarine.Instance.UpdateZoneText();
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
    public Vector3 IDL_GetPosition()
    {
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
