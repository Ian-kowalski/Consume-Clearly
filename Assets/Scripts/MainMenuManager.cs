using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    private const string CREDITS_SCENE_NAME = "Credits";
    private const string GAME_SCENE_NAME = "SampleScene";

    private void Start()
    {
        SetupAllMenuButtons();
    }
    
    private void SetupAllMenuButtons()
    {
        var buttons = GetComponentsInChildren<Button>(true);
        
        foreach (var button in buttons)
        {
            var text = button.GetComponentInChildren<TMP_Text>(true);
            var effect = text.gameObject.AddComponent<MenuButtonEffect>();
            
            var buttonNameLower = button.name.ToLower();
            if (buttonNameLower.Contains("continue"))
            {
                effect.SetEnabled(false);
            }
        }
    }
    public void ContinueGame()
    {
        // Functionality disabled for now
        Debug.Log("Continue functionality not implemented yet");
    }

    private void StartGame()
    {
        SceneManager.LoadScene(GAME_SCENE_NAME);
    }
    
    public void NewGame()
    {
        StartGame();
    }
    
    public void OpenSettings()
    {
        Debug.Log("Settings menu opened! Implementation pending.");
    }

    public void LoadCredits()
    {
        SceneManager.LoadScene(CREDITS_SCENE_NAME);
    }
    
    public void ExitGame()
    {
        if (GameManager.Instance != null)
        {
			GameManager.Instance.QuitGame();
        }
    }
}