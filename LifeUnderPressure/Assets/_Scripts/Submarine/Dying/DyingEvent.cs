using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DyingEvent : MonoBehaviour
{
    [SerializeField] GameObject submarineBroken;
    [SerializeField] GameObject sealogPickable;

    [SerializeField] Vector3 sealogOffset = new Vector3(0, 2, 1);

    [SerializeField] CanvasGroup blackScreen;

    [SerializeField] SubmarineMovement submarineMovement;
    float cooldown = 1f;
    float fadeDuration = 3f;

    Encyclopedia encyclopedia;
    GameObject tempSealog;
    public void OnDie(Vector3 placeOfDeath)
    {
        Instantiate(submarineBroken, placeOfDeath, Quaternion.identity);
        tempSealog = Instantiate(sealogPickable, placeOfDeath + sealogOffset, Quaternion.identity);

        if(encyclopedia==null) 
            encyclopedia = FindObjectOfType<Encyclopedia>();
        encyclopedia.ClearSealog();

        if (submarineMovement== null) 
            submarineMovement= FindObjectOfType<SubmarineMovement>();
        submarineMovement.enabled= false;

        StartCoroutine(SetBackScreen());
    }

    public void OnRespawn()
    {
        StartCoroutine(FadeOutAfterCooldown());
        submarineMovement.enabled = true;
        
    }

    public void ResetSealog()
    {
        encyclopedia.ResetSealogCache();
        Destroy(tempSealog);    
    }

    IEnumerator FadeOutAfterCooldown()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;

            blackScreen.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);

            yield return null;
        }

        blackScreen.alpha = 0f;
    }

    IEnumerator SetBackScreen()
    {
        yield return new WaitForSeconds(cooldown);
        blackScreen.alpha = 1f;
    }
}
