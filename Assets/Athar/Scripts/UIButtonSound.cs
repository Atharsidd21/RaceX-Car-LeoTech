using UnityEngine;

public class UIButtonSound : MonoBehaviour
{
    [Tooltip("Play sound on button click")]
    public bool playOnClick = true;

    public void PlayClickSound()
    {
        if (!playOnClick) return;

        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.PlayButtonClick();
        }
    }
}
