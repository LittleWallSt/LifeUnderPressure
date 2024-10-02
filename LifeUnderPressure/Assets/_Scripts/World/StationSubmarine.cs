using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationSubmarine : MonoBehaviour, IDistanceLoad 
{
    [SerializeField] private Transform dockingTransform = null;
    [SerializeField] private float dockingTime = 2f;

    private Submarine submarineInZone = null;
    private Vector3 startPos = Vector3.zero;
    private float time = 0f;
    private bool docking = false;
    private bool docked = false;
    private void Start()
    {
        IDL_AssignToGameManager();
    }
    private void OnValidate()
    {
        if (dockingTime <= 0f) dockingTime = 0.1f;
    }
    private void Update()
    {
        if (!submarineInZone) return;

        DockingProcess();
        DockingInput();
    }

    private void DockingInput()
    {
        if (docking) return;
        if (!Input.GetKeyDown(KeyCode.Tab)) return;

        docked = !docked;
        if (docked)
        {
            docking = true;
            time = 0f;
            startPos = submarineInZone.transform.position;
            // Janko >>
            AudioManager.instance.PlayOneShot(FMODEvents.instance.dockingSFX, startPos);
            // Janko <<
        }

        submarineInZone.EnableMovement(!docked);
        submarineInZone.SetDocked(false);
        if (!docked)
        {
            submarineInZone.ForceCloseCurrentMenu();
        }
    }

    private void DockingProcess()
    {
        if (!docking) return;

        submarineInZone.transform.position = Vector3.Slerp(startPos, dockingTransform.position, time);
        time += Time.deltaTime / dockingTime;
        if (time >= 1f)
        {
            docking = false;
            docked = true;
            submarineInZone.SetDocked(true);
            submarineInZone.ForceEnableUpgradeCanvas();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        Submarine submarine = other.gameObject.GetComponent<Submarine>();
        if (!submarine) return;

        submarineInZone = submarine;
        submarine.EnableDockText(true);
    }
    private void OnTriggerExit(Collider other)
    {
        Submarine submarine = other.gameObject.GetComponent<Submarine>();
        if (!submarine) return;

        if(submarine == submarineInZone)
        {
            submarineInZone = null;
            submarine.EnableDockText(false);
        }
    }

    // IDistanceLoad
    public void IDL_AssignToGameManager()
    {
        // Dont assign for now
        //GameManager.Instance.AssignIDL(this);
    }

    public Vector3 IDL_GetPosition()
    {
        return transform.position;
    }

    public void IDL_InDistance()
    {
        gameObject.SetActive(true);
    }

    public void IDL_OffDistance()
    {
        gameObject.SetActive(false);
    }
}
