using UnityEngine;
using DG.Tweening;

public class EditProfileTween : MonoBehaviour
{
    [Header("Tween Settings")]
    [SerializeField] private float animDuration = 0.35f;
    [SerializeField] private Ease openEase = Ease.OutCubic;
    [SerializeField] private Ease closeEase = Ease.InCubic;
    [SerializeField] private float offscreenOffset = 200f;

    private RectTransform rect;
    private Vector2 openPos;
    private Vector2 closedPos;
    private bool isInitialized = false;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        openPos = rect.anchoredPosition;
        //  closedPos = openPos + new Vector2(0f, rect.rect.height + offscreenOffset);
        closedPos = openPos - new Vector2(0f, Screen.height);


        rect.anchoredPosition = closedPos;
        gameObject.SetActive(false);

        isInitialized = true;
    }

    public void Open()
    {
        if (!isInitialized) return;

        gameObject.SetActive(true);
        rect.DOKill();

        rect.DOAnchorPos(openPos, animDuration)
            .SetEase(openEase);
    }

    public void Close()
    {
        if (!isInitialized) return;

        rect.DOKill();

        rect.DOAnchorPos(closedPos, animDuration)
            .SetEase(closeEase)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
    }
}
