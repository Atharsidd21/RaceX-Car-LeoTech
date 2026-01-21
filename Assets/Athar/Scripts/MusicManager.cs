using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;


public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;
    private AudioSource audioSource;
    [Header("UI Sounds")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip buttonClickClip;
    [SerializeField] private AudioClip BuySFX;


    private void Awake()
    {


        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyMusicState();
    }
    /* public void PlayMusic()
     {
         if (!audioSource.isPlaying)
             audioSource.Play();
     }*/
    public void PlayMusic()
    {
        ApplyMusicState();
    }


    public void StopMusic()
    {
        audioSource.Stop();
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }
    public void FadeOutAndStop(float duration = 0.8f)
    {
        audioSource.DOFade(0f, duration).OnComplete(() =>
        {
            audioSource.Stop();
            audioSource.volume = 1f; // reset for next play
        });
    }

    public void FadeInAndPlay(float duration = 0.8f)
    {
        audioSource.volume = 0f;
        audioSource.Play();
        audioSource.DOFade(1f, duration);



    }
    /*  public void ApplyMusicState()
      {
          bool musicOn = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;

          if (musicOn)
              PlayMusic();
          else
              StopMusic();
      }*/
    public void ApplyMusicState()
    {
        bool musicOn = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;

        if (musicOn)
        {
            if (!audioSource.isPlaying)
                audioSource.Play();
        }
        else
        {
            audioSource.Stop();
        }
    }


    //For Buttons 
    public void PlayButtonClick()
    {
        if (buttonClickClip == null || sfxSource == null)
            return;

        sfxSource.PlayOneShot(buttonClickClip);
    }
    public void PlayBuySound()
    {
        if (BuySFX != null)
            audioSource.PlayOneShot(BuySFX);
    }



}