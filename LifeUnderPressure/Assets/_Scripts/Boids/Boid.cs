using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid: MonoBehaviour
{
    [Header("Spawn Setup")]
    [SerializeField] private BoidUnit boidUnitPrefab;
    [SerializeField] private int boidSize;
    [SerializeField] private Vector3 spawnBounds;

    [Header("Speed Setup")]
    [Range(0, 20)]
    [SerializeField] private float _minSpeed;
    public float minSpeed { get { return _minSpeed; } }
    [Range(0, 20)]
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

    private void Start()
    {
        // We generate the boid units at the beginning
        GenerateUnits();
    }

    private void Update()
    {
        // Moves all the boid units
        for (int i = 0; i < allUnits.Length; i++)
        {
            allUnits[i].MoveUnit();
        }
    }

    private void GenerateUnits()
    {
        allUnits = new BoidUnit[boidSize];
        for (int i = 0; i < boidSize; i++)
        {
            // Random position generated inside the spawn bounds
            var randomVector = UnityEngine.Random.insideUnitSphere;
            randomVector = new Vector3(randomVector.x * spawnBounds.x, randomVector.y * spawnBounds.y, randomVector.z * spawnBounds.z);
            var spawnPosition = transform.position + randomVector;
            // Boid unit rotated randomly
            var rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);
            // We instantiate and initialaze the unit. Also assigned its respective boid to the unit
            allUnits[i] = Instantiate(boidUnitPrefab, spawnPosition, rotation);
            allUnits[i].AssignBoid(this);
            allUnits[i].InitializeSpeed(UnityEngine.Random.Range(minSpeed, maxSpeed));
        }
    }
}
