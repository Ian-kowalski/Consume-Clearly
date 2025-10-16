using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsManager : MonoBehaviour
{
    private const string MAIN_MENU_SCENE = "MainMenu";
    
    [SerializeField] private Animation creditsTextAnimation;

    private void Start()
    {
        // If you have multiple animations, you can play the specific one you want
        if (creditsTextAnimation != null && creditsTextAnimation.clip != null)
        {
            creditsTextAnimation.Play();
        }
    }

    public void ReturnToMainMenu()
    {
        // Stop the creditsTextAnimation before switching scenes
        if (creditsTextAnimation != null)
        {
            creditsTextAnimation.Stop();
        }

        SceneManager.LoadScene(MAIN_MENU_SCENE);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape key pressed - Returning to main menu");
            ReturnToMainMenu();
        }

        if (creditsTextAnimation != null && !creditsTextAnimation.isPlaying)
        {
            Debug.Log("Credits creditsTextAnimation completed - Automatically returning to main menu");
            ReturnToMainMenu();
        }
    }
}