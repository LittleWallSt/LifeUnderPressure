using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

public class Scanner : MonoBehaviour
{
    [Header("Calibration values")]
    [SerializeField] float scanTimer = 3f;
    [SerializeField] float scanAnimationSpeed = 1.5f;
    [SerializeField] float depletingSpeed;
    [SerializeField] int fishLayer = 8;

    [Header("Assets")]
    [SerializeField] GameObject scanRect;
    [SerializeField] GameObject borders;
    [SerializeField] Transform pivotPoint; // pivot point for distance between fishes measurment
    [SerializeField] GameObject bar;
    [SerializeField] GameObject scannerCanvas;


    LayerMask fishLayerMask;
    Quaternion initialRotation;

    float timeLeft;
    float range;
    bool scanning;
    bool locked;
    bool fishLost;

#region Events
    public Action<string, string> scanFinished;
    public Action<float> updateScanner;

    public Action<bool> lockActive;
    public Action<Vector3> targetLock;

    public Action<GameObject, bool> ScanEffect;
#endregion

    public Collider currentFish;

    private void Start()
    {
        fishLayerMask = (1 << fishLayer);
        range = borders.transform.localScale.z / 2;
        initialRotation = scanRect.transform.localRotation;
        timeLeft = scanTimer;
        scanRect.SetActive(false);
        scannerCanvas.SetActive(false);
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
                scanRect.SetActive(true);
                scannerCanvas.SetActive(true);
            } else if(!getHitColliders())
            {
                Debug.Log("worng one");
                ResetScanner(false);
                locked = false;
            } else 
            {
                ResetScanner(true);
                locked = false; // change placeds  
                lockActive.Invoke(true);
                if (currentFish != null) { ScanEffect.Invoke(currentFish.gameObject, true); Debug.Log("fish not null"); }
                
            }
        }

        if (locked)
        {
            ChooseTarget();
            ScannerAnimation();
        }
        if (currentFish!= null) { targetLock.Invoke(currentFish.transform.position); }



        if (!scanning) return;

        if (Scanning())
        {
            if(fishLost) lockActive.Invoke(true);
            fishLost = false;
            timeLeft -= Time.deltaTime;
            ScannerAnimation();
            
            if (timeLeft <= 0.0f)
            {
                // Aleksis >>
                FishInfo fishInfo = currentFish.gameObject.GetComponent<Fish>().FishInfo;
                QuestSystem.ScannedFish(fishInfo.fishName);
                // Aleksis <<
                ScanEffect.Invoke(currentFish.gameObject, false);
                lockActive.Invoke(false);
                DisplayInfo(fishInfo);
                ResetScanner(false);
                currentFish = null;

            }
        }
        else if (timeLeft <= scanTimer)
        {
            timeLeft += Time.deltaTime/2;
            if (!fishLost) lockActive.Invoke(false);
            fishLost = true;
            

        }

        updateScanner.Invoke(BarValue(scanTimer, timeLeft));
    }

    private void ChooseTarget()
    {
        Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, transform.localScale / 2,
            Quaternion.identity, fishLayerMask);

        if (hitColliders.Length == 0) return;

        currentFish = ClosestToCameraFish(hitColliders);
    }

    private bool getHitColliders()
    {
        return (Physics.OverlapBox(gameObject.transform.position, transform.localScale / 2,
            Quaternion.identity, fishLayerMask).Length > 0);
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
            //currentFish = ClosestToCameraFish(hitColliders);
            return false;
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

    public void DisplayInfo(FishInfo fishInfo)
    {
        if (fishInfo == null) return;
        scanFinished.Invoke(fishInfo.fishName, fishInfo.fishSmallDescription);
        currentFish = null;
    }

    private void ResetScanner(bool scan)
    {
        scanning = scan;
        scanRect.SetActive(scan);
        bar.SetActive(scan);
        scannerCanvas.SetActive(scan);
        timeLeft = scanTimer;
    }

    
    // Setters
    //Aleksis>>
    public void SetScanTimer(float timer)
    {
        scanTimer = timer;
    }
    public void SetScanAnimationSpeed(float speed)
    {
        scanAnimationSpeed = speed;
    }
    public void SetDepletingSpeed(float speed)
    {
        depletingSpeed = speed;
    }
}
