using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource loopSlot1;
    [SerializeField] private AudioSource loopSlot2;
    [SerializeField] private AudioSource oneShotSource;
    
    [Header("Sound Effects")]
    [SerializeField] private AudioClip startMusic;
    [SerializeField] private AudioClip eatingTheDots;
    [SerializeField] private AudioClip turnCorner;
    [SerializeField] private AudioClip extraLife;
    [SerializeField] private AudioClip ghostMove1;
    [SerializeField] private AudioClip ghostMove2;
    [SerializeField] private AudioClip ghostMove3;
    [SerializeField] private AudioClip ghostMove4;
    [SerializeField] private AudioClip ghostMove5;
    [SerializeField] private AudioClip fruit;
    [SerializeField] private AudioClip ghostRun;
    [SerializeField] private AudioClip ghostEaten;
    [SerializeField] private AudioClip ghostReturn;
    [SerializeField] private AudioClip pacmanDeath;
    
    //turn corner(): if eating dots is true, play turn corner sound effect, else play turn corner sound effect

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        GameManager.OnScoreChanged += UpdateGhostMoveSiren;
    }
    
    public void PlayLoop1(AudioClip clip)
    {
        loopSlot1.clip = clip;
        loopSlot1.Play();
    }

    public void StopLoop1()
    {
        loopSlot1.Stop();
    }
    
    public void PlayLoop2(AudioClip clip)
    {
        loopSlot2.clip = clip;
        loopSlot2.Play();
    }

    public void StopLoop2()
    {
        loopSlot2.Stop();
    }
    
    public void PlayOneShot(AudioClip clip)
    {
        oneShotSource.PlayOneShot(clip);
    }
    
    public void StopAllAudio()
    {
        loopSlot1.Stop();
        loopSlot2.Stop();
        oneShotSource.Stop();
    }
    
    public void PlayStartMusic()
    {
        PlayLoop1(startMusic);
    }
    
    public void PlayGhostMove()
    {
        PlayLoop1(ghostMove1);
        UpdateGhostMoveSiren();
    }
    
    private bool IsGhostMovePlaying()
    {
        return loopSlot1.clip == ghostMove1 ||
               loopSlot1.clip == ghostMove2 ||
               loopSlot1.clip == ghostMove3 ||
               loopSlot1.clip == ghostMove4 ||
               loopSlot1.clip == ghostMove5;
    }

    private void UpdateGhostMoveSiren()
    {
        if (!IsGhostMovePlaying())
            return;

        int type;
        int pelletsRemaining = GameManager.Instance.pelletsRemaining;

        if (pelletsRemaining > 195)
            type = 1;
        else if (pelletsRemaining > 146)
            type = 2;
        else if (pelletsRemaining > 97)
            type = 3;
        else if (pelletsRemaining > 48)
            type = 4;
        else
            type = 5;

        AudioClip target = type switch
        {
            1 => ghostMove1,
            2 => ghostMove2,
            3 => ghostMove3,
            4 => ghostMove4,
            _ => ghostMove5
        };

        if (loopSlot1.clip == target)
            return;

        PlayLoop1(target);
    }

    public void PlayFrightened()
    {
        PlayLoop1(ghostRun);
    }

    public void PlayGhostEaten()
    {
        loopSlot2.PlayOneShot(ghostEaten);
    }
    
    public void PlayRunHome()
    {
        PlayLoop1(ghostReturn);
    }

    public void PlayDeathSound()
    {
        loopSlot1.PlayOneShot(pacmanDeath);
    }
}