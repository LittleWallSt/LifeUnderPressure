using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanningEffect : MonoBehaviour
{
    [SerializeField]private Material scanEffect;
    private Material[] cash;
    private Renderer cashedObject;

    [SerializeField] Scanner scanner;

    private void Start()
    {
        scanner.ScanEffect += OnChange;
    }

    private void OnChange(GameObject fish, bool change) // change = true - change material, change = false - revert
    {
        if (change) ChangeMaterial(fish);
        else RevertMaterial(fish);
    }

    private void ChangeMaterial(GameObject fish)
    {
        Debug.Log("reacjed");
        cashedObject = fish.GetComponentInChildren<Renderer>();
        cash = cashedObject.materials;
        fish.GetComponentInChildren<Renderer>().material = scanEffect; 
    }

    private void RevertMaterial(GameObject fish)
    {
        if (cash != null && cash.Length > 0)
        {
            cashedObject.materials = cash;
        }
    }



    
}
