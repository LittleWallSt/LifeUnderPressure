using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using Unity.VisualScripting;
using UnityEngine;

public struct CuriousInfo
{
    public float speed;
    public float distance;
    public float factor;
    public float timer;
    public float coolDown;
    public float range;
}

public struct ScareInfo
{
    public float speed;
    public float distance;
    public float factor;
    public float timer;
}


public class BoidManager: MonoBehaviour, IDistanceLoad
{
    [Header("Spawn Setup")]
    [SerializeField] private bool isSquid = false; // maybe not necesary, not in this script lol 
    [SerializeField] private BoidUnit boidUnitPrefab;
    [SerializeField] private int boidSize;
    [SerializeField] private float zoneRadius;
    [SerializeField, Range(0.1f, 1.0f)] private float minScale;
    [SerializeField, Range(1.0f, 2.0f)] private float maxScale;

    [Header("Path to follow")]
    [SerializeField] private Path _path;
    public Path path { get { return _path; } }

    private int _currentWaypointIndex = 0;
    public int currWayPointIndex { get { return _currentWaypointIndex;  } }

    [Header("Speed Setup")]
    [Range(0, 8)]
    [SerializeField] private float _minSpeed;
    public float minSpeed { get { return _minSpeed; } }
    [Range(0, 8)]
    [SerializeField] private float _maxSpeed;
    public float maxSpeed { get { return _maxSpeed; } }


    /// <summary>
    ///  Avoidance: Steer to avoid crowding local flockmates
    ///  Alignment: Steer towards the average heading of local flockmates
    ///  Cohesion: Steer to move toward the avereage position of flockmates
    /// </summary>
    
    // Distances in which the boid unit will detect the neighbours in the 3 different steer behaviours
    [Header("Detection Distances")]

    [Range(0, 10)]
    [SerializeField] private float _cohesionDistance;
    public float cohesionDistance { get { return _cohesionDistance; } }

    [Range(0, 10)]
    [SerializeField] private float _avoidanceDistance;
    public float avoidanceDistance { get { return _avoidanceDistance; } }

    [Range(0, 10)]
    [SerializeField] private float _aligementDistance;
    public float aligementDistance { get { return _aligementDistance; } }
    
    // How much it will affect in the 3 steer behaviours
    [Header("Behaviour Weights")]

    [Range(0, 10)]
    [SerializeField] private float _cohesionWeight;
    public float cohesionWeight { get { return _cohesionWeight; } }

    [Range(0, 10)]
    [SerializeField] private float _avoidanceWeight;
    public float avoidanceWeight { get { return _avoidanceWeight; } }

    [Range(0, 10)]
    [SerializeField] private float _aligementWeight;
    public float aligementWeight { get { return _aligementWeight; } }

    public BoidUnit[] allUnits { get; set; }


    // Unit Behavoiur
    [Header("Scare Behaviour")]
    [Range(0, 100)]
    [SerializeField] int chanceScare;
    [Range(0,1)]
    [SerializeField] private float _scareFactor;
    [Range(0, 300)]
    [SerializeField] private float _scareDistance;
    [Range(0, 100)]
    [SerializeField] private float _scareSpeed;
    [Range(0, 100)]
    [SerializeField] private float _scaredTimer;

    [Header("Curious Behaviour")]
    [Range(0, 100)]
    [SerializeField] int chanceCurious;
    [Range(0, 1)]
    [SerializeField] private float _curiousFactor;
    [Range(0, 300)]
    [SerializeField] private float _curiousDistance;
    [Range(0, 100)]
    [SerializeField] private float _curiousSpeed;
    [Range(0, 100)]
    [SerializeField] private float _curiousTimer;
    [Range(0, 100)]
    [SerializeField] private float _curiousCoolDownTimer;
    [Range(0, 100)]
    [SerializeField] private float _curiousRange;

    private bool working;

    private int counter = 0;

    [SerializeField]    
    private int fishToUpdate = 20;
    [SerializeField, Range(0f, 0.5f)]
    private float tickRate = 0.05f;

    // Javi from Alexis's code >>
    #region IDL
    public void IDL_AssignToGameManager()
    {
        GameManager.Instance.AssignIDL(this);
    }

    public void IDL_OffDistance()
    {
        if (working)
        {
            working = false;
            Debug.Log("off dista");
            foreach (BoidUnit fish in allUnits)
            {
                fish.gameObject.SetActive(false);
            }
        }
    }

