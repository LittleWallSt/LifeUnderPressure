using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodRays : MonoBehaviour
{
    [SerializeField] private Light sun = null;
    [SerializeField] private Vector3 offset;

    private void Start()
    {
        if (sun == null) gameObject.SetActive(false);
    }
    private void Update()
    {
        Vector3 submarinePosition = Submarine.Instance.transform.position;
        transform.position = new Vector3(submarinePosition.x + offset.x, 0f + offset.y, submarinePosition.z + offset.z);
        Vector3 direction = submarinePosition - transform.position;
        direction.Normalize();
        transform.up = sun.transform.forward;
    }
}
