using UnityEngine;

public class FruitLevelCounterUI : MonoBehaviour
{
   
    public Transform[] slots;

    void OnEnable() => GameManager.OnLevelStarted += HandleLevelStarted;
    void OnDisable() => GameManager.OnLevelStarted -= HandleLevelStarted;

    private void HandleLevelStarted(int level)
    {
        int tiersReached = GetFruitIndexForLevel(level) + 1;
        int activeCount = Mathf.Min(tiersReached, slots.Length);

        for (int i = 0; i < slots.Length; i++)
            slots[i].gameObject.SetActive(i < activeCount);
    }

    private int GetFruitIndexForLevel(int level)
    {
        if (level <= 1) return 0; // Cherry
        if (level == 2) return 1; // Strawberry
        if (level <= 4) return 2; // Orange
        if (level <= 6) return 3; // Apple
        if (level <= 8) return 4; // Melon
        if (level <= 10) return 5; // Galaxian
        if (level <= 12) return 6; // Bell
        return 7; 
    }
}