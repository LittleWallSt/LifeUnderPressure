using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquidBehaviour : BoidUnit // Boid Unit????
{
    [Range(1, 6)][SerializeField] private float minUpTimer;
    [Range(1, 6)][SerializeField] private float maxUpTimer;
    [SerializeField] private float squidVel = 1.0f;

    private float upTimer = 0.0f;
    private float downTimer = 0.0f;

    private float squidTimer = 0.0f;

    private bool deceleration = false;
    private bool goingUp = true;

    private float maxHeight;
    private float minHeight;

    private void Start()
    {
        averageSpeed = speed;

        if (assignedBoid != null)
        {
            path = assignedBoid.path;
            currentWaypointIndex = assignedBoid.currWayPointIndex;
        }
        if (path.GetWaypoint(0).position.y > path.GetWaypoint(1).position.y)
        {
            maxHeight = path.GetWaypoint(0).position.y;
            minHeight = path.GetWaypoint(1).position.y;
        }
        else
        {
            maxHeight = path.GetWaypoint(1).position.y;
            minHeight = path.GetWaypoint(0).position.y;
        }

        upTimer = UnityEngine.Random.Range(minUpTimer, maxUpTimer);
        if (upTimer <= 1.8f && upTimer >= 1.0f) downTimer = 0.8f;
        else downTimer = upTimer - UnityEngine.Random.Range(0.5f, 1.3f);
    }
    // Update is called once per frame
    public override void MoveFish()
    {
        //If there are no waypoints
        if (path == null)
            return;
        if (path.Length == 0)
            return;


        //Only 2 waypoints  for the min and max height
        if (assignedBoid.currWayPointIndex > 2)
        {
            assignedBoid.SetNextWaypoint(0);
        }
        Transform targetWaypoint = path.GetWaypoint(assignedBoid.currWayPointIndex);
        directionToWaypoint = Vector3.zero;

        FindNeighbours();
        CalculateAverageSpeed();
        
        // Going up
        if (goingUp)
        {
            Debug.Log("Im going UP");

            squidTimer += Time.deltaTime;
            if (!deceleration)
            {
                squidVel = 1.1f;
                if (squidTimer > upTimer)
                {
                    squidTimer = 0.0f;
                    deceleration = true;
                    squidVel = 1.0f;
                }
            }
            else
            {
                squidVel = -0.5f;
                if (squidTimer > downTimer)
                {
                    squidTimer = 0.0f;
                    squidVel = 1.0f;
                    deceleration = false;
                }
            }

        }
        // Going down
        else
        {
            squidVel = -0.7f;
            Debug.Log("Im going DOWN");

        }

        directionToWaypoint = (targetWaypoint.position - transform.position).normalized;

        var cohesionVector = CalculateCohesionVector() * assignedBoid.cohesionWeight;
        var avoidanceVector = CalculateAvoidanceVector() * assignedBoid.avoidanceWeight;
        var aligementVector = CalculateAligementVector() * assignedBoid.aligementWeight;

        var moveVector = cohesionVector + avoidanceVector + aligementVector + directionToWaypoint;
        //moveVector = Vector3.SmoothDamp(myTransform.up, moveVector, ref currentVelocity, smoothDamp);
        moveVector = moveVector.normalized * speed;

        // If negative, we want it to be positive going up
        if(moveVector.y < 0 && goingUp ) moveVector.y = -moveVector.y * squidVel;
        else if(moveVector.y < 0 && !goingUp) moveVector.y = -moveVector.y * squidVel;
        else moveVector.y *= squidVel;
        if (moveVector == Vector3.zero)
            moveVector = transform.up;

        Debug.Log("Move vector: " + moveVector);
        myTransform.up = moveVector;
        myTransform.position += moveVector * Time.deltaTime;

        //if (Vector3.Distance(transform.position, targetWaypoint.position) < path.Radius)
        //{
        //    if (assignedBoid != null) assignedBoid.SetNextWaypoint();

        //}


        if (transform.position.y > maxHeight)
        {
            goingUp = false;
            Debug.Log(goingUp);
            if (assignedBoid != null) assignedBoid.SetNextWaypoint();
        }
        else if (transform.position.y < minHeight)
        {
            goingUp = true;
            Debug.Log(goingUp);
            if (assignedBoid != null) assignedBoid.SetNextWaypoint();

        }

    }
}
