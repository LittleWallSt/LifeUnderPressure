using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string sceneToLoad = "SCENE";
    [SerializeField] private float delayToStart = 2.5f;
    [SerializeField] private Animator menuAnimator = null;
    [SerializeField] private GameObject buttonsGrid = null;
    [SerializeField] private GameObject controlsMenu = null;
    [SerializeField] private GameObject controlsButton = null;

    // Janko >>
    [SerializeField] private GameObject settingsMenu = null;
    // Janko <<
    [SerializeField] private GameObject settings = null;

    private bool pressedPlay = false;
    private float timer = 0f;
    private void Start()
    {
        buttonsGrid.SetActive(true);
        controlsMenu.SetActive(false);
        // Janko >>
        settingsMenu.SetActive(false);
        // Janko <<
    }
    private void Update()
    {
        PressPlayProcess();
    }

    private void PressPlayProcess()
    {
        if (!pressedPlay) return;

        timer += Time.deltaTime;
        if (timer > delayToStart)
        {
            SceneManager.LoadScene(sceneToLoad);
            pressedPlay = false;
        }
    }

    public void Button_Play()
    {
        pressedPlay = true;
        menuAnimator.SetTrigger("PressPlay");
    }
    public void Button_Options()
    {
        if (pressedPlay) return;

        // Janko >>
        buttonsGrid.SetActive(false);
        settingsMenu.SetActive(true);
        // Janko <<
    }

    // Janko >>
    public void Button_SettingsBack()
    {
        if (pressedPlay) return;

        buttonsGrid.SetActive(true);
        settingsMenu.SetActive(false);
    }
    // Janko <<

    public void Button_Controls()
    {
        if (pressedPlay) return;

        settings.SetActive(false);
        controlsButton.SetActive(false);
        controlsMenu.SetActive(true);
    }
    public void Button_ControlsBack()
    {
        if (pressedPlay) return;

        settings.SetActive(true);
        controlsButton.SetActive(true);
        controlsMenu.SetActive(false);
    }
    public void Button_Quit()
    {
        if (pressedPlay) return;

        Application.Quit();
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }

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
