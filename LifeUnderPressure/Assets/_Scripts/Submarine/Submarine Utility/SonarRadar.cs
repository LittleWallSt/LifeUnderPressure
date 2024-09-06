using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonarRadar : MonoBehaviour
{
    [Header("Calibration values")]
    [SerializeField] float tickTime = 4f;
    [SerializeField] int fishLayer = 8;

    [SerializeField] float[] maxDistanceRadius;
    int upgradeLevel;
    float timeLeft;

    [SerializeField] GameObject scanArea;
    LayerMask fishLayerMask;


    public Action<float, Vector3> sonarBeep;

    private void Start()
    {
        fishLayerMask = (1 << fishLayer);
        if (maxDistanceRadius.Length>0) 
            ChangeMaxDistance(maxDistanceRadius[0]);
    }

    private void Update()
    {
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0.0f)
        {
            timeLeft = tickTime;


            Collider[] fishes = fishInArea();

            if (fishes.Length > 0)
            {
                foreach (Collider fish in fishes)
                {
                    sonarBeep.Invoke(DistPercentage(fish), fishDirection(fish));
                }
            }
        }
    }
    

    private Collider[] fishInArea()
    {
        return Physics.OverlapBox(scanArea.transform.position, scanArea.transform.localScale / 2,
            Quaternion.identity, fishLayerMask);
    }

    private void ChangeMaxDistance(float maxDistRad)
    {
        scanArea.transform.localScale = new Vector3(maxDistRad*2, maxDistRad*2, maxDistRad*2);
    }


    public float DistPercentage(Collider fish)
    {
        float dist = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z),
            new Vector3(fish.transform.position.x, 0, fish.transform.position.z)); 

        if (maxDistanceRadius.Length == 0) return 1;
        return dist / maxDistanceRadius[maxDistanceRadius.Length - 1];

    }

    public Vector3 fishDirection(Collider fish)
    {
        return (fish.transform.position - transform.position).normalized;
    }
    
}
