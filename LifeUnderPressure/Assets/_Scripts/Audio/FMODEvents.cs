using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    public static FMODEvents instance { get; private set; }

    /*[field: Header("VO Tutorial Lines")]
    [field: SerializeField] 
    public EventReference voiceLine_Tutorial_01 { get; private set; }*/

    [field: Header("Atmosphere SFX")]
    [field: SerializeField]
    public EventReference ambienceToPlay { get; private set; }

    [field: SerializeField]
    public EventReference SFX_Whale { get; set; }

    [field: Header("Upgrades")]
    [field: SerializeField]
    public EventReference upgradeFX { get; private set; }

    [field: Header("Docking Station")]
    [field: SerializeField]
    public EventReference dockingSFX { get; private set; }

    [field: Header("Scanning")]
    [field: SerializeField]
    public EventReference scannedNotificationSFX { get; private set; }

    [field: SerializeField]
    public EventReference scanningSFX { get; private set; }

    [field: Header("Submarine SFX")]
    [field: SerializeField]
    public EventReference propellerSFX { get; private set; }

    [field: SerializeField]
    public EventReference sonarSFX { get; private set; }

    [field: SerializeField]
    public EventReference SFX_Warning { get; private set; }

    [field: SerializeField]
    public EventReference SFX_Collision {  get; private set; }

    [field: SerializeField]
    public EventReference SFX_Implosion { get; private set; }

    [field: SerializeField]
    public EventReference SFX_Death {  get; private set; }

    [field: SerializeField]
    public EventReference SFX_Cracking_Window { get; private set; }

    [field: SerializeField]
    public EventReference SFX_Lightswitch { get; private set; }

    [field: Header("Music Tracks")]
    [field: SerializeField]
    public EventReference musicToPlay { get; private set; }

    [field: Header("VO Tracks")]
    [field: SerializeField]
    public EventReference voice01 { get; private set; }


    private void Awake()
    {
        if (instance != null)
            Debug.LogError("Found more than one <FMODEvents> instance in the scene.");

        instance = this;
    }
}