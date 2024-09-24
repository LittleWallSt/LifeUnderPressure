using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquidBehaviour : BoidUnit // Boid Unit????
{
    [Range(0, 2)][SerializeField] private float upTimer;
    [Range(0, 2)][SerializeField] private float downTimer;
    [SerializeField] private float squidVel = 1.0f;

    private float squidTimer = 0.0f;

    private bool deceleration = false;
    private bool goingUp = true;

    private float maxHeight;
    private float minHeight;

    private void Start()
    {
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
    }
    // Update is called once per frame
    public override void MoveFish()
    {
        //If there are no waypoints
        if (path == null)
            return;
        if (path.Length == 0)
            return;

        if (assignedBoid.currWayPointIndex > 2)
        {
            assignedBoid.SetNextWaypoint(0);
        }
        Transform targetWaypoint = path.GetWaypoint(assignedBoid.currWayPointIndex);
        directionToWaypoint = Vector3.zero;

        FindNeighbours();
        CalculateAverageSpeed();

        Vector3 squidMovement = Vector3.zero;

        // Going up
        if (goingUp)
        {
            squidTimer += Time.deltaTime;
            if(!deceleration)
            {
                speed *= 1.1f; 
                if(squidTimer > upTimer)
                {
                    squidTimer = 0.0f;
                    deceleration = true;
                }
            }
            else
            {
                speed *= 0.99f;
                if (squidTimer > downTimer)
                {
                    squidTimer = 0.0f;
                    deceleration = false;
                }
            }

            if (transform.position.y > maxHeight) goingUp = false;
        }
        // Going down
        else
        {
            squidVel *= 0.99f;
        }

        var cohesionVector = CalculateCohesionVector() * assignedBoid.cohesionWeight;
        var avoidanceVector = CalculateAvoidanceVector() * assignedBoid.avoidanceWeight;
        var aligementVector = CalculateAligementVector() * assignedBoid.aligementWeight;

        var moveVector = cohesionVector + avoidanceVector + aligementVector + directionToWaypoint;
        moveVector = Vector3.SmoothDamp(myTransform.up, moveVector, ref currentVelocity, smoothDamp);
        moveVector = moveVector.normalized * speed;
        if (moveVector == Vector3.zero)
            moveVector = transform.up;

        myTransform.up = moveVector;
        myTransform.position += moveVector * Time.deltaTime;

        if (Vector3.Distance(transform.position, targetWaypoint.position) < path.Radius)
        {
            if (assignedBoid != null) assignedBoid.SetNextWaypoint();

        }


        if (transform.position.y > maxHeight) goingUp = false; else goingUp = true;
    }
}
