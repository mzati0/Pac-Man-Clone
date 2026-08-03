using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource loopSlot1;
    [SerializeField] private AudioSource loopSlot2;
    [SerializeField] private AudioSource oneShotSource;

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
}