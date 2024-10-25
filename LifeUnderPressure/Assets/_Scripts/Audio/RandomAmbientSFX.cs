using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAmbientSFX : MonoBehaviour
{
    [SerializeField]
    private float minStartTimer = 15f, maxStartTimer = 25f;
    [SerializeField]
    private float minNextSoundTime = 60f, maxNextSoundTime = 90f;

    private float nextSoundTime;
    private float timer = 0f;

    private void Start()
    {
        timer = Random.Range(minStartTimer, maxStartTimer);
    }

    private void Update()
    {
        if (LevelVolume.Current.Level == 2)
        {
            if (timer < 0f)
            {
                AudioManager.instance.PlayOneShot(FMODEvents.instance.AMB_Random_Ambient_SFX, transform.position);

                nextSoundTime = Random.Range(minNextSoundTime, maxNextSoundTime);
                timer = nextSoundTime;
            }
            else
            {
                timer -= Time.deltaTime;
            }  
        }
    }
}