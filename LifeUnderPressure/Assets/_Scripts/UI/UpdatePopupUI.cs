using System.Collections;
using TMPro;
using UnityEngine;

public class UpdatePopupUI : MonoBehaviour
{
    [SerializeField] CanvasGroup popup;
    [SerializeField] TextMeshProUGUI popupText;
    [SerializeField] float cooldown = 2f;
    float fadeDuration = 2f;

    private void Start()
    {
        UpgradeTreeCanvas.Instance.onUnlock += DisplayPopup;
        popup.gameObject.SetActive(false);
    }

    void DisplayPopup(UpgradeType type)
    {
        popup.gameObject.SetActive(true);
        popup.alpha = 1.0f;
        popupText.text = "-New Update" + type.ToString() + " Unlocked-";
        StartCoroutine(FadeOutAfterCooldown());
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
            popup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);

            yield return null;
        }

        popup.alpha = 0f;
        popup.gameObject.SetActive(false);
    }
}
