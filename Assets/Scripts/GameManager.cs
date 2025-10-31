using System.Collections;
using Player;
using SaveSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    SaveData data = new SaveData();

    public float GameTime { get; private set; }
    public string CurrentScene { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Initialize()
    {
        GameTime = 0f;
        CurrentScene = SceneManager.GetActiveScene().name;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CurrentScene = scene.name;
    }

    private void Update()
    {
        GameTime += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GoToMainMenu();
        }
    }

    public void SaveProgress()
    {
        GameObject player = PlayerManager.Instance?.GetPlayer();
        if (player)
        {
            SaveData saveData = new SaveData
            {
                GameTime = GameTime,
                CurrentScene = SceneManager.GetActiveScene().name,
                PlayerPosition = player.transform.position
            };
            SaveSystem.SaveSystem.Save(saveData);
            Debug.Log(
                $"Saving game state - Time: {Time.time}, Scene: {saveData.CurrentScene}, Position: {saveData.PlayerPosition}");
        }
        else
        {
            SaveData saveData = new SaveData
            {
                GameTime = GameTime,
                CurrentScene = data.CurrentScene,
                PlayerPosition = data.PlayerPosition,
            };
            SaveSystem.SaveSystem.Save(saveData);
            
        }
    }

    public void LoadProgress()
    {
        Debug.Log("Loading game progress...");
        data = SaveSystem.SaveSystem.Load();
        if (data != null)
        {
            Debug.Log(
                $"Save data loaded - Time: {data.GameTime}, Scene: {data.CurrentScene}, Position: {data.PlayerPosition}");
            GameTime = data.GameTime;
            CurrentScene = data.CurrentScene;

            StartCoroutine(LoadSceneWithPlayerManager(data.CurrentScene, data.PlayerPosition));
        }
        else
        {
            Debug.LogWarning("No save data found to load");
        }
    }

    private IEnumerator LoadSceneWithPlayerManager(string sceneName, Vector3 playerPosition)
    {
        SceneManager.LoadScene(sceneName);

        // Wait for scene to load
        while (!SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            yield return null;
        }

        // Wait for PlayerManager to initialize player
        float timeout = 5f; // Set a timeout to avoid infinite waits
        float elapsed = 0f;
        while (PlayerManager.Instance == null || PlayerManager.Instance.GetPlayer() == null)
        {
            yield return null;
            elapsed += Time.deltaTime;

            if (elapsed >= timeout)
            {
                Debug.LogError("Timeout waiting for PlayerManager to initialize player.");
                yield break;
            }
        }

        // Set player position
        Debug.Log("Setting player position after reload...");
        PlayerManager.Instance.SetPlayerPosition(playerPosition);
    }

    public void GoToMainMenu()
    {
        SaveProgress();
        
        if (PlayerManager.Instance != null)
        {
            Destroy(PlayerManager.Instance.gameObject);
        }
        
        LoadMainMenu();
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}