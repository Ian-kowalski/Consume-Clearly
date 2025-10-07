using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void StartGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadGame();
        }
        SceneManager.LoadScene("SampleScene");
    }
    
    public void OpenSettings()
    {
        Debug.Log("Settings menu opened! Add logic for settings here.");
    }
    
    public void ExitGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SaveGame();
        }
        Debug.Log("Game Quit! Works only in Built Game.");
        Application.Quit();
    }

    public void NewGame()
    {
        SaveSystem.DeleteSave();
        StartGame();
    }
}