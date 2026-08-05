using UnityEngine;

public class Pellet : MonoBehaviour
{
    public enum PelletType { Normal, Power }

    [Header("Pellet Setup")]
    public PelletType type = PelletType.Normal;
    public int scoreValue = 10;
    public string playerTag = "Player";

    [Header("Power Pellet Flash")]
    public float flashInterval = 0.25f;

    private SpriteRenderer sr;
    private float flashTimer;

    void Reset()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        flashTimer = Random.Range(0f, flashInterval);
    }

    void Update()
    {
        if (type != PelletType.Power || sr == null) return;

        
        flashTimer += Time.unscaledDeltaTime;
        if (flashTimer >= flashInterval)
        {
            flashTimer = 0f;
            sr.enabled = !sr.enabled;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (GameManager.Instance == null)
            return;

        if (type == PelletType.Power)
            GameManager.Instance.PowerPelletEaten(scoreValue);
        else
            GameManager.Instance.PelletEaten(scoreValue);

        gameObject.SetActive(false);
    }
}