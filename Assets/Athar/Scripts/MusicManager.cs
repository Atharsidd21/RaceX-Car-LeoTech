using UnityEngine;
using DG.Tweening;


public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;
    private AudioSource audioSource;

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

    public void PlayMusic()
    {
        if (!audioSource.isPlaying)
            audioSource.Play();
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

}
