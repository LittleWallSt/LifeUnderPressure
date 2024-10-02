using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject buttons = null;
    [SerializeField] private GameObject controlsMenu = null;

    public bool EnableMenu(bool state)
    {
        gameObject.SetActive(state);
        buttons.SetActive(state);
        controlsMenu.SetActive(false);
        Time.timeScale = state ? 0f : 1f;
        InternalSettings.EnableCursor(gameObject.activeSelf);
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
}
