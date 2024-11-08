using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DyingEvent : MonoBehaviour
{
    [Header("Dying")]
    [SerializeField] GameObject submarineBroken;
    [SerializeField] GameObject sealogPickable;

    [SerializeField] Vector3 sealogOffset = new Vector3(0, 2, 1);

    [SerializeField] CanvasGroup blackScreen;
    [SerializeField] TextMeshProUGUI dyingText;

    [SerializeField] Submarine submarine;
    [SerializeField] Encyclopedia encyclopedia;

    [Header("Endgame")]
    [SerializeField] private string sceneToLoad = "Credits";
    [SerializeField] GameObject lights;

    [SerializeField] private GameObject questionIcon;
    [SerializeField] GameObject finalFish;
    float firstVoiceline = 1f;
    float chokingTime = 4f;


    float cooldown = 3f;
    float fadeDuration = 3f;
    float blackScreenDuration = 3f;

    // Janko >>
    private float waitBeforePlayingSound = .5f;
    // Janko <<

    GameObject tempSealog;

    void Awake()
    {
        if (encyclopedia==null) encyclopedia = FindObjectOfType<Encyclopedia>();
        finalFish.SetActive(false); //?
        questionIcon.SetActive(false);
    }

    public void OnDie(Vector3 placeOfDeath, DamageType damageType)
    {
        if (damageType == DamageType.End) return; 
        if (encyclopedia!=null)encyclopedia.ClearSealog();

        if (submarine== null) submarine= FindObjectOfType<Submarine>();
        submarine.getSubmarineMovement().enabled= false;
        submarine.enabled = false;
        dyingText.text = "You died " + damageType.ToCustomString();
        
        StartCoroutine(FadeOutAfterCooldown(placeOfDeath));
        

    }

    public void OnEnd()
    {
        questionIcon.SetActive(true);
        /* voiceline about fish spawned
         */
        finalFish.SetActive(true);
        
    }

    public void onDieEnd()
    {
        if (submarine == null) submarine = FindObjectOfType<Submarine>();
        submarine.getSubmarineMovement().enabled = false;
        submarine.enabled = false;
        dyingText.text = "You died. There is no life under this pressure.";
        Debug.Log("called here");

        StartCoroutine(CallEndScreen());
        Debug.Log("called");
    }

    public void OnRespawn(Vector3 placeOfDeath)
    {
        submarine.enabled = true;
        submarine.getSubmarineMovement().enabled = true;
        dyingText.text = "";

        submarine.ForceSetPosition(GameManager.Instance.InitialSpawnPoint);
        submarine.transform.rotation = Quaternion.identity;

        submarine.getSubmarineMovement().ResetMovement();
        submarine.getSubmarineHealth().Respawn();

        Instantiate(submarineBroken, placeOfDeath, Quaternion.identity);
        tempSealog = Instantiate(sealogPickable, placeOfDeath + sealogOffset, Quaternion.identity);

        encyclopedia.ping.setPingTransform(tempSealog.transform, "Sealog");

        var depthMeter = FindAnyObjectByType<DepthMeterUI>();
        depthMeter.OnRespawn();

    }

    public void ResetSealog()
    {
        encyclopedia.ResetSealogCache();
        tempSealog.SetActive(false); 
    }

    IEnumerator FadeOutAfterCooldown(Vector3 placeOfDeath)
    {
        // Janko >>
        AudioManager.instance.PlayOneShot(FMODEvents.instance.SFX_Death, placeOfDeath);
        // Janko <<
        float elapsedTime = 0f;
        while (elapsedTime < cooldown)
        {
            elapsedTime += Time.deltaTime;

            blackScreen.alpha = Mathf.Lerp(0f, 1f, elapsedTime / cooldown);

            yield return null;
        }
        
        blackScreen.alpha = 1f;
        // Janko >>
        yield return new WaitForSecondsRealtime(waitBeforePlayingSound);
        AudioManager.instance.PlayOneShot(FMODEvents.instance.SFX_Implosion, placeOfDeath);
        // Janko<< 
        yield return new WaitForSecondsRealtime(blackScreenDuration);
        elapsedTime = 0f;
        OnRespawn(placeOfDeath);
        while (elapsedTime < fadeDuration) 
        {
            elapsedTime += Time.deltaTime;

            blackScreen.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);

            yield return null;
        }

        blackScreen.alpha = 0f;
      
    }

    IEnumerator CallEndScreen()
    {
        /*you lose control over the submarine, and ben's voiceline starts to play.
         * he tells you that he is happy that you found the rare fish, but that he has to kill you to get the rewards 100% for himself.
         */
        yield return new WaitForSecondsRealtime(firstVoiceline);

        /*you will start taking damage slowly while a sound effect of choking is playing.
         */
        float elapsedTime = 0f;
        lights.gameObject.SetActive(false);
        float dmgPerFrame = Time.deltaTime / chokingTime * Submarine.Instance.getSubmarineHealth().MaxHealth;
        while (elapsedTime < chokingTime)
        {
            elapsedTime += Time.deltaTime;
            Submarine.Instance.getSubmarineHealth().DealDamage(dmgPerFrame, DamageType.End);

            yield return null;
        }

        /*he says goodbye and thank you, screen fades out. THE END
         * 
         */

        elapsedTime = 0f;
        while (elapsedTime < cooldown)
        {
            elapsedTime += Time.deltaTime;

            blackScreen.alpha = Mathf.Lerp(0f, 1f, elapsedTime / cooldown);

            yield return null;
        }

        blackScreen.alpha = 1f;

        yield return new WaitForSecondsRealtime(blackScreenDuration);
        dyingText.text = "";
        SceneManager.LoadScene(sceneToLoad);


    }





}
