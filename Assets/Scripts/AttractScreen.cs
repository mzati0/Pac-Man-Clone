using UnityEngine;
using UnityEngine.Video;

public class MainMenu : MonoBehaviour
{
    private bool _firstOpen = true; //keep it in this class or make an even higher game manager that remembers this value across scenes?
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private GameObject mainMenu;

    void Start()
    {
        videoPlayer.loopPointReached += OnVideoFinished;
        
        if(_firstOpen)
        {
            mainMenu.SetActive(false);

        }
        else
        {
            Setup();
        }
    }

    void Setup()
    {
        FindAnyObjectByType<Canvas>().gameObject.SetActive(false);
        mainMenu.SetActive(true);
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        _firstOpen = false;
        Setup();
    }
}
