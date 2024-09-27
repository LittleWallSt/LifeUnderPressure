using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScannerUI : MonoBehaviour
{
    [SerializeField] Slider scannerSlider;
    [SerializeField] TextMeshProUGUI fishName;
    [SerializeField] TextMeshProUGUI fishDescription;
    public CanvasGroup scanPanel; 

    [SerializeField] Scanner scanner;


    float cooldown = 3f;
    float fadeDuration = 3f;

    private void Start()
    {
        scanner.scanFinished += DisplayInfo;
        scanPanel.gameObject.SetActive(false);
    }

    public void DisplayInfo(string _fishName, string _fishDescription)
    {
        scanPanel.gameObject.SetActive(true);
        scanPanel.alpha = 1.0f;
        fishName.text = _fishName;
        fishDescription.text = _fishDescription;
        StartCoroutine(FadeOutAfterCooldown());
    }

    public void SetScanBarValue(float value)
    {
        scannerSlider.value = value; 
    }

    

    IEnumerator FadeOutAfterCooldown()
    {
        // Wait for the cooldown period before starting the fade
        yield return new WaitForSeconds(cooldown);


        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;

            // Calculate the new alpha based on how much time has passed
            scanPanel.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);

            yield return null;
        }

        scanPanel.alpha = 0f;
        scanPanel.gameObject.SetActive(false);
    }
}
