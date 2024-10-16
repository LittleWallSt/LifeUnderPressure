using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    // ANYTHING THAT USES THIS SCRIPT WILL NEED using FMODUnity; and maybe using FMOD.Studio;
    // Add [RequireComponent(typeof(StudioEventEmitter))] on top of the classes that will have Event Emitters.

    public static AudioManager instance { get; private set; }

    private List<EventInstance> eventInstancesList;
    private List<StudioEventEmitter> eventEmittersList;

    // We will use all ambience sound in a single event for each level? Use this approach then.
    private EventInstance ambienceEventInstance;
    private EventInstance musicEventInstance;

    private EventInstance currentInstance;
    private float timeLastSetInstance;

    [SerializeField]
    private float musicChangeCooldown = 5f;
    [SerializeField]
    private float playMusicOrAmbienceDelay = 10f;

    private void Awake()
    {
        if (instance != null)
            Debug.LogError("Found more than one <AudioManager> instance in the scene.");

        instance = this;

        eventInstancesList = new List<EventInstance>();
        eventEmittersList = new List<StudioEventEmitter>();
    }

    private void Start()
    {
        // Start ambience here or somewhere else?
        LevelVolume.Assign_OnCurrentVolumeChanged(SetArea);
        InitializeMusic(FMODEvents.instance.musicToPlay);
        InitializeAmbience(FMODEvents.instance.ambienceToPlay);
        //shouldPlaySoundTimer = timeBetweenSounds;

        currentInstance = musicEventInstance;
        currentInstance.start();
    }

    private void Update()
    {
        SetMusicOrAmbience();

        /*if (!IsPlaying(musicEventInstance))
        {
            InitializeAmbience(FMODEvents.instance.ambienceToPlay);
        }
        else if (!IsPlaying(ambienceEventInstance))
        {
            InitializeMusic(FMODEvents.instance.musicToPlay);
        }*/
    }

    private void SetMusicOrAmbience()
    {
        if (!IsPlaying(currentInstance) && Time.time - timeLastSetInstance > musicChangeCooldown)
        {
            currentInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            currentInstance = currentInstance.Equals(musicEventInstance) ? ambienceEventInstance : musicEventInstance;
            StartCoroutine(StartInstanceDelay(2f));
            timeLastSetInstance = Time.time;
        }
    }

    private IEnumerator StartInstanceDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        currentInstance.start();
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
        CleanUp();
        LevelVolume.Remove_OnCurrentVolumeChanged(SetArea);
    }
}