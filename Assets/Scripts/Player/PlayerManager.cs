using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    [SerializeField] private GameObject playerPrefab; // Reference to the player prefab

    private GameObject player;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Subscribe to the SceneManager.sceneLoaded event
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from the SceneManager.sceneLoaded event to prevent duplicate calls
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Clear the player reference if the scene has changed
        if (player != null && player.scene != scene)
        {
            Destroy(player);
            player = null;
            Debug.Log("Player reference cleared due to scene change.");
        }

        // Spawn player only if it's not found in the scene and the scene isn't the MainMenu
        if (scene.name != "MainMenu" && player == null)
        {
            Debug.Log($"Spawning player in scene {scene.name}.");
            SpawnPlayer(Vector3.zero); // Default spawn position
        }
        else if (player != null)
        {
            SetupMovementScript(); // Ensure movement script is properly set up
        }
    }

    public void SetPlayerPosition(Vector3 position)
    {
        StartCoroutine(WaitAndSetPosition(position));
    }

    private IEnumerator WaitAndSetPosition(Vector3 position)
    {
        while (player == null)
        {
            yield return null; // Wait until the player is instantiated
        }

        player.transform.position = position;
        Debug.Log($"Player position set to {position}.");
    }

    public GameObject GetPlayer()
    {
        return player;
    }

    public void SpawnPlayer(Vector3 spawnPosition)
    {
        if (playerPrefab == null)
        {
            Debug.LogError("Player Prefab is not assigned in PlayerManager!");
            return;
        }

        if (player == null)
        {
            player = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
            Debug.Log("Player successfully spawned.");

            // Validate spawn correctness
            Transform groundCheck = player.transform.Find("GroundCheck");
            if (groundCheck == null)
            {
                Debug.LogError("Spawned player prefab is missing a GroundCheck object!");
                return;
            }
        }
        else
        {
            Debug.LogWarning("Player already exists in the scene!");
        }

        SetupMovementScript();
        AssignTargetToCompanions();
    }

    private void SetupMovementScript()
    {
        if (player == null)
        {
            Debug.LogError("Player instance is null. Cannot setup MovementScript.");
            return;
        }

        MovementScript movementScript = player.GetComponent<MovementScript>();
        if (movementScript == null)
        {
            movementScript = player.AddComponent<MovementScript>();
            Debug.Log("MovementScript was missing and has been added to the player.");
        }

        // Assign the GroundCheck, if it exists
        Transform groundCheck = player.transform.Find("GroundCheck");
        if (groundCheck != null)
        {
            movementScript.SetupGroundCheck(groundCheck);
        }
        else
        {
            Debug.LogError(
                "GroundCheck transform could not be found on the player prefab. Please ensure the prefab is properly configured.");
        }
    }
    
    private void AssignTargetToCompanions()
    {
        if (player == null)
        {
            Debug.LogWarning("No player found when assigning companion targets.");
            return;
        }

        Transform targetPoint = player.transform.Find("TargetPoint");
        if (targetPoint == null)
        {
            Debug.LogWarning("Player prefab does not contain a TargetPoint child!");
            return;
        }

        foreach (var companion in FindObjectsOfType<CompanionFollow2D>())
        {
            companion.SetTarget(targetPoint);
            Debug.Log($"Assigned TargetPoint to {companion.name}");
        }
    }

}