using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    public Vector3 PlayerPosition => player ? player.transform.position : Vector3.zero;

    private GameObject player;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                Debug.LogError("No player found in scene! Make sure your player has the 'Player' tag.");
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetPlayerPosition(Vector3 position)
    {
        if (player != null)
        {
            player.transform.position = position;
        }
    }

    public GameObject GetPlayer()
    {
        return player;
    }
}