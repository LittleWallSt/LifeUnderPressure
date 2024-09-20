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

     protected float scaredFactor;
    protected float scaredDistance;
     protected float scaredSpeed;
    protected float scaredTimer;
    
     protected float curiousFactor;
    protected float curiousDistance;
   protected float curiousSpeed;
    protected float curiousTimer;
   protected float curiousCooldownTime;
   protected float curiousRange;

    protected bool curious;
    protected bool scared;

    //MOVEMENT WAYPOINT BASED
    //[Header("Movement properties")]
    protected Path path;
    protected int currentWaypointIndex;

    private Transform player;

    protected float speed;
    
    [SerializeField] protected float rotationSpeed;

    [SerializeField] protected float alignmentThreshold; 

    private Vector3 currentDirection;

    [SerializeField] protected Vector3 avoidanceDetection;

    //private float currentSpeed;
    protected float averageSpeed;

    protected bool isScared = false;
    protected bool isCurious = false;
    protected bool curiousCooldown = false;

    protected bool canBeNeighbour = false;

    private float timer = 0f;
    protected float cooldownTimer = 0f;
    protected Boid assignedBoid;
    protected Vector3 directionToWaypoint = Vector3.zero;

    public void SetCuriousBehaviour(CuriousInfo info)
    {
        curiousSpeed = info.speed;
        curiousFactor = info.factor;
        curiousDistance = info.distance;
        curiousTimer = info.timer;
        curiousCooldownTime = info.coolDown;
        curiousRange = info.range;

        isCurious = true;
    }

    public void SetScareBehaviour(ScareInfo info)
    {
        scaredFactor = info.factor;
        scaredDistance = info.distance;
        scaredSpeed = info.speed;
        scaredTimer = info.timer;

        isScared = true;
        Debug.Log("Aaaah I'm scared");

    }
    public void AssignBoid(Boid boid)
    {
        assignedBoid = boid;
    }

    // Start is called before the first frame update
    void Start()
    {
        //currentSpeed = speed;
        averageSpeed = speed;

        if(assignedBoid != null)
        {
            path = assignedBoid.path;
            currentWaypointIndex = assignedBoid.currWayPointIndex;
        }

        //if (path.Length > 0)
        //{
        //    transform.position = path.GetWaypoint(currentWaypointIndex).position;
        //    SetNextWaypoint();
        //}

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
    virtual public void MoveFish() {
        //If there are no waypoints
        if (path == null)
            return;
        if (path.Length == 0)
            return;

        Transform targetWaypoint = path.GetWaypoint(currentWaypointIndex);
        directionToWaypoint = Vector3.zero;

        FishBehaviour(); // Can be better lol
        
        // Path following behaviour
        if(!curious && !scared)
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
            if(assignedBoid != null) assignedBoid.SetNextWaypoint();

        }
    }

    /// <summary>
    /// Sets the next waypoint in the chain
    /// </summary>
    //protected void SetNextWaypoint()
    //{
    //    currentWaypointIndex = (currentWaypointIndex + 1) %path.Length;
    //}

    protected void FishBehaviour()
    {
        Debug.Log("Scared factor: " + scaredFactor);
        // scared behaviour
        if ((scaredFactor > 0.75f && Vector3.Distance(transform.position, player.position) < scaredDistance)
            || isScared)
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
            canBeNeighbour = false;
        }
        //Curious behaviour
        else if (((curiousFactor > 0.75f && Vector3.Distance(transform.position, player.position) < curiousDistance)
            || isCurious) && !curiousCooldown)
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
                if (speed < 0.1f) { Debug.Log("Fixing Speed"); speed = 0.4f; }

                if (speed < curiousSpeed) { Debug.Log("AAAAAAAAAAAAAAAAAAAAAcceleration"); speed *= 1.1f; }

                if (speed > curiousSpeed) { Debug.Log("Curious speed"); speed = curiousSpeed; }

            }
            else if (dist > curiousRange)
            {
                speed *= 0.99f;
                Debug.Log("Stopping");

            }
            else speed = 0.1f;

            timer += Time.deltaTime;
            if (timer > curiousTimer)
            {
                Debug.Log("Not curious anymore");
                isCurious = false;
                curiousCooldown = true;
                speed *= 1.1f;
                timer = 0.0f;
            }
            canBeNeighbour = false;

        }

        else if( !isCurious && speed < averageSpeed )
        {
            speed *= 1.1f;
        }
        else if(!isScared && speed > averageSpeed)
        {
            speed *= 0.99f;
        }
        else
        {
            canBeNeighbour = true;
        }
    }
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
