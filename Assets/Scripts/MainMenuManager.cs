 using UnityEngine;
 using UnityEngine.SceneManagement;

 public class MainMenuManager : MonoBehaviour
 {
     public void StartGame()
     {
         SceneManager.LoadScene("SampleScene");
     }
     
     public void OpenSettings()
     {
         Debug.Log("Settings menu opened! Add logic for settings here.");
     }
     
     public void ExitGame()
     {
         Debug.Log("Game Quit! Works only in Built Game.");
         Application.Quit(); // Quits the application
     }
 }
