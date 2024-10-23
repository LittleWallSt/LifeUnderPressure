using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookieSharkBehaviour : BoidUnit
{
    [SerializeField] public float timeToGoAway = 5.0f;
    [SerializeField] public float minDist = 7.0f;
    [SerializeField] public float damageInterval = 0.5f;
    [SerializeField] public float movRangeToScareAway = 2.0f;
    [SerializeField] public float damage = 5.0f;
    // [SerializedField] Blabla playerGlass or maybe it's fine only with player instance

    private bool nomnomPlayer = true;
    private bool inFrontOfGlass = false;
    private float damageTimer = 0.0f;
    private float movAccumulatedTimer = 0.0f;
    private float lastRot;
    private float currentRot;
    private float cooldown = 10.0f;
    private float cooldownTimer = 0.0f;
    private bool approaching = false;

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
        if (!approaching && !inFrontOfGlass)
        {
            directionToWaypoint = (targetWaypoint.position - transform.position).normalized;
            ObstacleAvoidance(ref directionToWaypoint);
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

        //Vector3 cohesionVector = CalculateCohesionVector() * assignedBoid.cohesionWeight;
        //Vector3 avoidanceVector = CalculateAvoidanceVector() * assignedBoid.avoidanceWeight;
        //Vector3 aligementVector = CalculateAligementVector() * assignedBoid.aligementWeight;

        Vector3 moveVector = directionToWaypoint;
        moveVector = Vector3.SmoothDamp(myTransform.forward, moveVector, ref currentVelocity, smoothDamp);
        moveVector = moveVector.normalized;
        moveVector *= speed;


        if (moveVector == Vector3.zero)
            moveVector = transform.forward;

        myTransform.forward = moveVector;
        myTransform.position += moveVector * Time.deltaTime;

        if (Vector3.Distance(transform.position, targetWaypoint.position) < path.Radius)
        {
            if (assignedBoid != null) assignedBoid.SetNextWaypoint();

        }

    }
    protected override void FishBehaviour()
    {
        if(nomnomPlayer)
        {
            float dist = Vector3.Distance(transform.position, player.position);
            if( !inFrontOfGlass && dist < minDist )
            {
                //Debug.Log("Gonna ite u >:D");
                approaching = true;
                directionToWaypoint = (player.position - transform.position).normalized;
                if (speed > 0f)speed *= 0.99f;
                if (dist <= 1.5)
                {
                    lastRot = player.eulerAngles.y;
                    inFrontOfGlass = true;
                    approaching = false;
                }
            }
            else if(inFrontOfGlass)
            {
                //Debug.Log("I'm bitting u ^V-V^");
                PositionAndFacePlayer();
                DamagePlayer();
                DetectLateralMov();
            }
        }
        else
        {
            // Timer para poner nomnom a true a lo mejor
            GoAway();
            //Debug.Log666666666666666666666("I should go to waypoint");
        }
    }

    private void PositionAndFacePlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        myTransform.position = player.position + player.forward * 1.5f;

        myTransform.LookAt(player.position);
    }

    private void DetectLateralMov()
    {
        currentRot = player.eulerAngles.y;
        float latMovement = Mathf.Abs(currentRot - lastRot);

        //Debug.Log("Lateral mov: " + latMovement);
        if (latMovement > 0.003f /*&& latMovement <= movRangeToScareAway*/) // bonita
        {
            movAccumulatedTimer += Time.deltaTime;

        }
        else // feaAAAAAAAAAA
        {
            movAccumulatedTimer = 0f;
            //Debug.Log("nopnonpnopnonopnop");
        }
        //Debug.Log("Timer: " + movAccumulatedTimer);

        if (movAccumulatedTimer >= timeToGoAway)
        {
            inFrontOfGlass = false;
            nomnomPlayer = false;
           // Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA " + movAccumulatedTimer + "    " + timeToGoAway);
            movAccumulatedTimer = 0.0f;
        }
        lastRot = currentRot;
    }

    private void DamagePlayer()
    {
        // Not the best way but to try
        player.GetComponentInParent<Health>().DealDamage(0.5f * Time.fixedDeltaTime, DamageType.CookieShark);
    }

    private void GoAway()
    {
        cooldownTimer += Time.deltaTime;
        if(speed < initialSpeed)
        {
            speed *= 1.1f;
        }
        if (cooldownTimer >= cooldown)
        {
            nomnomPlayer = true;
            cooldownTimer = 0.0f;
        }
    }
}
