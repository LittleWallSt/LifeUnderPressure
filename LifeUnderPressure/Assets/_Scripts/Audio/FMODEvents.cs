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

    [field: Header("Music Tracks")]
    [field: SerializeField]
    public EventReference musicToPlay { get; private set; }


    private void Awake()
    {
        if (instance != null)
            Debug.LogError("Found more than one <FMODEvents> instance in the scene.");

        instance = this;
    }
}