using FMODUnity;
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

    [Header("Voicelines")]
    [SerializeField] private Voiceline[] afterDeath;

    [SerializeField] private EventReference intro;
    [SerializeField] private EventReference outro;
    [SerializeField] private EventReference choking;
    int VOiterator = 0;

    [Header("Endgame")]
    [SerializeField] private string sceneToLoad = "Credits";
    [SerializeField] GameObject lights;

    [SerializeField] private GameObject questionIcon;
    [SerializeField] GameObject finalFish;

    [SerializeField] GameObject controlsScreen;
    float firstVoiceline = 1f;
    float chokingTime = 4f;


    float cooldown = 3f;
    float fadeDuration = 3f;
    float blackScreenDuration = 3f;

    // Janko >>
    private float waitBeforePlayingSound = .5f;
    // Janko <<

    GameObject tempSealog;

    ImageAnimation sonar;

    void Awake()
    {
        if (encyclopedia==null) encyclopedia = FindObjectOfType<Encyclopedia>();
        finalFish.SetActive(false); //?
        questionIcon.SetActive(false);

        controlsScreen?.SetActive(false);

        StartCoroutine(OnStart());

    }
    private void Start()
    {
        OnStartSetup(GameManager.Instance.InitialSpawnPoint + new Vector3(-12, 0, 0));
    }

    void OnStartSetup(Vector3 SealogPlacement)
    {

        Instantiate(submarineBroken, SealogPlacement, Quaternion.identity);
        tempSealog = Instantiate(sealogPickable, SealogPlacement + sealogOffset, Quaternion.identity);

        

        
    }

    public void OnDie(Vector3 placeOfDeath, Vector3 direction, DamageType damageType)
    {
        if (damageType == DamageType.End) return; 
        if (encyclopedia!=null)encyclopedia.ClearSealog();
        controlsScreen?.SetActive(false);

        if (submarine== null) submarine= FindObjectOfType<Submarine>();
        submarine.getSubmarineMovement().enabled= false;
        submarine.enabled = false;
        dyingText.text = "You died " + damageType.ToCustomString();

        if (sonar == null) sonar = FindAnyObjectByType<ImageAnimation>();
        if (sonar != null) sonar.enabled = false;
        StopAllCoroutines(); 
        StartCoroutine(FadeOutAfterCooldown(placeOfDeath, direction));
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
        
        StartCoroutine(CallEndScreen());
        
    }

    public void OnRespawn(Vector3 placeOfDeath, Vector3 direction)
    {
        submarine.ForceSetPosition(GameManager.Instance.InitialSpawnPoint);
        submarine.ForceSetEuler(GameManager.Instance.InitialEulerAngles);

        submarine.enabled = true;
        submarine.getSubmarineMovement().enabled = true;
        dyingText.text = "";

        submarine.getSubmarineMovement().ResetMovement();
        submarine.getSubmarineHealth().Respawn();

        Physics.SphereCast(placeOfDeath, 0.05f, Vector3.down, out RaycastHit hit, 100f);
        if (hit.transform)
        {
            placeOfDeath = hit.point;
            direction = hit.normal;
        }
        else direction = -direction;

        float distance = 1f;
        bool sealogBlocked = true;
        int count = 0;
        while (sealogBlocked)
        {
            distance += 1f;
            sealogBlocked = Physics.CheckSphere(placeOfDeath + direction.normalized * distance, .2f);
            if(++count > 5)
            {
                distance = 2f;
                Debug.Log("brea");
                break;
            }
        }

        Instantiate(submarineBroken, placeOfDeath, Quaternion.Euler(0, Random.Range(0, 360), 0));
        tempSealog = Instantiate(sealogPickable, placeOfDeath + direction.normalized * distance, Quaternion.identity);

        encyclopedia.ping.setPingTransform(tempSealog.transform, "Sealog");

        var depthMeter = FindAnyObjectByType<DepthMeterUI>();
        depthMeter.OnRespawn();

        if (afterDeath!=null && afterDeath.Length>VOiterator) VoicelinesUI.Instance?.CallVoiceline(afterDeath[VOiterator]);
        VOiterator++;

        if (sonar == null) sonar = FindAnyObjectByType<ImageAnimation>();
        if (sonar != null) sonar.enabled = true ;

    }

    public void ResetSealog()
    {
        encyclopedia.ResetSealogCache();
        tempSealog.SetActive(false); 
    }

    IEnumerator FadeOutAfterCooldown(Vector3 placeOfDeath, Vector3 direction)
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
        OnRespawn(placeOfDeath, direction);
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

        if (!outro.IsNull)AudioManager.instance?.PlayOneShot(outro, Submarine.Instance.transform.position);
        /*you lose control over the submarine, and ben's voiceline starts to play.
         * he tells you that he is happy that you found the rare fish, but that he has to kill you to get the rewards 100% for himself.
         */
        firstVoiceline = 7f;
        yield return new WaitForSecondsRealtime(firstVoiceline); //7 seconds  add red loght and choking /// 15 seconds choking
        chokingTime = 15f;
        /*you will start taking damage slowly while a sound effect of choking is playing.
         */
        if (!choking.IsNull)AudioManager.instance?.PlayOneShot(choking, Submarine.Instance.transform.position); 
        float elapsedTime = 0f;
        lights.gameObject.SetActive(false);
        float dmgPerFrame = Time.deltaTime / chokingTime * Submarine.Instance.getSubmarineHealth().MaxHealth;
        while (elapsedTime < chokingTime)
        {
            elapsedTime += Time.deltaTime;
            Submarine.Instance.getSubmarineHealth().DealDamage(dmgPerFrame, Vector3.zero, DamageType.End);

            yield return null;
        }


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

    IEnumerator OnStart()
    {
        //if (!intro.IsNull) AudioManager.instance?.PlayOneShot(intro, Submarine.Instance.transform.position);  
        yield return InternalSettings.WaitForDataLoading();
        if (!intro.IsNull) AudioManager.instance.PlayVoiceline(intro);
        yield return new WaitForSecondsRealtime(8f);

        controlsScreen?.SetActive(true);

        yield return new WaitForSecondsRealtime(5f);

        controlsScreen?.SetActive(false);

        encyclopedia.ping.setPingTransform(tempSealog.transform, "Sealog");

    }





}
