using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    private enum VolumeType
    {
        MASTER,
        SFX,
        MUSIC,
        AMBIENCE,
        VO
    }

    [Header("Type")]
    [SerializeField] private VolumeType volumeType;

    private Slider volumeSlider;

    private void Awake()
    {
        volumeSlider = GetComponentInChildren<Slider>();
    }
    private void OnEnable()
    {
        if (!AudioManager.instance) return;
        switch (volumeType)
        {
            case VolumeType.MASTER:
                volumeSlider.value = AudioManager.instance.masterVolume;
                break;
            case VolumeType.SFX:
                volumeSlider.value = AudioManager.instance.gameSoundVolume;
                break;
            case VolumeType.MUSIC:
                volumeSlider.value = AudioManager.instance.musicVolume;
                break;
            case VolumeType.AMBIENCE:
                volumeSlider.value = AudioManager.instance.ambienceVolume;
                break;
            case VolumeType.VO:
                volumeSlider.value = AudioManager.instance.voVolume;
                break;
            default:
                Debug.LogWarning("Volume Type not supported: " + volumeType);
                break;
        }
    }
    public void OnSliderValueChanged()
    {
        if (!AudioManager.instance || !InternalSettings.DataLoaded) return;
        switch (volumeType)
        {
            case VolumeType.MASTER:
                AudioManager.instance.masterVolume = volumeSlider.value;
                DataManager.WriteSettings("Volume_Master", (int)(volumeSlider.value * 100f));
                break;
            case VolumeType.SFX:
                AudioManager.instance.gameSoundVolume = volumeSlider.value;
                DataManager.WriteSettings("Volume_SFX", (int)(volumeSlider.value * 100f));
                break;
            case VolumeType.MUSIC:
                AudioManager.instance.musicVolume = volumeSlider.value;
                DataManager.WriteSettings("Volume_Music", (int)(volumeSlider.value * 100f));
                break;
            case VolumeType.AMBIENCE:
                AudioManager.instance.ambienceVolume = volumeSlider.value;
                DataManager.WriteSettings("Volume_Ambience", (int)(volumeSlider.value * 100f));
                break;
            case VolumeType.VO:
                AudioManager.instance.voVolume = volumeSlider.value;
                DataManager.WriteSettings("Volume_Voiceover", (int)(volumeSlider.value * 100f));
                break;
            default:
                Debug.LogWarning("Volume Type not supported: " + volumeType);
                break;
        }
    }
}