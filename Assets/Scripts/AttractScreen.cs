using System;
using UnityEngine;
using UnityEngine.InputSystem;
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
        // TODO: Make game play a demo run
    }

    private void Update()
    {
        if(Keyboard.current == null) return;
        if (Keyboard.current.anyKey.wasPressedThisFrame){
            
        }
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            Debug.Log("1 player");
            // Set game manager to one player mode
            //Start the game
        }

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            Debug.Log("2 player");
            // Set game manager to two player mode
            //Start the game
        }
    }
}