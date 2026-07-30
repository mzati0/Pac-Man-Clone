using UnityEngine;

public class Pellet : MonoBehaviour
{
    public enum PelletType { Normal, Power }

    [Header("Pellet Setup")]
    public PelletType type = PelletType.Normal;
    public int scoreValue = 10;
    public string playerTag = "Player";

    void Reset()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (GameManager.Instance == null)
        {
            Debug.LogWarning("[Pellet] No GameManager found in scene.");
            return;
        }

        if (type == PelletType.Power)
            GameManager.Instance.PowerPelletEaten(scoreValue);
        else
            GameManager.Instance.PelletEaten(scoreValue);

        gameObject.SetActive(false);
    }
}