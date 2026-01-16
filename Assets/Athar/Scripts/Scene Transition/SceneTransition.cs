using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance;

    [Header("Overlay")]
    [SerializeField] private RectTransform slideOverlay;

    [Header("Settings")]
    [SerializeField] private float slideDuration = 0.35f;

    [Tooltip("Time")]
    [SerializeField] private float holdTime = 0.5f;

    private bool isTransitioning = false;
    private float screenWidth;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        screenWidth = Screen.width;
        slideOverlay.gameObject.SetActive(false);
    }

    /// <summary>
    /// slideLeft = true  ? forward / next
    /// slideLeft = false ? back
    /// </summary>
    public void LoadSceneSlide(int buildIndex, bool slideLeft)
    {
        if (!isTransitioning)
            StartCoroutine(SlideRoutine(buildIndex, slideLeft));
    }

    private IEnumerator SlideRoutine(int buildIndex, bool slideLeft)
    {
        isTransitioning = true;

        slideOverlay.gameObject.SetActive(true);

        float dir = slideLeft ? 1f : -1f;
        Vector2 startPos = new Vector2(-screenWidth * dir, 0);
        Vector2 endPos = Vector2.zero;
        Vector2 exitPos = new Vector2(-screenWidth * dir,0);

        // Start offscreen
        slideOverlay.anchoredPosition = startPos;

        // Slide IN
        float t = 0f;
        while (t < slideDuration)
        {
            t += Time.deltaTime;
            slideOverlay.anchoredPosition =
                Vector2.Lerp(startPos, endPos, t / slideDuration);
            yield return null;
        }

        slideOverlay.anchoredPosition = endPos;
        // ?? HOLD FULL SCREEN
        if (holdTime > 0f)
            yield return new WaitForSecondsRealtime(holdTime);
        // Load scene BEHIND overlay
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(buildIndex);
        while (!loadOp.isDone)
            yield return null;

        yield return null; // buffer frame

        // Slide OUT
        t = 0f;
        while (t < slideDuration)
        {
            t += Time.deltaTime;
            slideOverlay.anchoredPosition =
                Vector2.Lerp(endPos, exitPos, t / slideDuration);
            yield return null;
        }

        slideOverlay.anchoredPosition = exitPos;
        slideOverlay.gameObject.SetActive(false);

        isTransitioning = false;

    }
}