using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelBGMusic : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource bgSource;
    [SerializeField] private AudioClip bgMusic;

    [Header("UI")]
    [SerializeField] private GameObject volumePanel;
    [SerializeField] private Slider volumeSlider;

    [Header("Volume Button UI")]
    [SerializeField] private Image volumeButtonImage;
    [SerializeField] private Sprite volumeFullSprite; // 60 - 100
    [SerializeField] private Sprite volumeLowSprite;  // 1 - 59
    [SerializeField] private Sprite volumeMuteSprite; // 0

    [Header("Volume Text")]
    [SerializeField] private TextMeshProUGUI volumeText;

    private const string VolumeKey = "LevelBGVolume";

    private void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat(VolumeKey, 0.7f);

        // Setup AudioSource
        bgSource.clip = bgMusic;
        bgSource.loop = true;
        bgSource.playOnAwake = false;

        bgSource.volume = savedVolume;
        bgSource.Play();

        // Setup UI
        volumeSlider.value = savedVolume;
        volumePanel.SetActive(false);

        UpdateVolumeUI(savedVolume);

        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
    }

    private void OnVolumeChanged(float value)
    {
        bgSource.volume = value;
        PlayerPrefs.SetFloat(VolumeKey, value);
        PlayerPrefs.Save();

        UpdateVolumeUI(value);
    }

    private void UpdateVolumeUI(float value)
    {
        int volumePercent = Mathf.RoundToInt(value * 100f);
        volumeText.text = volumePercent.ToString();

        if (volumePercent == 0)
        {
            volumeButtonImage.sprite = volumeMuteSprite;
        }
        else if (volumePercent <= 60)
        {
            volumeButtonImage.sprite = volumeLowSprite;
        }
        else
        {
            volumeButtonImage.sprite = volumeFullSprite;
        }
    }

    public void ToggleVolumePanel()
    {
        volumePanel.SetActive(!volumePanel.activeSelf);
    }

    private void OnDestroy()
    {
        volumeSlider.onValueChanged.RemoveListener(OnVolumeChanged);
    }
}