using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Score")]
    public int score = 0;

    [Header("Level")]
    public int level = 1;
    public float levelCompleteDelay = 2f;

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

    [HideInInspector] public int pelletsRemaining;
    private int pelletsEatenThisLevel;

    public static event Action<int> OnScoreChanged;
    public static event Action<float> OnFrightenedModeStarted;
    public static event Action OnFrightenedModeEnded;
    public static event Action OnAllPelletsEaten;
    public static event Action<int> OnLevelStarted;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        if (pacManMovement != null)
            pacManMovement.ResetToStart();

        pelletsRemaining = pelletSpawner != null ? pelletSpawner.PelletCount : FindObjectsOfType<Pellet>().Length;
        OnLevelStarted?.Invoke(level);
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

    public void AddScore(int amount)
    {
        score += amount;
        OnScoreChanged?.Invoke(score);
    }

public void PelletEaten(int scoreValue)
{
    AddScore(scoreValue);
    pelletsRemaining--;
    pelletsEatenThisLevel++;
    gameObject.GetComponent<GhostManager>().triggerDotInc();

    if (fruitSpawner != null)
        fruitSpawner.NotifyPelletEaten(pelletsEatenThisLevel);

    if (pelletsRemaining <= 0)
    {
        OnAllPelletsEaten?.Invoke();
        Time.timeScale = 0f; // pause everything immediately
        StartCoroutine(AdvanceToNextLevelAfterDelay());
    }
}

private IEnumerator AdvanceToNextLevelAfterDelay()
{
    yield return new WaitForSecondsRealtime(levelCompleteDelay);
    Time.timeScale = 1f; 
    NextLevel();
}

    public void PowerPelletEaten(int scoreValue)
    {
        PelletEaten(scoreValue);
        frightenedTimer = frightenedDuration;
        OnFrightenedModeStarted?.Invoke(frightenedDuration);
    }


    private void NextLevel()
    {
        level++;
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