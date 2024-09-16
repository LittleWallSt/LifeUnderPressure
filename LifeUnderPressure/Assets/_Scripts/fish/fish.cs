using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class Fish : MonoBehaviour
{
    //FISH PROPERTIES
    [Header("Fish properties")]
    [SerializeField] protected string fishName;

    [SerializeField] protected string fishInfo;

    [SerializeField] protected float scanTime;

    [Header("Fish Behaviour")]
    [Range(0f, 1f)]
    [SerializeField] protected float scaredFactor;
    [SerializeField] protected float scaredDistance;
    [SerializeField, Range(0, 300f)] protected float scaredSpeed;
    [SerializeField] protected float scaredTimer;
    
    [Range(0f, 1f)]
    [SerializeField] protected float curiousFactor;
    [SerializeField] protected float curiousDistance;
    [SerializeField, Range(0f, 300f)] protected float curiousSpeed;
    [SerializeField] protected float curiousTimer;
    [SerializeField] protected float curiousCooldownTime;
    [SerializeField] protected float curiousRange;

    protected bool curious;
    protected bool scared;

    //MOVEMENT WAYPOINT BASED
    [Header("Movement properties")]
    [SerializeField] private Path path;
    private int currentWaypointIndex = 0;

    private Transform player;

    [SerializeField] protected float speed;
    
    [SerializeField] protected float rotationSpeed;

    [SerializeField] protected float alignmentThreshold; 

    private Vector3 currentDirection;

    [SerializeField] protected Vector3 avoidanceDetection;

    private float currentSpeed;

    private bool isScared = false;
    private bool isCurious = false;
    private bool curiousCooldown = false;

    private float timer = 0f;
    private float cooldownTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        currentSpeed = speed;
        if (path.Length > 0)
        {
            transform.position = path.GetWaypoint(currentWaypointIndex).position;
            SetNextWaypoint();
        }

        // Aleksis >>
        player = Submarine.Instance.transform;
        // Aleksis <<
    }

    public void SetPath(Path newPath) {
        path = newPath;
    }

    public void SetRandomWaypoint()
    {
        currentWaypointIndex = Random.Range(0, path.Length);
    }

    #region Movement

    //TODO: add scared behaviour from player and curious behaviour
    //IDEAS: add waypoint to the front of the submarine that overrides all other waypoints when the fish is curious, 
    //when the fish is scared add a waypoint in a place for it to hide and then return to it's original waypoint
    /// <summary>
    /// How the fish will move, basic movement is waypoint based
    /// </summary>
    public void movement() {
        //If there are no waypoints
        if (path == null)
            return;
        if (path.Length == 0)
            return;

        Transform targetWaypoint = path.GetWaypoint(currentWaypointIndex);
        Vector3 directionToWaypoint = Vector3.zero;
        
        // scared behaviour
        if ((scaredFactor > 0.75f && Vector3.Distance(transform.position, player.position) < scaredDistance) 
            || isScared)
        {
            directionToWaypoint = (transform.position - player.position).normalized;
            if (!isScared)
            {
                isScared = true;
                timer = 0f;
            }

            if(currentSpeed < scaredSpeed)
                currentSpeed *= 1.1f;
            else currentSpeed = scaredSpeed;
            

            timer += Time.deltaTime;
            if(timer> scaredTimer)
            {
                isScared = false;
                timer = 0f;
                currentSpeed = speed;
            }
            
        }
        //Curious behaviour
        else if(((curiousFactor > 0.75f && Vector3.Distance(transform.position, player.position)< curiousDistance)
            || isCurious) && !curiousCooldown){
            
            directionToWaypoint = (player.position - transform.position).normalized;
            if (!isCurious)
            {
                isCurious = true;
            }

            float dist = Vector3.Distance(transform.position, player.position);
            if (dist > curiousRange)
            {
                if(curiousSpeed < 0.1f) curiousSpeed = 0.4f;
                if (currentSpeed < curiousSpeed)
                    currentSpeed *= 1.1f;
                if(currentSpeed > curiousSpeed) currentSpeed = curiousSpeed;
            }
            else
            {
                currentSpeed *= 0.99f;
            }

            timer += Time.deltaTime;
            if(timer > curiousTimer)
            {
                isCurious = false;
                curiousCooldown = true;
                currentSpeed = speed;
            }
        }
        // Path following behaviour
        else
        {
            directionToWaypoint = (targetWaypoint.position - transform.position).normalized;
        }

        // Cooldown for the fish to be curious
        if (curiousCooldown)
        {
            cooldownTimer += Time.deltaTime;
            if(cooldownTimer > curiousCooldownTime)
            {
                cooldownTimer = 0f;
                curiousCooldown = false;
            }
        }

        ObstacleAvoidance(ref directionToWaypoint);
        HeadTowards(directionToWaypoint);

        if (Vector3.Distance(transform.position, targetWaypoint.position) < path.Radius)
            SetNextWaypoint();
    }

    /// <summary>
    /// Sets the next waypoint in the chain
    /// </summary>
    protected void SetNextWaypoint()
    {
        currentWaypointIndex = (currentWaypointIndex + 1) %path.Length;
    }

   
     /// <summary>
    /// Target heads towards waypoint in a straightline until it reaches a certain distance threshhold
    /// </summary>
    /// <param name="direction">Direction where we are headed</param>
    /// <param name="target">Waypoints transform</param>
    protected void HeadTowards(Vector3 direction)
    {
        transform.position += transform.forward * currentSpeed * Time.deltaTime;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    // TODO: add layers
    /// <summary>
    /// Target avoids obstacles with three raycasts that tell it where obstacles are
    /// </summary>
    /// <param name="directionToWaypoint"></param>
    protected void ObstacleAvoidance(ref Vector3 directionToWaypoint)
    {
        RaycastHit hit;

        // Obstacle in front
        if (Physics.Raycast(transform.position, transform.forward, out hit, avoidanceDetection.y))
        {
            Debug.DrawRay(transform.position, transform.forward * hit.distance, Color.red);
            directionToWaypoint += transform.up;
        }

        // Obstacle in right of fish
        if (Physics.Raycast(transform.position, transform.right, out hit, avoidanceDetection.x))
        {
            Debug.DrawRay(transform.position, transform.right * hit.distance, Color.red);
            directionToWaypoint -= transform.right;
        }
        // Obstacle in left of fish
        if (Physics.Raycast(transform.position, -transform.right, out hit, avoidanceDetection.z))
        {
            Debug.DrawRay(transform.position, -transform.right * hit.distance, Color.red);
            directionToWaypoint += transform.right;
        }
        
    }

    #endregion Movement
   
    #region FishInfo
    /// <summary>
    /// The name of the fish
    /// </summary>
    /// <returns>String containing fish name</returns>
    public string getFishName()
    {
        return fishName;
    }

    /// <summary>
    /// The info of the scanned fish
    /// </summary>
    /// <returns>String containing the information of the fish</returns>
    public string scanInfo() {
        return fishInfo;
    }
    #endregion

    // Update is called once per frame
    void Update()
    {
        movement();
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, -transform.right * avoidanceDetection.z, Color.yellow);
        Debug.DrawRay(transform.position, transform.right * avoidanceDetection.x, Color.yellow);
        Debug.DrawRay(transform.position, transform.forward * avoidanceDetection.y, Color.yellow);
    }
}
