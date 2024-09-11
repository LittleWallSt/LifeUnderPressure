using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    [SerializeField]
    private float radius = 2f;

    [SerializeField]
    private Transform[] waypoints;

    public int Length { get { return waypoints.Length; } }
    public Transform GetWaypoint(int index) {  return waypoints[index]; }


    public float Radius { get { return radius; } }

    

    // Draws path in gizmos
    private void OnDrawGizmos()
    {
        for (int i = 0; i < waypoints.Length; i++) {
            Debug.DrawLine(waypoints[i].position, waypoints[(i+1)%waypoints.Length].position, Color.magenta);
        }
    }
}
