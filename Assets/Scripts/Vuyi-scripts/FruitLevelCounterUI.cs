using UnityEngine;
using System.Collections.Generic;

public class FruitLevelCounterUI : MonoBehaviour
{
    public Sprite[] fruitSprites;

    public Transform[] slots;

    public bool slotsOrderedLeftToRight = true;

    private SpriteRenderer[] slotRenderers;
    private readonly List<int> fruitHistory = new List<int>();

    void Awake()
    {
        slotRenderers = new SpriteRenderer[slots.Length];
        for (int i = 0; i < slots.Length; i++)
        {
            slotRenderers[i] = slots[i].GetComponent<SpriteRenderer>();
            slots[i].gameObject.SetActive(false); 
        }
    }

    void OnEnable() => GameManager.OnLevelStarted += HandleLevelStarted;
    void OnDisable() => GameManager.OnLevelStarted -= HandleLevelStarted;

    private void HandleLevelStarted(int level)
    {
        if (level <= 1)
            ResetRow();

        if (fruitHistory.Count >= slots.Length)
            return;

        int fruitIndex = GetFruitIndexForLevel(level);
        if (fruitIndex < 0 || fruitIndex >= fruitSprites.Length)
            return;

        fruitHistory.Add(fruitIndex);
        ActivateSlotFor(fruitHistory.Count - 1, fruitIndex); 
    }

    private void ResetRow()
    {
        fruitHistory.Clear();
        for (int i = 0; i < slots.Length; i++)
            slots[i].gameObject.SetActive(false);
    }


    private void ActivateSlotFor(int entryIndex, int fruitIndex)
    {
        int slotIndex = slotsOrderedLeftToRight ? (slots.Length - 1 - entryIndex) : entryIndex;

        slots[slotIndex].gameObject.SetActive(true);
        slotRenderers[slotIndex].sprite = fruitSprites[fruitIndex];
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