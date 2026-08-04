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
    
    public void PlayGhostMove(int type = 1)
    {
        switch (type)
        {
            case 1:
                PlayLoop1(ghostMove1);
                break;
            case 2:
                PlayLoop1(ghostMove2);
                break;
            case 3:
                PlayLoop1(ghostMove3);
                break;
            case 4:
                PlayLoop1(ghostMove4);
                break;
            case 5:
                PlayLoop1(ghostMove5);
                break;
        }
    }
}