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
    [SerializeField] public float approachSpeed = 7.0f;
    [SerializeField] private Animator animator;

    private bool nomnomPlayer = true;
    private bool inFrontOfGlass = false;
    private bool approaching = false;
    private float cooldown = 10.0f;
    //private float cooldownTimer = 0.0f;


    private float lastPlayerRotationY = 0.0f;                
    private int directionChanges = 0;                
    private bool rotatingRight = false;              
    private float freneticMovementThreshold = 1.0f;  
    private int maxDirectionChanges = 10;              
    private float timeToReset = 0.5f;                 
    private float freneticTimer = 0.0f;

    private bool hasReachedPosition = false;


    // >> Ulia change
    bool attached = false;
    // <<

    public override void MoveFish()
    {
        if (path == null || path.Length == 0) return;

        if (!approaching && !inFrontOfGlass)
        {
            Transform targetWaypoint = path.GetWaypoint(assignedBoid.currWayPointIndex);
            directionToWaypoint = (targetWaypoint.position - transform.position).normalized;
            ObstacleAvoidance(ref directionToWaypoint);
        }

        FishBehaviour();

        if (!inFrontOfGlass)
        {
            if (approaching)
            {
                smoothDamp = 0;
                if (speed > 7.0f) speed *= 1.01f;
            }
            else smoothDamp = 1;
            Vector3 moveVector = Vector3.SmoothDamp(myTransform.forward, directionToWaypoint, ref currentVelocity, smoothDamp);
            moveVector = moveVector.normalized * speed;
            myTransform.forward = moveVector;
            myTransform.position += moveVector * Time.deltaTime;
        }

        if (Vector3.Distance(transform.position, path.GetWaypoint(assignedBoid.currWayPointIndex).position) < path.Radius)
        {
            assignedBoid.SetNextWaypoint();
        }
    }

    protected override void FishBehaviour()
    {
        float dist = Vector3.Distance(transform.position, player.position);

        if (nomnomPlayer && !inFrontOfGlass)
        {
            if (dist < minDist)
            {
                approaching = true;
                directionToWaypoint = (player.position + player.forward * 1.5f - transform.position).normalized;

                Quaternion targetRotation = Quaternion.LookRotation(directionToWaypoint);
                myTransform.rotation = Quaternion.Slerp(myTransform.rotation, targetRotation, Time.deltaTime * 7.0f);

                if (dist <= 2.5f)
                {
                    inFrontOfGlass = true;
                    approaching = false;
                }
            }


        }
        else if (inFrontOfGlass)
        {
            PositionAndFacePlayer();
            if(!DetectFreneticMouseMovement())
            {
                DamagePlayer();
                //>> sound
                if (!attached)
                {
                    AudioManager.instance.PlayOneShot(Submarine.Instance.getCookieVoicelines(), 
                        Submarine.Instance.transform.position);
                    attached = true; 
                }
                //<<
            }
            else
            {
                attached = false;
                nomnomPlayer = false;
                inFrontOfGlass = false;
                directionChanges = 0;
                freneticTimer = 0.0f;
                myTransform.SetParent(null);
            }

        }
        else
        {
            attached = false;
            GoAway();
        }
        
        animator.SetBool("InFrontOfGlass", inFrontOfGlass);
    }

    private void PositionAndFacePlayer()
    {
        Vector3 fixedPositionInFrontOfPlayer = player.position + player.forward * 1.5f;
        float epsilon = 0.05f; 

        if (!hasReachedPosition) 
        {
            myTransform.position = Vector3.MoveTowards(myTransform.position, fixedPositionInFrontOfPlayer, Time.deltaTime * 7.0f);

            Vector3 directionToPlayer = (player.position - myTransform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            myTransform.rotation = Quaternion.Slerp(myTransform.rotation, targetRotation, Time.deltaTime * 7.0f);

            if (Vector3.Distance(myTransform.position, fixedPositionInFrontOfPlayer) <= epsilon)
            {
                hasReachedPosition = true; 
                myTransform.position = fixedPositionInFrontOfPlayer; 
                myTransform.rotation = Quaternion.LookRotation(player.position - myTransform.position); 
                myTransform.rotation *= Quaternion.Euler(-30, 0, 0); 
            }
        }
        else
        {
            myTransform.position = fixedPositionInFrontOfPlayer;
            myTransform.rotation = Quaternion.LookRotation(player.position - myTransform.position);
            myTransform.rotation *= Quaternion.Euler(-30, 0, 0);
            
        }
    }


    private bool DetectFreneticMouseMovement()
    {
        float currentRotationY = player.eulerAngles.y;
        float rotationChange = currentRotationY - lastPlayerRotationY;

        if (Mathf.Abs(rotationChange) > freneticMovementThreshold)
        {
            bool currentRotatingRight = rotationChange > 0;

            if (currentRotatingRight != rotatingRight)
            {
                directionChanges++;
                rotatingRight = currentRotatingRight;
                freneticTimer = 0.0f; 
            }
        }

        lastPlayerRotationY = currentRotationY;
        freneticTimer += Time.deltaTime;

        if (freneticTimer > timeToReset)
        {
            directionChanges = 0;
            freneticTimer = 0.0f;
        }
        return directionChanges >= maxDirectionChanges;
    }


    private void DamagePlayer()
    {
        Submarine.Instance.getSubmarineHealth().DealDamage(damage * Time.fixedDeltaTime, Vector3.zero, DamageType.CookieShark);
        if (Submarine.Instance.getSubmarineHealth().Value == 0) {
            attached = false;
            nomnomPlayer = false;
            inFrontOfGlass = false;
            directionChanges = 0;
            freneticTimer = 0.0f;
            myTransform.SetParent(null);  

            GoAway(); }
    }

private void GoAway()
    {
        cooldownTimer += Time.deltaTime;
        if (speed < initialSpeed)
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
