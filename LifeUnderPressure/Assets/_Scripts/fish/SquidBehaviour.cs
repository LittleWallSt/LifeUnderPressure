using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquidBehaviour : BoidUnit
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

    private Vector3 initialPosition; 

    private void Start()
    {
        averageSpeed = speed;

        initialPosition = transform.position;

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
        downTimer = upTimer - UnityEngine.Random.Range(0.5f, 1.3f);
    }

    public override void MoveFish()
    {
        if (path == null || path.Length == 0) return;

        if (assignedBoid.currWayPointIndex > 2)
        {
            assignedBoid.SetNextWaypoint(0);
        }
        Transform targetWaypoint = path.GetWaypoint(assignedBoid.currWayPointIndex);
        directionToWaypoint = Vector3.zero;

        FindNeighbours();
        CalculateAverageSpeed();

        squidTimer += Time.deltaTime;

        if (goingUp)
        {
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
        else
        {
            squidVel = -0.7f; 
        }

        directionToWaypoint = (targetWaypoint.position - transform.position).normalized;

        Vector3 moveVector = new Vector3(0, squidVel, 0);

        Vector3 newPosition = new Vector3(initialPosition.x, transform.position.y, initialPosition.z);

        newPosition += moveVector * speed * Time.deltaTime;

        transform.position = newPosition;

        if (transform.position.y > maxHeight)
        {
            goingUp = false; 
            if (assignedBoid != null) assignedBoid.SetNextWaypoint();
        }
        else if (transform.position.y < minHeight)
        {
            goingUp = true; 
            if (assignedBoid != null) assignedBoid.SetNextWaypoint();
        }
    }
}
