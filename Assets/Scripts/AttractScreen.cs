using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Video;

public class AttractScreen : MonoBehaviour
{
    [Header("Startup")]
    [SerializeField] private VideoPlayer videoPlayer; 
    [FormerlySerializedAs("mainMenu")] [SerializeField] private GameObject ui;
    [SerializeField] private GameObject attractScreenUI;
    [SerializeField] private GameObject readyScreenUI;

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

    private void Setup()
    {
        var canvas = FindAnyObjectByType<Canvas>();
        if (canvas != null) canvas.gameObject.SetActive(false);
        
        ui.SetActive(true);
        if(GameManager.Instance.credits > 0)
        {
            readyScreenUI.SetActive(true);
            attractScreenUI.SetActive(false);
        }
        else
        {
            readyScreenUI.SetActive(false);
            attractScreenUI.SetActive(true);
        }
    }
    
    // private void StartGame(InputAction.CallbackContext context)
    // {
    //     if (GameManager.Instance.credits > 0)
    //     {
    //         print("suffer");
    //     }
    // }
    public void EndAttractScreen()
    {
        // TODO: Make game play a demo run
        // UPDATE: Nope xd
    }
}