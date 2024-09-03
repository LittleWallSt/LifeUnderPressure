using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class Scanner : MonoBehaviour
{
    [Header("Calibration values")]
    [SerializeField] float scanTimer = 3f;
    [SerializeField] float scanAnimationSpeed = 1.5f;
    [SerializeField] int fishLayer = 8;

    [Header("Borders")]
    [SerializeField] GameObject scanRect;
    [SerializeField] GameObject borders;
    [SerializeField] Transform pivotPoint; // pivot point for distance between fishes measurment


    LayerMask fishLayerMask;
    

    float timeLeft;
    float range;
    bool scanning;

    public Action<string, string> scanFinished;
    public Action<float> updateScanner;

    public Collider currentFish;

    private void Start()
    {
        fishLayerMask = (1 << fishLayer);
        range = borders.transform.localScale.z / 2;
        timeLeft = scanTimer;
        scanRect.SetActive(false);
        scanning = false; 

    }

    private void Update()
    {

        if (Input.GetKey(KeyCode.F)) scanning = !scanning;

        

        updateScanner.Invoke(BarValue(scanTimer, timeLeft));
        ScannerAnimation();


        if (!scanning) return; ////////

        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0.0f)
        {
            DisplayInfo(scanTimer, timeLeft);
            timeLeft = scanTimer;
        }


    }

    private void ScannerAnimation()
    {
        scanRect.SetActive(true);
        float newY = Mathf.Sin(Time.time * scanAnimationSpeed) * range;

        scanRect.transform.localPosition = new Vector3(scanRect.transform.localPosition.x, scanRect.transform.localPosition.y, newY);
    }

    private float BarValue(float fishTimer, float timeLeft)
    {
        return (fishTimer - timeLeft) / fishTimer;
    }
    
    public bool Scanning()
    {
        Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, transform.localScale / 2,
            Quaternion.identity, fishLayerMask);

        if (hitColliders.Length == 0) return false; ;

        if (currentFish == null)
        {
            currentFish = ClosestFish(hitColliders);
            return true;
        }
        else
        {
            return hitColliders.Contains(currentFish);
        }
    }

    private Collider ClosestFish(Collider[] hitColliders)
    {
        Collider closestFish = hitColliders[0];

        float minDist = Vector3.Distance(pivotPoint.position, closestFish.gameObject.transform.position);

        foreach(var collider in hitColliders)
        {
            float tempDist = Vector3.Distance(pivotPoint.position, collider.gameObject.transform.position);
            if (tempDist < minDist)
            {
                minDist = tempDist;
                closestFish = collider;
            }
        }

        Debug.Log(closestFish.name);

        return closestFish;
    }

    public void DisplayInfo(float fishTimer, float timeLeft)
    {
        scanFinished.Invoke("Nemo", "Blup blup");
        scanning = false;
        currentFish = null;
        scanRect.SetActive(false);
    }
}
