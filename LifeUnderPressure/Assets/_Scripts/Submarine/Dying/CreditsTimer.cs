using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsTimer : MonoBehaviour
{

    [SerializeField] float creditsTimer = 3f;
    [SerializeField] CanvasGroup pressButonText;
    [SerializeField] private string sceneToLoad;

    bool timerFinished = false;
    float timer = 0f;

    private void Start()
    {
        pressButonText.alpha = 0f;
        InternalSettings.EnableCursor(true); 
    }
    void Update()
    {

        timer += Time.deltaTime;
        if (timer >= creditsTimer)
        {
            timerFinished = true;
            StartCoroutine(fadeIn());
        }

        if (timerFinished)
        {
            if (Input.anyKeyDown)
            {
                
                SceneManager.LoadScene(sceneToLoad);
            }
        }
    }


    IEnumerator fadeIn()
    {
        float elapsedTime = 0f;
        float cooldown = 1f;
        while (elapsedTime < cooldown)
        {
            elapsedTime += Time.deltaTime;

            pressButonText.alpha = Mathf.Lerp(0f, 1f, elapsedTime / cooldown);

            yield return null;
        }

        pressButonText.alpha = 1f;
    }
}
