using UnityEngine;

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