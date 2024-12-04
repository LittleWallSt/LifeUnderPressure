using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string sceneToLoad = "SCENE";
    [SerializeField] private float delayToStart = 2.5f;
    [SerializeField] private Animator menuAnimator = null;
    [SerializeField] private GameObject sceneLoadedText = null;
    [SerializeField] private GameObject buttonsGrid = null;
    [SerializeField] private GameObject controlsMenu = null;
    [SerializeField] private GameObject controlsButton = null;

    // Janko >>
    [SerializeField] private GameObject settingsMenu = null;
    // Janko <<
    [SerializeField] private GameObject settings = null;

    private bool pressedPlay = false;
    private float timer = 0f;

    private AsyncOperation operation = null;
    private bool playSceneLoaded = false;
    private void Start()
    {
        sceneLoadedText.SetActive(false);
        buttonsGrid.SetActive(true);
        controlsMenu.SetActive(false);
        // Janko >>
        settingsMenu.SetActive(false);
        // Janko <<
    }
    private void Update()
    {
        //PressPlayProcess();
        SceneLoadingProcess();
    }

    private void SceneLoadingProcess()
    {
        if (operation == null) return;

        if (!playSceneLoaded && operation.progress >= 0.9f)
        {
            playSceneLoaded = true;
            sceneLoadedText.SetActive(true);
        }
        if (playSceneLoaded && Input.anyKeyDown)
        {
            operation.allowSceneActivation = true;
            operation = null;
        }
    }

    private void PressPlayProcess()
    {
        if (!pressedPlay) return;

        timer += Time.deltaTime;
        if (timer > delayToStart)
        {
            pressedPlay = false;
        }
    }

    public async void Button_Play()
    {
        pressedPlay = true;
        menuAnimator.SetTrigger("PressPlay");
        //operation = SceneManager.LoadSceneAsync(sceneToLoad);
        //operation.allowSceneActivation = false;
        buttonsGrid.SetActive(false);
        await LoadSceneWithProgress();
    }
    private async Task LoadSceneWithProgress()
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneToLoad);

        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            // Update progress bar
            //progressBar.value = Mathf.Clamp01(asyncOperation.progress / 0.9f);

            // Check if loading is done
            if (asyncOperation.progress >= 0.9f)
            {
                //Debug.Log("Press any key to activate the scene...");

                sceneLoadedText.SetActive(true);
                if (Input.anyKeyDown)
                {
                    asyncOperation.allowSceneActivation = true;
                }
            }

            await Task.Yield(); // Wait for the next frame
        }
    }
    public void Button_Options()
    {
        if (pressedPlay) return;

        // Janko >>
        buttonsGrid.SetActive(false);
        settingsMenu.SetActive(true);
        // Janko <<
    }
    public void Button_Credits()
    {
        if (pressedPlay) return;

        SceneManager.LoadScene("Credits");
    }
    public void Button_DeleteSave()
    {
        if (pressedPlay) return;

        DataManager.Clear();
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
}