    public void IDL_InDistance()
    {
        if (!working)
        {
            working = true;
            Debug.Log("in  dista");
            foreach (BoidUnit fish in allUnits)
            {
                fish.gameObject.SetActive(true);
            }
        }
    }

    public Vector3 IDL_GetPosition()
    {
        return transform.position;
    }
    #endregion
    // << Javi from Alexis's code

    private IEnumerator Start()
    {
        if (_path.Length > 0)
        {
            transform.position = _path.GetWaypoint(_currentWaypointIndex).position;
            SetNextWaypoint();
        }
        // We generate the boid units at the beginning
        GenerateUnits();
        yield return new WaitForSeconds(1);
        
        StartCoroutine(UpdateFrequency());
    }
    public void SetRandomWaypoint()
    {
        _currentWaypointIndex = Random.Range(0, _path.Length);
    }
    public void SetNextWaypoint()
    {
        _currentWaypointIndex = (_currentWaypointIndex + 1) % _path.Length;
    }
    public void SetNextWaypoint(int index)
    {
        // Need control
        _currentWaypointIndex = index;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawSphere(transform.position, zoneRadius);
    }

    #region tick
    
    private IEnumerator UpdateFrequency()
    {
        int nUpdates = 0;
        while (true)
        {
            nUpdates += fishToUpdate;
            for (int i = counter; i < allUnits.Length && i < nUpdates; i++)
            {
                allUnits[i].MoveFish();
            }
            counter = nUpdates;
            if(counter >= allUnits.Length - 1)
            {
                counter = 0;
                nUpdates = 0;
            }
            yield return new WaitForSeconds(tickRate);
        }
    }
    

    //private void Update()
    //{
    //    for (int i = 0; i < allUnits.Length; i++)
    //    {
    //        allUnits[i].MoveFish();
    //    }
    //}
    #endregion

    private void GenerateUnits()
    {
        allUnits = new BoidUnit[boidSize];
        for (int i = 0; i < boidSize; i++)
        {
            Vector3 spawnPos = Vector3.zero;
            if(!isSquid)
            {
                int checks = 0;
                spawnPos = transform.position + Random.insideUnitSphere * zoneRadius;
                Collider[] colls = Physics.OverlapSphere(spawnPos, 0.2f, InternalSettings.EnvironmentLayer);

                float terrainHeight = GameManager.Instance.GetTerrainHeight(spawnPos);
                if (spawnPos.y < terrainHeight) spawnPos.y = terrainHeight + 0.2f;

                while (colls.Length > 0 && checks < 50)
                {
                    spawnPos = transform.position + Random.insideUnitSphere * zoneRadius;
                    colls = Physics.OverlapSphere(spawnPos, 0.2f, InternalSettings.EnvironmentLayer);
                    checks++;
                }

            }
            else 
            {
                //// Random position generated inside the spawn bounds
                var randomVector = UnityEngine.Random.insideUnitSphere;
                randomVector = new Vector3(randomVector.x * zoneRadius, randomVector.y * zoneRadius, randomVector.z * zoneRadius);
                spawnPos = transform.position + randomVector;
            
            }
            // Boid unit rotated randomly
            var rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);
            // We instantiate and initialaze the unit. Also assigned its respective boid to the unit
            allUnits[i] = Instantiate(boidUnitPrefab, spawnPos, rotation);
            allUnits[i].gameObject.layer = LayerMask.NameToLayer("Fish");
            allUnits[i].gameObject.transform.localScale *= Random.Range(minScale, maxScale);
            allUnits[i].AssignBoid(this);
            allUnits[i].InitializeSpeed(UnityEngine.Random.Range(minSpeed, maxSpeed));
            allUnits[i].setGeneralFish(isSquid);

            int rand = Random.Range(0, 100);
            if(rand < chanceCurious)
            {
                CuriousInfo info = new CuriousInfo();
                info.speed = _curiousSpeed;
                info.distance = _curiousDistance;
                info.factor = _curiousFactor;
                info.timer = _curiousTimer;
                info.coolDown = _curiousCoolDownTimer;
                info.range = _curiousRange;

                allUnits[i].SetCuriousBehaviour(info);
            }
            else if(rand < chanceScare)
            {
                ScareInfo info = new ScareInfo();
                info.speed = _scareSpeed;
                info.distance = _scareDistance;
                info.factor = _scareFactor;
                info.timer = _scaredTimer;

                allUnits[i].SetScareBehaviour(info);
            }
        }
    }
}
