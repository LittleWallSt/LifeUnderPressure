using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class fish : MonoBehaviour
{
    //FISH PROPERTIES
    [SerializeField]
    protected float scanTime;
    
    [Range(0f, 1f)]
    [SerializeField]
    protected float scaredFactor;
    
    [Range(0f, 1f)]
    [SerializeField]
    protected float curiousFactor;

    [SerializeField]
    protected string fishInfo;

    protected bool curious;
    protected bool scared;

    //MOVEMENT WAYPOINT BASED
    [SerializeField]
    protected Transform[] waypoints; // Array of patrol points
    private int currentWaypointIndex = 0;

    [SerializeField]
    protected float speed;
    [SerializeField]
    protected float rotationSpeed;
    [SerializeField]
    protected float waypointRadius;

    private bool isTurning = false;
    [SerializeField]
    protected float turnRadius = 5.0f; 
    [SerializeField]
    protected float alignmentThreshold = 5.0f; 

    private Vector3 currentDirection;    

    // Start is called before the first frame update
    void Start()
    {
        if (waypoints.Length > 0)
        {
            transform.position = waypoints[currentWaypointIndex].position;
            SetNextWaypoint();
        }
    }

    #region Movement

    //TODO: add scared behaviour from player and curious behaviour
    //IDEAS: add a navmesh waypoint to the front of the submarine that overrides all other waypoints when the fish is curious, 
    //when the fish is scared add a waypoint in a place for it to hide and then return to it's original waypoint
    /// <summary>
    /// How the fish will move, basic movement is waypoint based
    /// </summary>
    public void movement() {
        //If there are no waypoints
        if (waypoints.Length == 0)
            return;

        Vector3 turnCenter = Vector3.zero;
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Vector3 directionToWaypoint = (targetWaypoint.position - transform.position).normalized;

        if (Vector3.Distance(transform.position, targetWaypoint.position) < turnRadius)
        {
            if (!isTurning)
            {
                isTurning = true;
                turnCenter = targetWaypoint.position + transform.forward * turnRadius;
            }
        }
        
        if (isTurning)
        {
            Turning(turnCenter, directionToWaypoint);
        }
        else
        {
            HeadTowards(directionToWaypoint, targetWaypoint);
        }
    }

    /// <summary>
    /// Sets the next waypoint in the chain
    /// </summary>
    protected void SetNextWaypoint()
    {
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
    }

    /// <summary>
    /// Turns in a smooth motion around the waypoint, higher speeds will make the turn radius bigger
    /// </summary>
    /// <param name="turnCenter">where to turn around</param>
    /// <param name="direction">The direction vector of the next waypoint to know when to stop turning</param>
    protected void Turning(Vector3 turnCenter, Vector3 direction)
    {
        Vector3 directionToTurnCenter = (turnCenter - transform.position).normalized;
        Vector3 turnDirection = Vector3.Cross(Vector3.up, directionToTurnCenter).normalized;

        //we slow down because if not it appears as if it´s going much faster than it should be
        transform.position += transform.forward * speed*0.7f * Time.deltaTime;

        transform.position = turnCenter + Quaternion.AngleAxis(rotationSpeed * Time.deltaTime * 360, Vector3.up) * (transform.position - turnCenter);
        transform.LookAt(transform.position + transform.forward);

        //When in an alignement threshold stop turning and go
        if (Vector3.Angle(transform.forward, direction) < alignmentThreshold)
        {
            isTurning = false;
            SetNextWaypoint();
        }
    }

    /// <summary>
    /// Target heads towards waypoint in a straightline until it reaches a certain distance threshhold
    /// </summary>
    /// <param name="direction">Direction where we are headed</param>
    /// <param name="target">Waypoints transform</param>
    protected void HeadTowards(Vector3 direction, Transform target)
    {
        transform.position += transform.forward * speed * Time.deltaTime;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < waypointRadius)
        {
            SetNextWaypoint();
        }
    }

    #endregion Movement

    /// <summary>
    /// The info of the scanned fish
    /// </summary>
    /// <returns>String containing the information of the fish</returns>
    public string scanInfo() {
        return fishInfo;
    }
    

    // Update is called once per frame
    void Update()
    {
        movement();
    }
}
