using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;


public class introManager : MonoBehaviour
{
    private const string ROAMING_AREA = "RoamingAreaCenter";

    [SerializeField] private VideoPlayer videoPlayer;

    void Start()
    {
        videoPlayer.loopPointReached += LoadRoaming;
    }

    void LoadRoaming(VideoPlayer vp)
    {
        Debug.Log("Intro finished");
        SceneManager.LoadScene(ROAMING_AREA);
    }
}
