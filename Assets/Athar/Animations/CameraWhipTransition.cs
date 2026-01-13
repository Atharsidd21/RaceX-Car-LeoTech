using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CameraWhipTransition : MonoBehaviour
{
    public static CameraWhipTransition Instance;

    [Header("Camera")]
    [SerializeField] private Camera whipCamera;

    [Header("Whip Settings")]
    [SerializeField] private float whipDistance = 25f;
    [SerializeField] private float whipDuration = 0.35f;
    [SerializeField]
    private AnimationCurve whipCurve =
        AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Fade (Optional)")]
    [SerializeField] private CanvasGroup fadeGroup;
    [SerializeField] private float fadeAlpha = 0.4f;

    private Vector3 startPos;
    private bool isTransitioning;

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

        whipCamera.enabled = false;
        startPos = whipCamera.transform.position;
    }

    /// <summary>
    /// Camera whip transition using build index
    /// </summary>
    public void LoadSceneWhip(int buildIndex)
    {
        if (!isTransitioning)
            StartCoroutine(WhipRoutine(buildIndex));
    }

    private IEnumerator WhipRoutine(int buildIndex)
    {
        isTransitioning = true;

        whipCamera.enabled = true;

        if (fadeGroup != null)
            fadeGroup.alpha = 0f;

        Vector3 endPos = startPos + Vector3.right * whipDistance;

        float t = 0f;
        while (t < whipDuration)
        {
            t += Time.deltaTime;
            float p = whipCurve.Evaluate(t / whipDuration);

            whipCamera.transform.position =
                Vector3.Lerp(startPos, endPos, p);

            if (fadeGroup != null)
                fadeGroup.alpha = Mathf.Lerp(0f, fadeAlpha, p);

            yield return null;
        }

        // ?? CUT SCENE AT PEAK MOTION
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(buildIndex);
        while (!loadOp.isDone)
            yield return null;

        yield return null; // buffer frame

        // RESET CAMERA
        whipCamera.transform.position = startPos;
        whipCamera.enabled = false;

        if (fadeGroup != null)
            fadeGroup.alpha = 0f;

        isTransitioning = false;
    }
}
