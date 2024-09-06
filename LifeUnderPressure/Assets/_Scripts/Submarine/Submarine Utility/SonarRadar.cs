using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonarRadar : MonoBehaviour
{
    [Header("Calibration values")]
    [SerializeField] float tickTime = 4f;
    [SerializeField] int fishLayer = 8;

    [SerializeField] float[] maxDistance;
    int upgradeLevel;
    float timeLeft;

    GameObject scanArea;
    LayerMask fishLayerMask;


    public Action<float, Vector3> sonarBeep;

    private void Start()
    {
        fishLayerMask = (1 << fishLayer);
    }

    private void Update()
    {
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0.0f)
        {
            timeLeft = tickTime;

            sonarBeep.Invoke(0.5f, Vector3.forward);

            /*Collider[] fishes = fishInArea();

            if (fishes.Length > 0)
            {
                foreach (Collider fish in fishes)
                {

                }
            }*/
        }
    }
    

    private Collider[] fishInArea()
    {
        return Physics.OverlapBox(scanArea.transform.position, scanArea.transform.localScale / 2,
            Quaternion.identity, fishLayerMask);
    }


    public float DistPercentage(Collider fish)
    {
        float dist = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z),
            new Vector3(fish.transform.position.x, 0, fish.transform.position.z)); 

        if (maxDistance.Length == 0) return 1;
        return dist / maxDistance[maxDistance.Length - 1];

    }

    public Vector3 fishDirection(Collider fish)
    {
        return (fish.transform.position - transform.position).normalized;
    }
    
}
