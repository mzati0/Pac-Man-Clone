using System;
using System.Collections;
using Misc;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        OnePlayer,
        TwoPlayer,
        Attract
    }
    
    public static GameManager Instance { get; private set; }
    [Header("Speed")]
    public float BaseSpeed = 10f;
    [Header("Credits")]
    public int credits = 0;

    [Header("Score")]
    public int score = 0;
    public int p2Score = 0;
    public int highScore = 0;

    [Header("Ghost Scoring")]
    public int[] ghostScoreValues = { 200, 400, 800, 1600 }; 
    private int ghostsEatenThisFrightened;

    [Header("Level")]
    public int level = 1;

    [Header("Lives")]
    public int startingLives = 3;
    public int extraLifeScoreThreshold = 10000;
    public float deathDelay = 1.5f;
    private int lives;
    private bool extraLifeAwarded;
    public int Lives => lives;

    [Header("Maze")]
    public PelletSpawner pelletSpawner;

    [Header("Fruit")]
    public FruitSpawner fruitSpawner;

    [Header("Pac-Man")]
    public PacMovement pacManMovement;
    
    [Header("Text")]
    [SerializeField] private GameObject playerOneTextPrefab;
    [SerializeField] private GameObject readyTextPrefab;
    [SerializeField] private GameObject gameOverTextPrefab;
    

    [Header("Inputs")]
    [SerializeField] private InputActionReference creditAction;
    [SerializeField] private InputActionReference onePlayerAction;
    [SerializeField] private InputActionReference twoPlayerAction;
    [SerializeField] private InputActionReference anyKeyAction;

    
    [HideInInspector] public bool firstOpen = true;
    [HideInInspector] public int pelletsRemaining;
    private int pelletsEatenThisLevel;
    
    public GameState CurrentGameState { get; private set; } = GameState.Attract;

    public static event Action OnCreditChanged;
    public static event Action OnScoreChanged;
    public static event Action<float> OnFrightenedModeStarted;
    public static event Action OnFrightenedModeEnded;
    public static event Action OnAllPelletsEaten;
    public static event Action<int> OnLevelStarted;
    public static event Action<int> OnLivesChanged;
    public static event Action OnPacManDied;
    public static event Action OnGameOver;
    
    void OnEnable()
    {
        creditAction.action.performed += AddCredit;
        onePlayerAction.action.performed += LoadIntoGame;
        anyKeyAction.action.performed += AnyKeyPressed;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        creditAction.action.performed -= AddCredit;
        onePlayerAction.action.performed -= LoadIntoGame;
        anyKeyAction.action.performed -= AnyKeyPressed;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        
        creditAction.action.Disable();
        onePlayerAction.action.Disable();
        anyKeyAction.action.Disable();
    }
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        creditAction.action.Enable();
        onePlayerAction.action.Enable();
        anyKeyAction.action.Enable();
        
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            CurrentGameState = GameState.Attract;
        }
        else
        {
            CreditsText.Instance.HideCredits();
            CurrentGameState = GameState.OnePlayer;
            score = 0;
            level = 1;
            pelletSpawner = FindAnyObjectByType<PelletSpawner>();
            fruitSpawner = FindAnyObjectByType<FruitSpawner>();
            pacManMovement = FindAnyObjectByType<PacMovement>();
            
            lives = startingLives;

            if (pacManMovement != null)
                pacManMovement.ResetToStart();

            pelletsRemaining = pelletSpawner != null ? pelletSpawner.PelletCount : FindObjectsOfType<Pellet>().Length;
            OnLevelStarted?.Invoke(level);
            OnLivesChanged?.Invoke(lives);
            StartCoroutine(GameBeginStage1());
            OneUpFlash.Instance.StartFlash();
        }
    }

    private void AddCredit(InputAction.CallbackContext context)
    {
        if (firstOpen) return;

        var attract = FindAnyObjectByType<AttractScreen>();
        if (attract != null)
        {
            if (attract.ShowControls) return;
        }
        if (credits < 99)
        {
            credits++;
        }
        OnCreditChanged?.Invoke();
    }
    private void LoadIntoGame(InputAction.CallbackContext context)
    {
        if (!FindAnyObjectByType<AttractScreen>()) return;
        if (credits > 0)
        {
            credits--;
            SceneManager.LoadScene(1);
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
        if (score > highScore) highScore = score;
        OnScoreChanged?.Invoke();

        if (!extraLifeAwarded && score >= extraLifeScoreThreshold)
        {
            extraLifeAwarded = true;
            AddLife();
        }
    }

    public void AddLife()
    {
        lives++;
        OnLivesChanged?.Invoke(lives);
        SoundManager.Instance.PlayExtraLifeSound();
    }

    public void PelletEaten(int scoreValue)
    {
        AddScore(scoreValue);
        pelletsRemaining--;
        pelletsEatenThisLevel++;
        FindAnyObjectByType<GhostManager>().triggerDotInc();
        SoundManager.Instance.PlayEatingDots();

        if (fruitSpawner != null)
            fruitSpawner.NotifyPelletEaten(pelletsEatenThisLevel);

        if (pelletsRemaining <= 0)
        {
            OnAllPelletsEaten?.Invoke();
            StartCoroutine(AdvanceToNextLevelAfterDelay());
        }
    }

    public void PowerPelletEaten(int scoreValue)
    {
        PelletEaten(scoreValue);
        ghostsEatenThisFrightened = 0; 
        FindAnyObjectByType<GhostManager>().triggerFrightened();
    }

    public void GhostEaten()
    {
        int index = Mathf.Min(ghostsEatenThisFrightened, ghostScoreValues.Length - 1);
        AddScore(ghostScoreValues[index]);
        ghostsEatenThisFrightened++;
    }

    public void PacManDied()
    {
        OnPacManDied?.Invoke();

        
        if (fruitSpawner != null)
            fruitSpawner.DespawnActiveFruit();

        StartCoroutine(RespawnAfterDelay());
    }

    private IEnumerator RespawnAfterDelay()
    {
        SoundManager.Instance.StopAllAudio();
        pacManMovement.StopAnm();
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(deathDelay);
        HideGhosts();
        pacManMovement.triggerPacDeathAnm();
        SoundManager.Instance.PlayDeathSound();
        yield return new WaitForSecondsRealtime(1f); // Wait for death animation to finish
        pacManMovement.GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSecondsRealtime(1f);
        lives--;
        FindAnyObjectByType<GhostManager>().PacManDeath();
        FindAnyObjectByType<GhostManager>().useGlobleDotCounter = true;
        OnLivesChanged?.Invoke(lives);

        if (lives <= 0)
        {
            Instantiate(gameOverTextPrefab, new Vector3(9, 14, 0), Quaternion.identity);
            CreditsText.Instance.ShowCredits();
            yield return new WaitForSecondsRealtime(1.5f);
            OnGameOver?.Invoke();
            Time.timeScale = 1f;
            SceneManager.LoadScene(0);
            yield break;
        }
        
        pacManMovement.GetComponent<SpriteRenderer>().enabled = true;
        ShowGhosts();
        Time.timeScale = 1f;

        if (pacManMovement != null) pacManMovement.ResetToStart();
        StartCoroutine(GameBeginStage2());
        // TODO: reset ghosts to their spawn points here too (likely via GhostManager)
    }

    private void ShowGhosts()
    {
        foreach (var ghost in FindObjectsByType<GhostPathing>()) {
            ghost.GetComponent<SpriteRenderer>().enabled = true;
        }
    }

    private void HideGhosts()
    {
        foreach (var ghost in FindObjectsByType<GhostPathing>()) {
            ghost.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    private IEnumerator GameBeginStage1()
    {
        SoundManager.Instance.PlayStartMusic();
        lives++;
        OnLivesChanged?.Invoke(lives);
        HideGhosts();
        pacManMovement.GetComponent<SpriteRenderer>().enabled = false;
        var playerOneText = Instantiate(playerOneTextPrefab, new Vector3(9, 20, 0), Quaternion.identity);
        var readyText = Instantiate(readyTextPrefab, new Vector3(11, 14, 0), Quaternion.identity);
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(2.5f);
        Destroy(playerOneText);
        Destroy(readyText);
        ShowGhosts();
        pacManMovement.GetComponent<SpriteRenderer>().enabled = true;
        Time.timeScale = 1f;
        StartCoroutine(GameBeginStage2());
        lives--;
        OnLivesChanged?.Invoke(lives);
    }

    private IEnumerator GameBeginStage2()
    {
        pacManMovement.StopAnm();
        Time.timeScale = 0f;
        var readyText = Instantiate(readyTextPrefab, new Vector3(11, 14, 1), Quaternion.identity);
        yield return new WaitForSecondsRealtime(2.5f);
        pacManMovement.PlayAnm();
        Destroy(readyText);
        Time.timeScale = 1;
        SoundManager.Instance.PlayGhostMove();
    }


    private IEnumerator AdvanceToNextLevelAfterDelay()
    {
        Time.timeScale = 0f;
        SoundManager.Instance.StopAllAudio();
        pacManMovement.StopAnm();
        yield return new WaitForSecondsRealtime(1.5f);
        HideGhosts();
        TilemapFlash.Instance.Flash();
        yield return new WaitForSecondsRealtime(2f);
        Time.timeScale = 1f;
        pacManMovement.PlayAnm();
        ShowGhosts();
        NextLevel();
    }

    private void NextLevel()
    {
        level++;
        GhostManager.instance.NewLevel(level);
        pelletsEatenThisLevel = 0;

        if (pacManMovement != null)
            pacManMovement.ResetToStart();

        if (pelletSpawner != null)
        {
            pelletSpawner.ClearPellets();
            pelletSpawner.SpawnPellets();
            pelletsRemaining = pelletSpawner.PelletCount;
        }

        if (fruitSpawner != null)
            fruitSpawner.ResetForNewLevel(level);

        OnLevelStarted?.Invoke(level);
        StartCoroutine(GameBeginStage2());
    }
    
    private void AnyKeyPressed(InputAction.CallbackContext context)
    {
        if(FindAnyObjectByType<AttractScreen>() && FindAnyObjectByType<AttractScreen>().ShowControls)
        {
            FindAnyObjectByType<AttractScreen>().ShowControls = false;
            FindAnyObjectByType<AttractScreen>().Setup();
        }
    }
}