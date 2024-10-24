using FMOD.Studio;
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
    [SerializeField] GameObject bar;
    

    private EventInstance scanningInstance;

    LayerMask fishLayerMask;
    Quaternion initialRotation;

    float timeLeft;

#region Events
    public Action scanFinished;
    public Action<float> updateScanner;
    public Action resetScannerLock;

    public Action<ScannerState> lockActive;
    public Action<Vector3> targetLock;

    public Action<GameObject, bool> ScanEffect;
    public Action<bool> ShowWarning;
#endregion

    public Collider currentFish;

    bool mouseDown;

    

    private ScannerState _currentState = ScannerState.None;

    // Property to monitor changes to currstate
    public ScannerState currentState
    { 
        get { return _currentState; }
        set
        {
            if (_currentState != value)
            {
                _currentState = value;
                if (lockActive!=null) lockActive.Invoke(_currentState);
                Debug.Log("Value changes");// Call the method when the current state changes
            }
        }
    }

    private void Start()
    {
        fishLayerMask = (1 << fishLayer);
        initialRotation = scanRect.transform.localRotation;
        timeLeft = scanTimer;
        scanRect.SetActive(false);
        bar.SetActive(false);
        mouseDown = false;

        currentState = ScannerState.Inactive;

        // Janko >> 
        scanningInstance = AudioManager.instance.CreateInstance(FMODEvents.instance.scanningSFX);
        scanningInstance.setParameterByName("ScanningInput", 0);
        scanningInstance.start();
        // Janko << 
    }

    private void Update()
    {


        if (Input.GetMouseButtonUp(0))
        {
            mouseDown = false;
            if (currentState == ScannerState.Scanning)
            {
                currentState = ScannerState.Inactive;
                ResetScanner(false);
            }
        }
            


        if (Input.GetMouseButton(0) && currentState == ScannerState.Inactive)
        {
            if (ShowWarning != null) ShowWarning.Invoke(true);
        } else if (Input.GetMouseButtonUp(0) && currentState == ScannerState.Inactive)
        {
            if (ShowWarning != null) ShowWarning.Invoke(false);
        }
            


        switch (currentState)
        {
            case ScannerState.Inactive:
                InactiveUpdate();
                break;
            case ScannerState.InRange:
                InRangeUpdate();
                break;
            case ScannerState.Scanning:
                ScanningUpdate();
                break;
        }

    }


    private void FixedUpdate()
    {
        if (getHitColliders())
        {
            if (ShowWarning != null) ShowWarning.Invoke(false);
            if (currentState == ScannerState.InRange && Input.GetMouseButton(0) && !mouseDown)
            {
                currentState = ScannerState.Scanning;
                mouseDown = true;
                ResetScanner(true);
            }

            if (currentState == ScannerState.Inactive)
            {
                currentState = ScannerState.InRange;
            }
        }
        else
        {
            currentState = ScannerState.Inactive;
            ResetScanner(false);
            currentFish = null;
        }
    }

    // Janko >>
    private void OnDestroy()
    {
        scanningInstance.stop(STOP_MODE.ALLOWFADEOUT);
    }
    // Janko <<

    private void ScanningUpdate() // update for when scanner is active
    {
        timeLeft -= Time.deltaTime;
        ScannerAnimation();

        // Janko >>
        scanningInstance.setParameterByName("ScanningInput", 1);
        // Janko <<

        if(currentFish!=null)targetLock?.Invoke(currentFish.transform.position);
        updateScanner.Invoke(BarValue(scanTimer, timeLeft));

        if (timeLeft <= 0.0f)
        {
            if (currentFish != null && currentFish.gameObject.tag == "Sealog") FinishScanningSealog();
            else
            FinishedScanner();
        }
    }

    private void InRangeUpdate() // update for when fish is in range but not scanned
    {
        ChooseTarget();
        if (currentFish != null) { targetLock.Invoke(currentFish.transform.position); }

        // Janko >>
        scanningInstance.setParameterByName("ScanningInput", 0);
        // Janko <<
    }

    private void InactiveUpdate() // update for scanner being inactive
    {
        if (timeLeft < scanTimer)
        {
            timeLeft += Time.deltaTime / 2;
            updateScanner.Invoke(BarValue(scanTimer, timeLeft));
        }
    }

    private void FinishedScanner()
    {
        // Aleksis >>
        FishInfo fishInfo = currentFish.gameObject.GetComponent<Fish>().FishInfo;
        fishInfo.locked = false;
        if (fishInfo.OnLockedChange != null) fishInfo.OnLockedChange.Invoke();
        QuestSystem.ScannedFish(fishInfo);
        DataManager.Write("FishScanned_" + fishInfo.fishName, 1);
        // Aleksis <<

        if (ScanEffect!=null)ScanEffect.Invoke(currentFish.gameObject, false);

        DisplayInfo(fishInfo);

        ResetScanner(false);
        currentFish = null;
        Submarine.Instance.GetEncyclopedia().ping.EnablePing(false);
        Submarine.Instance.GetEncyclopedia().ping.pingArea = null; 
        currentState = ScannerState.Inactive;
        lockActive.Invoke(currentState);

        // Janko >>
        scanningInstance.setParameterByName("ScanningInput", 0);
        AudioManager.instance.PlayOneShot(FMODEvents.instance.scannedNotificationSFX, transform.position);
        // Janko <<
    }

    void FinishScanningSealog() {

        Debug.Log("scanned sealog");
        FindObjectOfType<DyingEvent>().ResetSealog();
        if (ScanEffect != null) ScanEffect.Invoke(currentFish.gameObject, false);
        ResetScanner(false);
        currentFish = null;

        Submarine.Instance.GetEncyclopedia().ping.EnablePing(false);
        Submarine.Instance.GetEncyclopedia().ping.pingArea = null;
        currentState = ScannerState.Inactive;
        lockActive.Invoke(currentState);

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(transform.position, borders.transform.localScale);

    }
    private void ChooseTarget()
    {
        Collider[] hitColliders = Physics.OverlapBox(transform.position, borders.transform.localScale / 2,
            transform.rotation, fishLayerMask);

        if (hitColliders.Length == 0) return;

        currentFish = ClosestToCameraFish(hitColliders);
    }

    private bool getHitColliders()
    {
        return (Physics.OverlapBox(transform.position, borders.transform.localScale / 2, 
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
    


#region ClosestFishDetection


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
        if (scanFinished!=null) scanFinished.Invoke();
        currentFish = null;
    }

    private void ResetScanner(bool scan)
    {
        scanRect.SetActive(scan);
        timeLeft = scanTimer;
        resetScannerLock.Invoke();
        if (currentFish!=null && ScanEffect!=null) ScanEffect.Invoke(currentFish.gameObject, scan);
        // Janko >>
        scanningInstance.setParameterByName("ScanningInput", 0);
        // Janko <<
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

public enum ScannerState
{
    None, 
    Inactive,
    InRange, 
    Scanning
}


