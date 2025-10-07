using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    private float gameTime;
    private SaveData currentSaveData;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSaveSystem();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeSaveSystem()
    {
        currentSaveData = new SaveData();
        LoadGame();
    }

    private void Update()
    {
        if (currentSaveData != null)
        {
            currentSaveData.playTime += Time.deltaTime;
        }
    }

    public void SaveGame()
    {
        SaveSystem.Save(currentSaveData);
    }

    public void LoadGame()
    {
        currentSaveData = SaveSystem.Load();
        if (currentSaveData == null)
        {
            currentSaveData = new SaveData();
        }
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SaveGame(); // Save before loading main menu
        SceneManager.LoadScene("MainMenu");
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }
}