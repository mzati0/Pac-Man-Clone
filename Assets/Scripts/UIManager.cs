using System;
using System.Collections;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private int _displayedCredits = 0;
    private Coroutine _creditCoroutine;
    [SerializeField] private AudioClip creditSound;
    
    [SerializeField] private DigitDisplay credits;
    [SerializeField] private DigitDisplay p1Score;
    [SerializeField] private DigitDisplay p2Score;
    [SerializeField] private DigitDisplay highScore;
    
    [Header("Sprites")]
    [SerializeField] private Sprite[] digitSprites; // Array of sprites for digits 0-9

    
    private void OnEnable()
    {
        GameManager.OnCreditChanged += OnCreditChanged;
        GameManager.OnScoreChanged += UpdateScore;
    }
    
    private void OnDisable()
    {
        GameManager.OnCreditChanged -= OnCreditChanged;
        GameManager.OnScoreChanged -= UpdateScore;
    }

    private void OnCreditChanged()
    {
        _creditCoroutine ??= StartCoroutine(UpdateDisplayedCredits());
    }

    private void Start()
    {
        _displayedCredits = GameManager.Instance.credits;
        UpdateUICredits();
        UpdateScore();
    }

    private IEnumerator UpdateDisplayedCredits()
    {
        while (_displayedCredits < GameManager.Instance.credits)
        {
            SoundManager.Instance.PlayOneShot(creditSound);
            _displayedCredits++;
            UpdateUICredits();
            yield return new WaitForSeconds(0.3f);
        }
        _creditCoroutine = null;
    }

    private void UpdateUICredits()
    {
        if (credits.Length == 0) return;
        credits.SetDigit(0, digitSprites[_displayedCredits % 10]); // Ones place
        credits.SetDigit(1, (_displayedCredits / 10) % 10 > 0 ? digitSprites[(_displayedCredits / 10) % 10] : null); // Clear tens place if it's zeTens place}
    }

        private void UpdateScore()
        {
            // Idk either big dawg, my eyes hurt just looking at it
            if (p1Score.Length != 0)
            {
                p1Score.SetDigit(0, digitSprites[GameManager.Instance.score % 10]);
                p1Score.SetDigit(1, digitSprites[(GameManager.Instance.score / 10) % 10]);
                p1Score.SetDigit(2, GameManager.Instance.score >= 100 ? digitSprites[(GameManager.Instance.score / 100) % 10] : null);
                p1Score.SetDigit(3, GameManager.Instance.score >= 1000 ? digitSprites[(GameManager.Instance.score / 1000) % 10] : null);
                p1Score.SetDigit(4, GameManager.Instance.score >= 10000 ? digitSprites[(GameManager.Instance.score / 10000) % 10] : null);
                p1Score.SetDigit(5, GameManager.Instance.score >= 100000 ? digitSprites[(GameManager.Instance.score / 100000) % 10] : null);
            }
            //2 player isn't implemented so we are just going to use the same score 
            if (highScore.Length != 0)
            {
                highScore.SetDigit(0, digitSprites[GameManager.Instance.score % 10]);
                highScore.SetDigit(1, digitSprites[(GameManager.Instance.score / 10) % 10]);
                highScore.SetDigit(2, GameManager.Instance.score >= 100 ? digitSprites[(GameManager.Instance.score / 100) % 10] : null);
                highScore.SetDigit(3, GameManager.Instance.score >= 1000 ? digitSprites[(GameManager.Instance.score / 1000) % 10] : null);
                highScore.SetDigit(4, GameManager.Instance.score >= 10000 ? digitSprites[(GameManager.Instance.score / 10000) % 10] : null);
                highScore.SetDigit(5, GameManager.Instance.score >= 100000 ? digitSprites[(GameManager.Instance.score / 100000) % 10] : null);
            }
        }
}

[Serializable]
public class DigitDisplay
{
    [SerializeField] public SpriteRenderer[] digits;

    public void SetDigit(int index, Sprite sprite)
    {
        digits[index].sprite = sprite;
    }

    public int Length => digits.Length;
}
