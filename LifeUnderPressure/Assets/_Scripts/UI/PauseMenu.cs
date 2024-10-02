using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public bool EnableMenu(bool state)
    {
        gameObject.SetActive(state);
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
