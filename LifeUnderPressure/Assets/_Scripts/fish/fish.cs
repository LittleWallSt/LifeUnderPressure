using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Fish : MonoBehaviour
{
    //FISH PROPERTIES
    [Header("Fish properties")]
    // Aleksis >> changed from string to FishInfo
    [SerializeField] protected FishInfo fishInfo;
    public FishInfo FishInfo => fishInfo;

    [Header("Fish Behaviour")]
    [SerializeField, Range(0.0f, 1.0f)] protected float scaredFactor;
    [SerializeField] protected float scaredDistance;
    [SerializeField] protected float scaredSpeed;
    [SerializeField] protected float scaredTimer;

    [SerializeField, Range(0.0f, 1.0f)] protected float curiousFactor;
    [SerializeField] protected float curiousDistance;
    [SerializeField] protected float curiousSpeed;
    [SerializeField] protected float curiousTimer;
    [SerializeField] protected float curiousCooldownTime;
    [SerializeField] protected float curiousRange;


    //MOVEMENT WAYPOINT BASED
    protected Path path;
    protected int currentWaypointIndex;

    protected Transform player;
    [Header("Movement properties")]
    [SerializeField] protected float speed;
    
    [SerializeField] protected float rotationSpeed;

    [SerializeField] protected float alignmentThreshold; 

    private Vector3 currentDirection;

    [SerializeField] protected Vector3 avoidanceDetection;
    [SerializeField] protected LayerMask mask;
    protected int layer = 0;
    
    protected float averageSpeed;

    protected bool isScared = false;
    protected bool isCurious = false;
    protected bool curiousCooldown = false;


    protected float timer = 0f;
    protected float cooldownTimer = 0f;
    protected Vector3 directionToWaypoint = Vector3.zero;

    

    // Start is called before the first frame update
    void Start()
    {
        //Initial speed for fish, in boid it is the average speed of all the fish group
        averageSpeed = speed;

        if (path.Length > 0)
        {
            SetNextWaypoint();
        }

        // Aleksis >>
        player = Submarine.Instance.transform;
        // Aleksis <<

        layer = 1 >> mask;
    }

    public void SetPath(Path newPath) {
        path = newPath;
    }

    public void SetRandomWaypoint()
    {
      currentWaypointIndex = Random.Range(0, path.Length);
    }

    #region Movement

    /// <summary>
    /// How the fish will move, basic movement is waypoint based
    /// </summary>
    virtual public void MoveFish() {
        //If there are no waypoints
        if (path == null)
            return;
        if (path.Length == 0)
            return;

        Transform targetWaypoint = path.GetWaypoint(currentWaypointIndex);
        directionToWaypoint = Vector3.zero;

        FishBehaviour();
        
        // Path following behaviour
        if(!isCurious && !isScared)
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
        {
            SetNextWaypoint();
        }
    }

    /// <summary>
    /// Sets the next waypoint in the chain
    /// </summary>
    protected void SetNextWaypoint()
    {
        currentWaypointIndex = (currentWaypointIndex + 1) % path.Length;
    }

    #region FishScaredCurious
    /// <summary>
    /// Changes the direction vector to head in the opposite direction of the player
    /// </summary>
    private void Scared()
    {
        directionToWaypoint = (transform.position - player.position).normalized;
        if (!isScared)
        {
            Debug.Log("Aaaah I'm scared");
            isScared = true;
            timer = 0f;
        }

        if (speed < scaredSpeed)
            speed *= 1.1f;
        else speed = scaredSpeed;


        timer += Time.deltaTime;
        if (timer > scaredTimer)
        {
            isScared = false;
            timer = 0f;
            speed *= 0.99f;
            Debug.Log("Not scared anymore >:D");
        }
    }

    /// <summary>
    /// Changes the vector to head towards to face the player and head in that direction
    /// </summary>
    private void Curious()
    {
        directionToWaypoint = (player.position - transform.position).normalized;
        if (!isCurious)
        {
            isCurious = true;
        }

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > curiousRange * 2)
        {
            Debug.Log("Going to range. Dist: " + dist);
            if (speed < 0.1f) 
                speed = 0.4f;

            if (speed < curiousSpeed) 
                speed *= 1.1f; 

            if (speed > curiousSpeed)  
                speed = curiousSpeed; 

        }
        
        else if (dist > curiousRange)
            speed *= 0.99f;
        
        else 
            speed = 0.1f;

        timer += Time.deltaTime;
        if (timer > curiousTimer)
        {
            Debug.Log("Not curious anymore");
            isCurious = false;
            curiousCooldown = true;
            speed *= 1.1f;
            timer = 0.0f;
        }
    }
    virtual protected void FishBehaviour()
    {
        // scared behaviour
        if ((scaredFactor > 0.75f && Vector3.Distance(transform.position, player.position) < scaredDistance)
            || isScared)
            Scared();
        
        //Curious behaviour
        else if (((curiousFactor > 0.75f && Vector3.Distance(transform.position, player.position) < curiousDistance)
            || isCurious) && !curiousCooldown)
            Curious();

        else if( !isCurious && speed < averageSpeed )
        {
            speed *= 1.1f;
        }
        else if(!isScared && speed > averageSpeed)
        {
            speed *= 0.99f;
        }
    }
    #endregion
    /// <summary>
    /// Target heads towards waypoint in a straightline until it reaches a certain distance threshhold
    /// </summary>
    /// <param name="direction">Direction where we are headed</param>
    /// <param name="target">Waypoints transform</param>
    protected void HeadTowards(Vector3 direction)
    {
        transform.position += transform.forward * speed * Time.deltaTime;

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
        if (Physics.Raycast(transform.position, transform.forward, out hit, avoidanceDetection.y, layer))
        {
            Debug.DrawRay(transform.position, transform.forward * hit.distance, Color.red);
            directionToWaypoint += transform.up;
        }

        // Obstacle in right of fish
        if (Physics.Raycast(transform.position, transform.right, out hit, avoidanceDetection.x,layer))
        {
            Debug.DrawRay(transform.position, transform.right * hit.distance, Color.red);
            directionToWaypoint -= transform.right;
        }
        // Obstacle in left of fish
        if (Physics.Raycast(transform.position, -transform.right, out hit, avoidanceDetection.z, layer))
        {
            Debug.DrawRay(transform.position, -transform.right * hit.distance, Color.red);
            directionToWaypoint += transform.right;
        }
        
    }

    #endregion Movement
   
    // Update is called once per frame
    void Update()
    {
        MoveFish();
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, -transform.right * avoidanceDetection.z, Color.yellow);
        Debug.DrawRay(transform.position, transform.right * avoidanceDetection.x, Color.yellow);
        Debug.DrawRay(transform.position, transform.forward * avoidanceDetection.y, Color.yellow);
    }
}
