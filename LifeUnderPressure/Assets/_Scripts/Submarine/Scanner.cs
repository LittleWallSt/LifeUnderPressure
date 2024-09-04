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
    [SerializeField] float depletingSpeed;
    [SerializeField] int fishLayer = 8;

    [Header("Borders")]
    [SerializeField] GameObject scanRect;
    [SerializeField] GameObject borders;
    [SerializeField] Transform pivotPoint; // pivot point for distance between fishes measurment


    LayerMask fishLayerMask;
    Quaternion initialRotation;

    float timeLeft;
    float range;
    bool scanning;
    bool locked;
    

    public Action<string, string> scanFinished;
    public Action<float> updateScanner;
    public Action<Vector3> targetLock;

    public Collider currentFish;

    private void Start()
    {
        fishLayerMask = (1 << fishLayer);
        range = borders.transform.localScale.z / 2;
        initialRotation = scanRect.transform.localRotation;
        timeLeft = scanTimer;
        scanRect.SetActive(false);
        scanning = false;
        locked = false;

    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.F) && !scanning)
        {
            if (!locked)
            {
                locked = true;
            } else
            {
                ResetScanner();
                locked = false;
            }
        }

        if(locked) ChooseTarget();
        if (currentFish!= null) { targetLock.Invoke(currentFish.transform.position); }



        if (!scanning) return;

        if (Scanning())
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0.0f)
            {
                DisplayInfo(scanTimer, timeLeft);
                ResetScanner();
                

            }
        }
        else if (timeLeft <= scanTimer)
        {
            timeLeft += Time.deltaTime; 
        }




        updateScanner.Invoke(BarValue(scanTimer, timeLeft));
        ScannerAnimation();

    }

    private void ChooseTarget()
    {
        Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, transform.localScale / 2,
            Quaternion.identity, fishLayerMask);

        if (hitColliders.Length == 0) return;

        currentFish = ClosestToCameraFish(hitColliders);
    }

    private void ScannerAnimation()
    {
        float newY = Mathf.PingPong(Time.time * scanAnimationSpeed, 50f * 2) - 50f; 
        scanRect.transform.localRotation = initialRotation* Quaternion.Euler(newY, 0, 0);  
    }

    private float BarValue(float fishTimer, float timeLeft)
    {
        return (fishTimer - timeLeft) / fishTimer;
    }
    
    public bool Scanning() //returns True if fish is still within Radar collider
    {
        Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, transform.localScale / 2,
            Quaternion.identity, fishLayerMask);

        if (hitColliders.Length == 0) return false; ;

        if (currentFish == null)
        {
            currentFish = ClosestToCameraFish(hitColliders);
            return true;
        }
        else
        {
            return hitColliders.Contains(currentFish);
        }
    }

#region ClosestFishDetection

    private Collider ClosestFish(Collider[] hitColliders)
    {
        Collider closestFish = hitColliders[0];

        float minDist = (pivotPoint.position - closestFish.gameObject.transform.position).sqrMagnitude;

        foreach(var collider in hitColliders)
        {
            float tempDist = (pivotPoint.position - collider.gameObject.transform.position).sqrMagnitude;
            if (tempDist < minDist)
            {
                minDist = tempDist;
                closestFish = collider;
            }
        }

        Debug.Log(closestFish.name);

        return closestFish;
    }

    private Collider ClosestToCameraFish(Collider[] hitColliders) // return Fish collider that is closest to the center
    {
        Vector2 screenCenter = Camera.main.ViewportToScreenPoint(new Vector2(0.5f, 0.5f));

        Collider closestFish = null;
        float minDist = Mathf.Infinity;

        foreach (var collider in hitColliders)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(collider.gameObject.transform.position);

            float distance = Vector2.Distance(screenCenter, screenPos);
            if (distance < minDist)
            {
                minDist = distance;
                closestFish = collider;
            }

        }

        return closestFish;
    }

#endregion

    public void DisplayInfo(float fishTimer, float timeLeft)
    {
        scanFinished.Invoke(currentFish.name, "Blup blup");
        currentFish = null;
    }

    private void ResetScanner()
    {
        scanning = !scanning;
        scanRect.SetActive(scanning);
        timeLeft = scanTimer;
    }

    
}
