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
        GameManager.OnScoreChanged += UpdateScore;
    }

    private void OnCreditChanged()
    {
        _creditCoroutine ??= StartCoroutine(UpdateDisplayedCredits());
    }
    
    //Courutine to update the displayed credits smoothly
    private IEnumerator UpdateDisplayedCredits()
    {
        while (_displayedCredits < GameManager.Instance.credits)
        {
            SoundManager.Instance.PlayOneShot(creditSound);
            yield return new WaitForSeconds(0.2f);
            _displayedCredits++;
            UpdateUICredits();
            yield return new WaitForSeconds(0.3f);
        }
        _creditCoroutine = null;
    }
    
    private void UpdateUICredits()
    {
        credits.SetDigit(0, digitSprites[_displayedCredits % 10]); // Ones place
        credits.SetDigit(1, digitSprites[(_displayedCredits / 10) % 10]); // Tens place
    }
    //int digit1 = (number / 100000) % 10;
    //int digit2 = (number / 10000) % 10;
    //int digit3 = (number / 1000) % 10;
    //int digit4 = (number / 100) % 10;
    //int digit5 = (number / 10) % 10;
    //int digit6 = number % 10;

    private void UpdateScore()
    {
        p1Score.SetDigit(0, digitSprites[GameManager.Instance.score % 10]);
        //p1Score.SetDigit(1, digitSprites[GameManager.Instance.score % 10]);
        
        p2Score.SetDigit(0, digitSprites[GameManager.Instance.p2Score % 10]);
        
        highScore.SetDigit(0, digitSprites[GameManager.Instance.highScore % 10]);
        

        
    }
}

[Serializable]
public class DigitDisplay
{
    [SerializeField] public SpriteRenderer[] digits;

    public void SetDigit(int index, Sprite sprite)
    {
        //digits[index].sprite = sprite;
    }

    public int Length => digits.Length;
}
