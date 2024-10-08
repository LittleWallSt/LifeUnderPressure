using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistantSounds : MonoBehaviour
{
    private float shouldPlaySoundTimer = 0f;
    private float timeBetweenSounds = 75f;

    private void Start()
    {
        shouldPlaySoundTimer = 20f;
    }

    private void Update()
    {
        PlayDistantSound(FMODEvents.instance.SFX_Whale, transform.position);
    }

    private void PlayDistantSound(EventReference sound, Vector3 worldPos)
    {
        if (shouldPlaySoundTimer <= 0f)
        {
            AudioManager.instance.PlayOneShot(sound, worldPos);
            shouldPlaySoundTimer = timeBetweenSounds;
        }
        else
            shouldPlaySoundTimer -= Time.deltaTime;
    }
}