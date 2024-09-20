using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicChangeTrigger : MonoBehaviour
{
    [Header("MusicArea")]
    [SerializeField]
    private MusicArea area;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //AudioManager.instance.SetMusicArea(area);
        }
    }
}