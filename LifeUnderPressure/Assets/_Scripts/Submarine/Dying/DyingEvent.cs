using System;
using System.Collections;
using UnityEngine;

public class DyingEvent : MonoBehaviour
{
    [SerializeField] GameObject submarineBroken;
    [SerializeField] GameObject sealogPickable;

    [SerializeField] Vector3 sealogOffset = new Vector3(0, 2, 1);

    [SerializeField] CanvasGroup blackScreen;

    [SerializeField] Submarine submarine;
    [SerializeField] Encyclopedia encyclopedia;
    float cooldown = 1f;
    float fadeDuration = 3f;
    float blackScreenDuration = 3f;

    
    GameObject tempSealog;

    void Awake()
    {
        if (encyclopedia==null) encyclopedia = FindObjectOfType<Encyclopedia>();
    }

    public void OnDie(Vector3 placeOfDeath)
    {        
        if (encyclopedia!=null)encyclopedia.ClearSealog();

        if (submarine== null) submarine= FindObjectOfType<Submarine>();
        submarine.getSubmarineMovement().enabled= false;
        submarine.enabled = false;
        StartCoroutine(FadeOutAfterCooldown(placeOfDeath));
        

    }

    public void OnRespawn(Vector3 placeOfDeath)
    {
        submarine.enabled = true;
        submarine.getSubmarineMovement().enabled = true;

        submarine.ForceSetPosition(new Vector3(0f, -2f, 0f));
        submarine.transform.rotation = Quaternion.identity;

        submarine.getSubmarineMovement().ResetMovement();
        submarine.getSubmarineHealth().ResetHealth();

        Instantiate(submarineBroken, placeOfDeath, Quaternion.identity);
        tempSealog = Instantiate(sealogPickable, placeOfDeath + sealogOffset, Quaternion.identity);

        encyclopedia.ping.setPingTransform(tempSealog.transform, "Sealog"); 

    }

    public void ResetSealog()
    {
        encyclopedia.ResetSealogCache();
        tempSealog.SetActive(false); 
    }

    IEnumerator FadeOutAfterCooldown(Vector3 placeOfDeath)
    {
        yield return new WaitForSecondsRealtime(cooldown);
        blackScreen.alpha = 1f;
        yield return new WaitForSecondsRealtime(blackScreenDuration);
        float elapsedTime = 0f;
        OnRespawn(placeOfDeath);
        while (elapsedTime < fadeDuration) 
        {
            elapsedTime += Time.deltaTime;

            blackScreen.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);

            yield return null;
        }

        blackScreen.alpha = 0f;

        
    }


    

    
}
