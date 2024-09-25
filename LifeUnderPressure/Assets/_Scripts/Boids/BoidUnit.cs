using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoidUnit : Fish
{
    // Field of view. The units should not detect other units behind them
    [SerializeField] private float FOVAngle;
    // The lower this value is the closer we can get to the cohesion vector. Therefore it will rotate faster 
    [SerializeField] private float smoothDamp;

    private List<BoidUnit> cohesionNeighbours = new List<BoidUnit>();
    private List<BoidUnit> avoidanceNeighbours = new List<BoidUnit>();
    private List<BoidUnit> aligementNeighbours = new List<BoidUnit>();
    private Vector3 currentVelocity;

    private bool canBeNeighbour = false;

    private BoidManager assignedBoid;

    public Transform myTransform { get; set; }

    // Javi >>
    private void Start()
    {
        averageSpeed = speed;

        if (assignedBoid != null)
        {
            path = assignedBoid.path;
            currentWaypointIndex = assignedBoid.currWayPointIndex;
        }
        player = Submarine.Instance.transform;
        layer = 1 >> mask;
    }
    // << Javi 
    private void Awake()
    {
        myTransform = transform;
    }


    public void InitializeSpeed(float speed)
    {
        this.speed = speed;
    }
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
    public void AssignBoid(BoidManager boid)
    {
        assignedBoid = boid;
    }
    #region FishOverrides
    public override void MoveFish()
    {
        //If there are no waypoints
        if (path == null)
            return;
        if (path.Length == 0)
            return;

        Transform targetWaypoint = path.GetWaypoint(assignedBoid.currWayPointIndex);
        directionToWaypoint = Vector3.zero;

        FindNeighbours();
        CalculateAverageSpeed();

        FishBehaviour();
        // Path following behaviour
        if (!isCurious && !isScared)
        {
            directionToWaypoint = (targetWaypoint.position - transform.position).normalized;
        }

        // Cooldown for the fish to be curious
        if (curiousCooldown)
        {
            cooldownTimer += Time.deltaTime;
            if (cooldownTimer > curiousCooldownTime)
            {
                cooldownTimer = 0f;
                curiousCooldown = false;
            }
        }

        Vector3 cohesionVector = CalculateCohesionVector() * assignedBoid.cohesionWeight;
        Vector3 avoidanceVector = CalculateAvoidanceVector() * assignedBoid.avoidanceWeight;
        Vector3 aligementVector =  CalculateAligementVector() * assignedBoid.aligementWeight;

        Vector3 moveVector = cohesionVector + avoidanceVector + aligementVector + directionToWaypoint;
        moveVector = Vector3.SmoothDamp(myTransform.forward, moveVector, ref currentVelocity, smoothDamp);
        moveVector = moveVector.normalized * speed;
        if (moveVector == Vector3.zero)
            moveVector = transform.forward;

        myTransform.forward = moveVector;
        myTransform.position += moveVector * Time.deltaTime;

        if (Vector3.Distance(transform.position, targetWaypoint.position) < path.Radius)
        {
            if (assignedBoid != null) assignedBoid.SetNextWaypoint();

        }
    }

    // Javi from Paulas code in fish>>
    protected override void FishBehaviour()
    {
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

        else if (!isCurious && speed < averageSpeed)
        {
            speed *= 1.1f;
        }
        else if (!isScared && speed > averageSpeed)
        {
            speed *= 0.99f;
        }
        else
        {
            canBeNeighbour = true;
        }
    }
    #endregion
    // << Javi from Paulas code in fish
    private void FindNeighbours()
    {
        cohesionNeighbours.Clear();
        avoidanceNeighbours.Clear();
        aligementNeighbours.Clear();
        var allUnits = assignedBoid.allUnits;
        for (int i = 0; i < allUnits.Length; i++)
        {
            var currentUnit = allUnits[i];
            if (currentUnit != this && !isCurious && !isScared && canBeNeighbour)
            {
                float currentNeighbourDistanceSqr = Vector3.SqrMagnitude(currentUnit.myTransform.position - myTransform.position);
                if (currentNeighbourDistanceSqr <= assignedBoid.cohesionDistance * assignedBoid.cohesionDistance)
                {
                    cohesionNeighbours.Add(currentUnit);
                }
                if (currentNeighbourDistanceSqr <= assignedBoid.avoidanceDistance * assignedBoid.avoidanceDistance)
                {
                    avoidanceNeighbours.Add(currentUnit);
                }
                if (currentNeighbourDistanceSqr <= assignedBoid.aligementDistance * assignedBoid.aligementDistance)
                {
                    aligementNeighbours.Add(currentUnit);
                }
            }
        }
    }

    // Calculate speed based on the neighbour 
    private void CalculateAverageSpeed()
    {
        if (cohesionNeighbours.Count == 0)
            return;
        speed = 0;
        for (int i = 0; i < cohesionNeighbours.Count; i++)
        {
            speed += cohesionNeighbours[i].speed;
        }

        // Average speed
        speed /= cohesionNeighbours.Count;
        speed = Mathf.Clamp(speed, assignedBoid.minSpeed, assignedBoid.maxSpeed);

        averageSpeed = speed;
    }

    // Average position of all units in a certain radius
    private Vector3 CalculateCohesionVector()
    {
        var cohesionVector = Vector3.zero;
        if (cohesionNeighbours.Count == 0)
            return Vector3.zero;
        int neighboursInFOV = 0;
        for (int i = 0; i < cohesionNeighbours.Count; i++)
        {
            if (IsInFOV(cohesionNeighbours[i].myTransform.position))
            {
                neighboursInFOV++;
                cohesionVector += cohesionNeighbours[i].myTransform.position;
            }
        }

        cohesionVector /= neighboursInFOV;
        // We have to substract our pos bc cohesionVec rn is an average world pos and
        // we want to convert it to a local position
        cohesionVector -= myTransform.position;
        cohesionVector = cohesionVector.normalized;
        return cohesionVector;
    }

    // Calculates the average heading
    private Vector3 CalculateAligementVector()
    {
        var aligementVector = myTransform.forward;
        if (aligementNeighbours.Count == 0)
            return myTransform.forward;
        int neighboursInFOV = 0;
        for (int i = 0; i < aligementNeighbours.Count; i++)
        {
            if (IsInFOV(aligementNeighbours[i].myTransform.position))
            {
                neighboursInFOV++;
                aligementVector += aligementNeighbours[i].myTransform.forward;
            }
        }

        // Average aligment vector
        aligementVector /= neighboursInFOV;
        aligementVector = aligementVector.normalized;
        return aligementVector;
    }

    // Calculates the direction in wich can avoid the crowding local flockmates
    private Vector3 CalculateAvoidanceVector()
    {
        var avoidanceVector = Vector3.zero;
        if (aligementNeighbours.Count == 0)
            return Vector3.zero;
        int neighboursInFOV = 0;
        for (int i = 0; i < avoidanceNeighbours.Count; i++)
        {
            if (IsInFOV(avoidanceNeighbours[i].myTransform.position))
            {
                neighboursInFOV++;
                avoidanceVector += (myTransform.position - avoidanceNeighbours[i].myTransform.position);
            }
        }

        // Average avoidance vector
        avoidanceVector /= neighboursInFOV;
        avoidanceVector = avoidanceVector.normalized;
        return avoidanceVector;
    }

    // This method will return true if an angle between our forward dir an the dir to the neighbour
    // pos is less than the FOV angle we declared in the inspector
    private bool IsInFOV(Vector3 position)
    {
        return Vector3.Angle(myTransform.forward, position - myTransform.position) <= FOVAngle;
    }
}
