using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance;

    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 0.4f;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public IEnumerator FadeIn()
    {
        yield return Fade(0.4f, 0f);
    }

    public IEnumerator FadeOut()
    {
        yield return Fade(0f, 0.4f);
    }

     IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float time = 0f;
        Color color = fadeImage.color;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, time / fadeDuration);
            color.a = alpha;
            fadeImage.color = color;
            yield return null;
        }

        color.a = endAlpha;
        fadeImage.color = color;
    }
}
