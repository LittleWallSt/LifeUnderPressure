using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderwaterCurrent : MonoBehaviour, IDistanceLoad
{
    [SerializeField] private CapsuleCollider coll = null;
    [SerializeField] private float strength = 6f;

    private Vector3 startPos;
    private Vector3 endPos;

    private Vector3 direction;
    public Vector3 Direction => direction;
    public float Strength => strength;
    public float Radius => coll.radius;

    private static List<UnderwaterCurrent> triggering = new List<UnderwaterCurrent>();
    public static List<UnderwaterCurrent> Triggering
    {
        get => new List<UnderwaterCurrent>(triggering);
        private set
        {
            if(value.Count != triggering.Count)
            {
                triggering = value;
            }
        }
    }
    private void OnValidate()
    {
        Setup();
    }
    private void Start()
    {
        Setup();
        IDL_AssignToGameManager();
    }
    private void Setup()
    {
        direction = transform.forward;
        startPos = transform.position + ((-direction * coll.height) / 2f);
        endPos = transform.position + ((direction * coll.height) / 2f);
    }
    public Vector3 GetPointOnCurrent(Vector3 pos)
    {
        Vector3 rq = endPos - startPos;
        float scalarMagnitude = (Vector3.Dot(rq, startPos - pos) / Vector3.Dot(rq, endPos - startPos));
        return startPos - (scalarMagnitude * rq);
    }
    private void OnTriggerEnter(Collider other)
    {
        Submarine submarine = other.GetComponent<Submarine>();
        if (submarine = null) return;

        List<UnderwaterCurrent> list = new List<UnderwaterCurrent>(triggering);
        list.Add(this);
        Triggering = list;
    }
    private void OnTriggerExit(Collider other)
    {
        Submarine submarine = other.GetComponent<Submarine>();
        if (submarine = null) return;

        List<UnderwaterCurrent> list = new List<UnderwaterCurrent>(triggering);
        list.Remove(this);
        Triggering = list;
    }
    private void OnDrawGizmos()
    {
        Quaternion rotation = Quaternion.LookRotation(Direction);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(startPos + rotation * new Vector3(0f, Radius, 0f), endPos + rotation * new Vector3(0f, Radius, 0f));
        Gizmos.DrawLine(startPos + rotation * new Vector3(0f, -Radius, 0f), endPos +  rotation * new Vector3(0f, -Radius, 0f));
        Gizmos.DrawLine(startPos + rotation * new Vector3(Radius, 0f, 0f), endPos + rotation * new Vector3(Radius, 0f, 0f));
        Gizmos.DrawLine(startPos + rotation * new Vector3(-Radius, 0f, 0f), endPos + rotation * new Vector3(-Radius, 0f, 0f));
    }

    // Distance load
    public void IDL_OffDistance()
    {
        gameObject.SetActive(false);
    }

    public void IDL_InDistance()
    {
        gameObject.SetActive(true);
    }

    public void IDL_AssignToGameManager()
    {
        GameManager.Instance?.AssignIDL(this);
    }

    public Vector3 IDL_GetPosition()
    {
        return transform.position;
    }
}
