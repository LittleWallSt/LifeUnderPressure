using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject buttons = null;
    [SerializeField] private GameObject controlsMenu = null;

    private static Action<bool> onPaused = null;
    public bool EnableMenu(bool state)
    {
        gameObject.SetActive(state);
        buttons.SetActive(state);
        controlsMenu.SetActive(false);
        Time.timeScale = state ? 0f : 1f;
        InternalSettings.EnableCursor(gameObject.activeSelf);
        Submarine.Instance.getSubmarineMovement().enabled = !state;

        Call_OnPaused(state);
        return state;
    }
    public void Button_Continue()
    {
        EnableMenu(false);
    }
    public void Button_Options()
    {

    }
    public void Button_Controls()
    {
        buttons.SetActive(false);
        controlsMenu.SetActive(true);
    }
    public void Button_ControlsBack()
    {
        buttons.SetActive(true);
        controlsMenu.SetActive(false);
    }
    public void Button_Save()
    {
        DataManager.SaveData();
    }
    public void Button_Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void Button_Quit()
    {
        Application.Quit();
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }

    private static void Call_OnPaused(bool paused)
    {
        if (onPaused != null) onPaused(paused);
    }
    public static void Assign_OnPaused(Action<bool> action)
    {
        onPaused += action;
    }
    public static void Remove_OnPaused(Action<bool> action)
    {
        onPaused -= action;
    }

    // Janko >>
    public void PlayClickSound()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.SFX_UI_Click, transform.position);
    }

    public void PlayHoverSound()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.SFX_UI_Hover, transform.position);
    }
    // Janko <<
}
