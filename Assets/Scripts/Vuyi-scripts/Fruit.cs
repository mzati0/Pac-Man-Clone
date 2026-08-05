using UnityEngine;

public class Fruit : MonoBehaviour
{
    [Header("Value")]
    public int points = 100;

    [Header("Lifetime")]
    public float lifetime = 10f;

    public string playerTag = "Player";

    [Header("Score Popup")]
    public Sprite scorePopupSprite;
    public float scorePopupDuration = 1f;
    public string scorePopupSortingLayer = "Default";
    public int scorePopupOrderInLayer = 10;

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
        SoundManager.Instance.PlayFruitEaten();

        SpawnScorePopup();

        Destroy(gameObject);
    }

    void SpawnScorePopup()
    {
        if (scorePopupSprite == null) return;

        GameObject popup = new GameObject("FruitScorePopup");
        popup.transform.position = transform.position;

        SpriteRenderer sr = popup.AddComponent<SpriteRenderer>();
        sr.sprite = scorePopupSprite;
        sr.sortingLayerName = scorePopupSortingLayer;
        sr.sortingOrder = scorePopupOrderInLayer;

        Destroy(popup, scorePopupDuration);
    }
}