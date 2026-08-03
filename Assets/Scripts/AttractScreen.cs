using UnityEngine;
using UnityEngine.Video;

public class AttractScreen : MonoBehaviour
{
    private bool _firstOpen = true; // TODO: Move to GameManager if this should persist across scenes.
    [Header("Startup")]
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private GameObject mainMenu;
    void Start()
    {
        videoPlayer.loopPointReached += OnVideoFinished;

        if (_firstOpen)
        {
            mainMenu.SetActive(false);
        }
        else
        {
            Setup();
        }
    }
    void OnVideoFinished(VideoPlayer vp)
    {
        _firstOpen = false;
        Setup();
    }
    void Setup()
    {
        FindAnyObjectByType<Canvas>().gameObject.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void EndAttractScreen()
    {
        Setup();
    }
}