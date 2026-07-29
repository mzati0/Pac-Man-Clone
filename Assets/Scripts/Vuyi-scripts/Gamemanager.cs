using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Score")]
    public int score = 0;

    [Header("Frightened Mode")]
    public float frightenedDuration = 6f;
    private float frightenedTimer;
    public bool IsFrightened => frightenedTimer > 0f;

    [Header("Fruit")]
    public FruitSpawner fruitSpawner;

    [Header("Debug")]
    public bool debugMode = true;

    [HideInInspector] public int pelletsRemaining;
    private int pelletsEatenThisLevel;

    public static event Action<int> OnScoreChanged;
    public static event Action<float> OnFrightenedModeStarted; 
    public static event Action OnFrightenedModeEnded;
    public static event Action OnAllPelletsEaten;

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
        pelletsRemaining = FindObjectsOfType<Pellet>().Length;
        if (debugMode)
            Debug.Log($"[GameManager] level start, {pelletsRemaining} pellets to clear");
    }

    void Update()
    {
        if (frightenedTimer <= 0f) return;

        frightenedTimer -= Time.deltaTime;
        if (frightenedTimer <= 0f)
        {
            frightenedTimer = 0f;
            if (debugMode) Debug.Log("[GameManager] frightened mode ended");
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

        if (fruitSpawner != null)
            fruitSpawner.NotifyPelletEaten(pelletsEatenThisLevel);

        if (debugMode)
            Debug.Log($"[GameManager] pellet eaten (+{scoreValue}), {pelletsRemaining} remaining");

        if (pelletsRemaining <= 0)
        {
            if (debugMode) Debug.Log("[GameManager] all pellets eaten - level clear");
            OnAllPelletsEaten?.Invoke();
        }
    }

    public void PowerPelletEaten(int scoreValue)
    {
        PelletEaten(scoreValue);

        frightenedTimer = frightenedDuration;

        if (debugMode)
            Debug.Log($"[GameManager] power pellet eaten - frightened mode for {frightenedDuration}s");

        OnFrightenedModeStarted?.Invoke(frightenedDuration);
    }
}
