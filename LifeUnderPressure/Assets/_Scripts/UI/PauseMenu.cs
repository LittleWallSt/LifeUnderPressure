using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject buttons = null;
    [SerializeField] private GameObject controlsMenu = null;
    [SerializeField] private GameObject controlsButton = null;
    [SerializeField] private GameObject settingsMenu = null;
    [SerializeField] private GameObject settings = null;

    private static Action<bool> onPaused = null;
    public bool EnableMenu(bool state)
    {
        gameObject.SetActive(state);
        buttons.SetActive(state);
        controlsMenu.SetActive(false);
        // Janko >>
        settingsMenu.SetActive(false);
        // Janko <<
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
        // Janko >>
        buttons.SetActive(false);
        settingsMenu.SetActive(true);
        // Janko <<
        settings.SetActive(true);
    }

    // Janko >>
    public void Button_SettingsBack()
    {
        buttons.SetActive(true);
        settingsMenu.SetActive(false);
        DataManager.SaveSettingsData();
    }
    // Janko <<

    public void Button_Controls()
    {
        controlsMenu.SetActive(true);
        settings.SetActive(false);
        controlsButton.SetActive(false);
    }
    public void Button_ExitToMenu()
    {
        SceneManager.LoadScene(InternalSettings.MainMenuSceneName);
        Time.timeScale = 1f;
    }
    public void Button_ControlsBack()
    {
        controlsMenu.SetActive(false);
        settings.SetActive(true);
        controlsButton.SetActive(true);
    }
    public void Button_Save()
    {
        DataManager.SaveMainData();
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

    public void PlayFishClickSound()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.SFX_UI_Click_Fish, transform.position);
    }
    // Janko <<
}
