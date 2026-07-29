using UnityEngine;

public class Fruit : MonoBehaviour
{
    [Header("Value")]
    public int points = 100;

    [Header("Lifetime")]
    public float lifetime = 10f;

   
    public string playerTag = "Player";

    void Reset()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }

    void Start()
    {
        if (lifetime > 0f)
            Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (GameManager.Instance != null)
            GameManager.Instance.AddScore(points);

        Destroy(gameObject);
    }
}