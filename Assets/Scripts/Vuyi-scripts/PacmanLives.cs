using UnityEngine;

/// <summary>
/// Attach to the "Lives" GameObject. Drag its Pac-Man icon children into
/// lifeIcons in FILL-PRIORITY order — element 0 is the first icon to turn
/// off when Pac-Man loses a life (and the last one to come back on).
///
/// The life currently in play never gets its own icon — only reserve lives
/// are shown. With startingLives = 3, only 2 icons are on at the very start
/// of a game. Losing a life turns the next one off; earning the 10,000-point
/// extra life turns one back on. The 4th slot is a spare that only lights up
/// if you ever add a second extra-life threshold.
/// </summary>
public class PacmanLives : MonoBehaviour
{
    [Tooltip("Assign in fill-priority order: element 0 turns off first / comes back on last.")]
    public GameObject[] lifeIcons;

    void OnEnable() => GameManager.OnLivesChanged += HandleLivesChanged;
    void OnDisable() => GameManager.OnLivesChanged -= HandleLivesChanged;

    private void HandleLivesChanged(int lives)
    {
        // Reserve lives = total lives minus the one currently in play.
        int reserveCount = Mathf.Clamp(lives - 1, 0, lifeIcons.Length);

        for (int i = 0; i < lifeIcons.Length; i++)
        {
            if (lifeIcons[i] != null)
                lifeIcons[i].SetActive(i < reserveCount);
        }
    }
}