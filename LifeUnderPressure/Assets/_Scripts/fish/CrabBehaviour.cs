using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class CrabBehaviour : MonoBehaviour
{
    [SerializeField] public float changeWaypointDistance = 1.0f;
    [SerializeField] public float walkRadius = 10.0f;
    [SerializeField] public float lateralSpeed = 3.0f;
    [SerializeField] public float stopTimer = 2.0f;

    [Header("Path Accesibility")]
    [SerializeField] public float stuckCheckTimer = 2.0f;
    [SerializeField] public float minMoveDist = 0.2f;
    [SerializeField] public float minDistToEdge = 5.0f;

    [Header("Animations")]
    [SerializeField] public Animator animator;

    private NavMeshAgent agent;
    private bool stop = false;
    private Vector3 lastPos;
    private float stuckTimer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        //animator = GetComponent<Animator>();
        lastPos = transform.position;

        SetNewRandomDestination();
    }

    private void Update()
    {
        if(!stop)
        {

            if(!agent.pathPending && agent.remainingDistance < changeWaypointDistance)
            {
                StartCoroutine(WaitBeforeMoving());
                //SetNewRandomDestination();
            }
            else
            {
                CheckIfStuck();
                //if (!animator.enabled) animator.enabled = true;
                MoveForward();
                
            }
            animator.SetFloat("Speed", agent.speed);
        }
        else
        {
            animator.SetFloat("Speed", 0);
        }
    }

    private void CheckIfStuck()
    {
        stuckTimer += Time.deltaTime;

        if(stuckTimer >= stuckCheckTimer)
        {
            if(Vector3.Distance(transform.position, lastPos) < minMoveDist)
            {
                SetNewRandomDestination();
            }

            stuckTimer = 0.0f;
            lastPos = transform.position;
        }
    }

    IEnumerator WaitBeforeMoving()
    {
        stop = true;
        agent.isStopped = true;
        yield return new WaitForSeconds(stopTimer);

        agent.isStopped = false;
        SetNewRandomDestination();
        stop = false;
    }

    private void SetNewRandomDestination()
    {
        bool destFound = false;

        do
        {
            Vector3 dir = Random.insideUnitSphere * walkRadius;
            dir += transform.position;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(dir, out hit, walkRadius, NavMesh.AllAreas))
            {
                if(IsFarFromEdge(hit.position, minDistToEdge))
                {
                    if (agent.CalculatePath(hit.position, new NavMeshPath()))
                    {
                        dir = hit.position;
                        agent.SetDestination(dir);
                        destFound = true;
                    }
                }
            }
        }
        while (!destFound);
    }

    private bool IsFarFromEdge(Vector3 position, float minDistToEdge)
    {
        NavMeshHit edgeHit;

        if (NavMesh.Raycast(position, position + Vector3.forward * minDistToEdge, out edgeHit, NavMesh.AllAreas) ||
            NavMesh.Raycast(position, position + Vector3.back * minDistToEdge, out edgeHit, NavMesh.AllAreas) ||
            NavMesh.Raycast(position, position + Vector3.left * minDistToEdge, out edgeHit, NavMesh.AllAreas) ||
            NavMesh.Raycast(position, position + Vector3.right * minDistToEdge, out edgeHit, NavMesh.AllAreas))
        {
            return false;
        }

        return true;
    }

    void MoveForward()
    {

        Vector3 directionToTarget = agent.steeringTarget - transform.position;
        directionToTarget.y = 0;  
        directionToTarget.Normalize();

        if (directionToTarget != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * lateralSpeed);
        }
    }
}
