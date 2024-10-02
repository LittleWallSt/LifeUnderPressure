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

    private bool pressedPlay = false;
    private float timer = 0f;
    private void Start()
    {
        buttonsGrid.SetActive(true);
        controlsMenu.SetActive(false);
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

    }
    public void Button_Controls()
    {
        if (pressedPlay) return;

        buttonsGrid.SetActive(false);
        controlsMenu.SetActive(true);
    }
    public void Button_ControlsBack()
    {
        if (pressedPlay) return;

        buttonsGrid.SetActive(true);
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
}
