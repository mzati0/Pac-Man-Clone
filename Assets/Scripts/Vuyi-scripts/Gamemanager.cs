using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        OnePlayer,
        TwoPlayer,
        Attract
    }
    public static GameManager Instance { get; private set; }
    [Header("Credits")]
    public int credits = 0;

    [Header("Score")]
    public int score = 0;
    public int p2Score = 0;
    public int highScore = 0;

    [Header("Level")]
    public int level = 1;
    public float levelCompleteDelay = 2f;

    [Header("Lives")]
    public int startingLives = 3;
    public int extraLifeScoreThreshold = 10000;
    public float deathDelay = 1.5f;
    private int lives;
    private bool extraLifeAwarded;
    public int Lives => lives;

    [Header("Frightened Mode")]
    public float frightenedDuration = 6f;
    private float frightenedTimer;
    public bool IsFrightened => frightenedTimer > 0f;

    [Header("Maze")]
    public PelletSpawner pelletSpawner;

    [Header("Fruit")]
    public FruitSpawner fruitSpawner;

    [Header("Pac-Man")]
    public PacMovement pacManMovement;

    [Header("Inputs")]
    [SerializeField] private InputActionReference creditAction;
    [SerializeField] private InputActionReference onePlayerAction;
    [SerializeField] private InputActionReference twoPlayerAction;
    
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
        
        creditAction.action.Enable();
    }

    void OnDisable()
    {
        creditAction.action.performed -= AddCredit;
        
        creditAction.action.Disable();
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

    void Start()
    {
        lives = startingLives;

        if (pacManMovement != null)
            pacManMovement.ResetToStart();

        pelletsRemaining = pelletSpawner != null ? pelletSpawner.PelletCount : FindObjectsOfType<Pellet>().Length;
        OnLevelStarted?.Invoke(level);
        OnLivesChanged?.Invoke(lives);
    }

    void Update()
    {
        if (frightenedTimer <= 0f) return;
        frightenedTimer -= Time.deltaTime;
        if (frightenedTimer <= 0f)
        {
            frightenedTimer = 0f;
            OnFrightenedModeEnded?.Invoke();
        }
    }
    
    public void AddCredit(InputAction.CallbackContext context)
    {
        if(credits < 99) credits++;
        OnCreditChanged?.Invoke();
        //play
    }

    public void AddScore(int amount)
    {
        score += amount;
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
    }

    public void PelletEaten(int scoreValue)
    {
        AddScore(scoreValue);
        pelletsRemaining--;
        pelletsEatenThisLevel++;
        FindAnyObjectByType<GhostManager>().triggerDotInc();

        if (fruitSpawner != null)
            fruitSpawner.NotifyPelletEaten(pelletsEatenThisLevel);

        if (pelletsRemaining <= 0)
        {
            OnAllPelletsEaten?.Invoke();
            Time.timeScale = 0f;
            StartCoroutine(AdvanceToNextLevelAfterDelay());
        }
    }

    public void PowerPelletEaten(int scoreValue)
    {
        PelletEaten(scoreValue);
        FindAnyObjectByType<GhostManager>().triggerFrightened();
        frightenedTimer = frightenedDuration;
        OnFrightenedModeStarted?.Invoke(frightenedDuration);
    }

    public void PacManDied()
    {
        OnPacManDied?.Invoke();
        StartCoroutine(RespawnAfterDelay());
    }

    private IEnumerator RespawnAfterDelay()
    {

        pacManMovement.StopAnm();
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(deathDelay);
        foreach (var ghost in FindObjectsByType<GhostPathing>()) {
            ghost.GetComponent<SpriteRenderer>().enabled = false;
        }
        pacManMovement.triggerPacDeathAnm();
        yield return new WaitForSecondsRealtime(1f); // Wait for death animation to finish
        pacManMovement.GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSecondsRealtime(0.2f);
        Time.timeScale = 1f;
        foreach (var ghost in FindObjectsByType<GhostPathing>()) {
            ghost.GetComponent<SpriteRenderer>().enabled = true;
        }
        pacManMovement.GetComponent<SpriteRenderer>().enabled = true;
        lives--;
        FindAnyObjectByType<GhostManager>().PacManDeath();
        FindAnyObjectByType<GhostManager>().useGlobleDotCounter = true;
        OnLivesChanged?.Invoke(lives);

        if (lives <= 0)
        {
            OnGameOver?.Invoke();
            Time.timeScale = 0f; // TODO: hook up a Game Over screen and restart the game
            yield break;
        }

        if (pacManMovement != null)
            pacManMovement.ResetToStart();

        // TODO: reset ghosts to their spawn points here too (likely via GhostManager)
    }

    private IEnumerator AdvanceToNextLevelAfterDelay()
    {
        yield return new WaitForSecondsRealtime(levelCompleteDelay);
        Time.timeScale = 1f;
        NextLevel();
    }

    private void NextLevel()
    {
        level++;
        GhostManager.instance.NewLevel(level);
        pelletsEatenThisLevel = 0;
        frightenedTimer = 0f;

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
    }
}