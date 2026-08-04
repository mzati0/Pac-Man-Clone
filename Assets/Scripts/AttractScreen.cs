using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Video;

public class AttractScreen : MonoBehaviour
{
    [Header("Startup")]
    [SerializeField] private VideoPlayer videoPlayer; 
    [NonSerialized] public bool ShowControls;
    [Header("UI Elements")]
    [SerializeField] private GameObject ui;
    [SerializeField] private GameObject attractScreenUI;
    [SerializeField] private GameObject readyScreenUI;
    [SerializeField] private GameObject controlsScreenUI;
    [SerializeField] private GameObject scoreUI;

    // [Header("Input Actions")]
    // [SerializeField] private InputActionReference onePlayerAction;
    // [SerializeField] private InputActionReference twoPlayerAction;

    private void OnEnable()
    {
        videoPlayer.loopPointReached += OnVideoFinished;
        GameManager.OnCreditChanged += Setup;
        // onePlayerAction.action.performed += StartGame;
    }
    
    private void OnDisable(){
        videoPlayer.loopPointReached -= OnVideoFinished;
        GameManager.OnCreditChanged -= Setup;
        // onePlayerAction.action.performed -= StartGame;

    }

    private void Start()
    {
        if (GameManager.Instance.firstOpen)
        {
            ui.SetActive(false);
            ShowControls = true;
        }
        else
        {
            Setup();
        }
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        GameManager.Instance.firstOpen = false;
        Setup();
    }

    public void Setup()
    {
        var canvas = FindAnyObjectByType<Canvas>();
        if (canvas != null) canvas.gameObject.SetActive(false);
        
        ui.SetActive(true);
        if (ShowControls)
        {
            HideScore();
            readyScreenUI.SetActive(false);
            attractScreenUI.SetActive(false);
            controlsScreenUI.SetActive(true);
        }

        else
        {
            ShowScore();
            if(GameManager.Instance.credits > 0)
            {
                readyScreenUI.SetActive(true);
                attractScreenUI.SetActive(false);
                controlsScreenUI.SetActive(false);
            }
            else
            {
                readyScreenUI.SetActive(false);
                attractScreenUI.SetActive(true);
                controlsScreenUI.SetActive(false);
            }
        }
    }
    private void ShowScore()
    {
        scoreUI.SetActive(true);
    }
    private void HideScore()
    {
        scoreUI.SetActive(false);
    }
    public void EndAttractScreen()
    {
        // TODO: Make game play a demo run
        // UPDATE: Nope xd
    }
}