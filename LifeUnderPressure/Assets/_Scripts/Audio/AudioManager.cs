using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    // ANYTHING THAT USES THIS SCRIPT WILL NEED using FMODUnity; and maybe using FMOD.Studio;
    // Add [RequireComponent(typeof(StudioEventEmitter))] on top of the classes that will have Event Emitters.
    [Header("Volume Settings")]
    [Range(0f, 1f)]
    public float masterVolume = 1f;

    [Range(0f, 1f)]
    public float gameSoundVolume = 1f;

    [Range(0f, 1f)]
    public float musicVolume = 1f;

    [Range(0f, 1f)]
    public float ambienceVolume = 1f;

    private Bus masterBus;
    private Bus sfxBus;
    private Bus musicBus;
    private Bus ambienceBus;

    public static AudioManager instance { get; private set; }

    private List<EventInstance> eventInstancesList;
    private List<StudioEventEmitter> eventEmittersList;

    // We will use all ambience sound in a single event for each level? Use this approach then.
    private EventInstance ambienceEventInstance;
    private EventInstance musicEventInstance;

    private EventInstance currentInstance;
    private float timeLastSetInstance;

    private bool isPaused = false;

    [Header("Music Settings")]
    [SerializeField]
    private float musicChangeCooldown = 5f;
    [SerializeField]
    private float playMusicOrAmbienceDelay = 2f;


    private void Awake()
    {
        if (instance)
        {
            foreach(Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        eventInstancesList = new List<EventInstance>();
        eventEmittersList = new List<StudioEventEmitter>();

        masterBus = RuntimeManager.GetBus("bus:/");
        sfxBus = RuntimeManager.GetBus("bus:/SFX_Bus");
        musicBus = RuntimeManager.GetBus("bus:/Music_Bus");
        ambienceBus = RuntimeManager.GetBus("bus:/Ambience_Bus");
    }

    private void Start()
    {
        PauseMenu.Assign_OnPaused(OnPause);
        Cave.Assign_OnInsideChanged(OnCaveInsideChanged);

        LevelVolume.Assign_OnCurrentVolumeChanged(SetArea);
        InitializeMusic(FMODEvents.instance.musicToPlay);
        InitializeAmbience(FMODEvents.instance.ambienceToPlay);

        currentInstance = musicEventInstance;
        currentInstance.start();
    }
    public void AssignSubmarineEvents(Submarine submarine)
    {
        Submarine.Instance.getSubmarineHealth().Assign_OnDie(OnDie);
        Submarine.Instance.getSubmarineHealth().Assign_OnRespawn(OnRespawn);
    }
    private void Update()
    {
        //CheckWhatMusicToPlay();

        SetVolume();

        if (isPaused)
            return;

        SetMusicOrAmbience();
    }

    /*private void CheckWhatMusicToPlay()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            FMODEvents.instance.musicToPlay = EventReference.Find("event:/Music/Menu Song");
        }
        else if (SceneManager.GetActiveScene().rootCount == 1)
        {
            FMODEvents.instance.musicToPlay = EventReference.Find("event:/Music/Levels Music");
        }
        else
            return;
    }*/

    private void SetVolume()
    {
        masterBus.setVolume(masterVolume);
        sfxBus.setVolume(gameSoundVolume);
        musicBus.setVolume(musicVolume);
        ambienceBus.setVolume(ambienceVolume);
    }

    private void SetMusicOrAmbience()
    {
        if (Cave.Inside) return;

        if (!IsPlaying(currentInstance) && Time.time - timeLastSetInstance > musicChangeCooldown)
        {
            currentInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            SwapCurrentInstance();
            playMusicOrAmbienceDelay = Random.Range(2f, 4.5f); // can't be higher than music change cooldown
            StartCoroutine(StartInstanceDelay(playMusicOrAmbienceDelay));
            timeLastSetInstance = Time.time;
        }
    }

    private IEnumerator StartInstanceDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        currentInstance.start();
    }
    public void SwapCurrentInstance()
    {
        currentInstance = currentInstance.Equals(musicEventInstance) ? ambienceEventInstance : musicEventInstance;
    }
    // Events
    public void StartCaveCollapse()
    {
        currentInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        SwapCurrentInstance();
        StartCoroutine(StartInstanceDelay(2f));
    }
    private void OnCaveInsideChanged()
    {
        if (Cave.Inside)
        {
            ambienceEventInstance.setParameterByName("Area", 3);
            musicEventInstance.setParameterByName("Area", 3);
            currentInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            currentInstance = ambienceEventInstance;
            currentInstance.start();
        }
        else
        {
            SetArea();
        }
    }
    private void OnDie(DamageType type)
    {
        currentInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        isPaused = true;
    }

    private void OnRespawn()
    {
        currentInstance.start();
        //currentInstance.setPaused(false);       
        isPaused = false;
    }

    private void OnPause(bool paused)
    {
        currentInstance.setPaused(paused);
        isPaused = paused;
    }

    private bool IsPlaying(EventInstance instance)
    {
        PLAYBACK_STATE state;
        instance.getPlaybackState(out state);
        return state != PLAYBACK_STATE.STOPPED;
    }

    // Use triggers and stuff for this. Will need a new Class for this,
    // for example AreaChangeTrigger from shallow to deeper parts, use for ambiance as well, make new trigger for that, refer to the video on yt.
    // Use seek speed on parameters in FMOD for smoother transitions
    public void SetParameterValue(string parameterName, float parameterValue)
    {
        // exampleEventInstance.setParameterByName(parameterName, parameterValue);
    }

    // Might have to add separate method to play main menu music.

    public void SetArea()
    {
        if (Cave.Inside) return;

        musicEventInstance.setParameterByName("Area", LevelVolume.Current.Level);
        ambienceEventInstance.setParameterByName("Area", LevelVolume.Current.Level);
    }

    private void InitializeAmbience(EventReference ambienceEventReference)
    {
        ambienceEventInstance = CreateInstance(ambienceEventReference);
        //ambienceEventInstance.start();
    }

    private void InitializeMusic(EventReference musicEventReference)
    {
        musicEventInstance = CreateInstance(musicEventReference);
        //musicEventInstance.start();
    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    // Used for sounds that will need to loop and will be played and stopped somewhere.
    // Need using FMOD.Studio in the scripts that will use these sounds and
    // EventInstance (private) type variable for the sound itself, playerFootsteps for example.
    // Need to initialize them (variables) in Start() of those scripts as well.
    public EventInstance CreateInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstancesList.Add(eventInstance);

        return eventInstance;
    }

    // FMOD Spatializer for sounds that are going to be space dependent; louder if you are closer and vice versa.
    // Will use FMOD Emitter for this, these sounds will mostly loop as well I think.
    // For the eventReference variable we will pass in the event/sound we want to override the event reference on the emitter with.
    // That field needs to be filled in the inspector for the attenuation field to work.
    // GameObject the emitter is attached to is emitterGameObject.
    // We could also set attenuation from here instead of on the emitteron the gameobject in the inspector?
    // For example we will need private StudioEventEmitter emitter and then we ->
    // Need to initialize the variable in Start() of the scripts that use this like ->
    // emitter = AudioManager.instance.InitializeEventEmitter(FMODEvents.instance.coinIdleSound, this.gameObject) and then ->
    // emitter.Play();.
    // Remember to emitter.Stop(); when the sound needs to stop of course.
    public StudioEventEmitter InitializeEventEmitter(EventReference eventReference, GameObject emitterGameObject)
    {
        StudioEventEmitter emitter = emitterGameObject.GetComponent<StudioEventEmitter>();
        emitter.EventReference = eventReference;
        eventEmittersList.Add(emitter);

        return emitter;
    }

    private void CleanUp()
    {
        // Stop and release any created event instances 
        foreach (EventInstance eventInstance in eventInstancesList)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }

        // Stop all the event emitters, if we don't they might hang around in any other scenes.
        foreach (StudioEventEmitter emitter in eventEmittersList)
        {
            emitter.Stop();
        }
    }

    private void OnDestroy()
    {
        if (instance != this) return;

        CleanUp();
        LevelVolume.Remove_OnCurrentVolumeChanged(SetArea);
        if (Submarine.Instance) Submarine.Instance.getSubmarineHealth().Remove_OnDie(OnDie);
        if (Submarine.Instance) Submarine.Instance.getSubmarineHealth().Remove_OnRespawn(OnRespawn);
        PauseMenu.Remove_OnPaused(OnPause);
        Cave.Remove_OnInsideChanged(OnCaveInsideChanged);
    }
}