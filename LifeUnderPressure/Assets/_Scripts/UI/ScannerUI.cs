using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScannerUI : MonoBehaviour
{
    [SerializeField] Slider scannerSlider;
    [SerializeField] TextMeshProUGUI fishName;
    [SerializeField] TextMeshProUGUI fishDescription;
    [SerializeField] GameObject scanPanel;


    [SerializeField] Scanner scanner;

    private void Start()
    {
        scanner.scanFinished += DisplayInfo;
        scanner.updateScanner += SetScanBarValue;
    }

    public void DisplayInfo(string _fishName, string _fishDescription)
    {
        fishName.text = _fishName;
        fishDescription.text = _fishDescription;
    }

    public void SetScanBarValue(float value)
    {
        scannerSlider.value = value; 
    }
}
