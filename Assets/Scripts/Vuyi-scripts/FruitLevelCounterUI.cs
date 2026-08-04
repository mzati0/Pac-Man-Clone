using UnityEngine;
using System.Collections.Generic;

public class FruitLevelCounterUI : MonoBehaviour
{
    
    public Sprite[] fruitSprites;

    
    public Transform[] slots;

    private SpriteRenderer[] slotRenderers;
    private readonly List<int> fruitHistory = new List<int>();

    void Awake()
    {
        slotRenderers = new SpriteRenderer[slots.Length];
        for (int i = 0; i < slots.Length; i++)
            slotRenderers[i] = slots[i].GetComponent<SpriteRenderer>();
    }

    void OnEnable() => GameManager.OnLevelStarted += HandleLevelStarted;
    void OnDisable() => GameManager.OnLevelStarted -= HandleLevelStarted;

    private void HandleLevelStarted(int level)
    {
        if (level <= 1)
            fruitHistory.Clear(); 

        fruitHistory.Add(GetFruitIndexForLevel(level));
        if (fruitHistory.Count > slots.Length)
            fruitHistory.RemoveAt(0); 

        RefreshSlots();
    }

    private void RefreshSlots()
    {
        int emptySlots = slots.Length - fruitHistory.Count;

        for (int i = 0; i < slots.Length; i++)
        {
            int historyIndex = i - emptySlots;
            bool active = historyIndex >= 0;

            slots[i].gameObject.SetActive(active);
            if (active)
                slotRenderers[i].sprite = fruitSprites[fruitHistory[historyIndex]];
        }
    }

    
    public static int GetFruitIndexForLevel(int level)
    {
        if (level <= 1) return 0; // Cherry
        if (level == 2) return 1; // Strawberry
        if (level <= 4) return 2; // Orange
        if (level <= 6) return 3; // Apple
        if (level <= 8) return 4; // Melon
        if (level <= 10) return 5; // Galaxian
        if (level <= 12) return 6; // Bell
        return 7; // Key, capped for all levels 13+
    }
}