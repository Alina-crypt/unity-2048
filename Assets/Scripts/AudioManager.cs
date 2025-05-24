using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("AudioSource")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip background;
    public AudioClip GameOver;
    public AudioClip Merge;
    public AudioClip Move;
    public AudioClip Click;


    private void Awake()
    {
       
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (!musicSource.isPlaying)
        {
            musicSource.clip = background;
            musicSource.loop = true;
            musicSource.Play();
        }
    }
    public void PlaySFX(AudioClip clip)
    {
       sfxSource.PlayOneShot(clip);
    }
    public void PlayMove()
    {
        PlaySFX(Move);
    }
    public void PlayMerge()
    {
        PlaySFX(Merge);
    }

    public void PlayClick()
    {
        PlaySFX(Click);
    }

    public void PlayGameOver()
    {
        PlaySFX(GameOver);
    }


}




