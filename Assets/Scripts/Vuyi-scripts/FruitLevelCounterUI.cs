using System.Collections.Generic;
using UnityEngine;

public class FruitLevelCounterUI : MonoBehaviour
{
    public Transform[] slots;

    private readonly List<int> recentLevels = new List<int>();

    void OnEnable() => GameManager.OnLevelStarted += HandleLevelStarted;
    void OnDisable() => GameManager.OnLevelStarted -= HandleLevelStarted;

    private void HandleLevelStarted(int level)
    {
        recentLevels.Add(level);
        if (recentLevels.Count > slots.Length)
            recentLevels.RemoveAt(0);

        RefreshSlots();
    }

    private void RefreshSlots()
    {
        int emptySlots = slots.Length - recentLevels.Count;

        for (int i = 0; i < slots.Length; i++)
        {
            int historyIndex = i - emptySlots;
            int activeChild = historyIndex >= 0 ? GetFruitIndexForLevel(recentLevels[historyIndex]) : -1;

            for (int c = 0; c < slots[i].childCount; c++)
                slots[i].GetChild(c).gameObject.SetActive(c == activeChild);
        }
    }

    private int GetFruitIndexForLevel(int level)
    {
        if (level <= 1) return 0;
        if (level == 2) return 1;
        if (level <= 4) return 2;
        if (level <= 6) return 3;
        if (level <= 8) return 4;
        if (level <= 10) return 5;
        if (level <= 12) return 6;
        return 7;
    }
}