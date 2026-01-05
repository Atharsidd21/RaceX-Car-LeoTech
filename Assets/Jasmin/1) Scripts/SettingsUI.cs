using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// SettingsUI
/// ------------------------------------------------------------
/// Keeps UI toggles in sync with saved settings (GameDataPrefs)
/// and applies changes via AudioManager (which saves through GameDataPrefs).
///
/// Use:
/// - Assign Music Toggle + SFX Toggle in inspector.
/// - Drop this script on your Settings Panel UI object.
/// </summary>
public class SettingsUI : MonoBehaviour
{
    [Header("Toggles")]
    [SerializeField] private Toggle musicToggle;
    [SerializeField] private Toggle sfxToggle;

    private bool _wired;

    private void OnEnable()
    {
        // Make sure defaults exist (safe even if already initialized)
        if (GameDataPrefs.Instance != null)
            GameDataPrefs.Instance.EnsureDefaults();

        RefreshTogglesFromPrefs();
        WireToggleCallbacksOnce();
        SubscribeToPrefsEvents();
    }

    private void OnDisable()
    {
        UnsubscribeFromPrefsEvents();
    }

    /// <summary>
    /// Purpose: Read saved values from GameDataPrefs and set UI toggle state.
    /// Uses SetIsOnWithoutNotify to avoid triggering callbacks while updating UI.
    /// </summary>
    public void RefreshTogglesFromPrefs()
    {
        if (musicToggle == null || sfxToggle == null) return;
        if (GameDataPrefs.Instance == null) return;

        bool musicOn = GameDataPrefs.Instance.GetMusicEnabled();
        bool sfxOn = GameDataPrefs.Instance.GetSfxEnabled();

        musicToggle.SetIsOnWithoutNotify(musicOn);
        sfxToggle.SetIsOnWithoutNotify(sfxOn);
    }

    /// <summary>
    /// Purpose: Hook up UI toggle -> AudioManager functions (only once).
    /// </summary>
    private void WireToggleCallbacksOnce()
    {
        if (_wired) return;
        _wired = true;

        if (musicToggle != null)
            musicToggle.onValueChanged.AddListener(OnMusicToggleChanged);

        if (sfxToggle != null)
            sfxToggle.onValueChanged.AddListener(OnSfxToggleChanged);
    }

    /// <summary>
    /// Purpose: When user changes Music toggle, apply via AudioManager
    /// (AudioManager will save it through GameDataPrefs).
    /// </summary>
    private void OnMusicToggleChanged(bool enabled)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetMusicEnabled(enabled);
        else if (GameDataPrefs.Instance != null)
            GameDataPrefs.Instance.SetMusicEnabled(enabled);
    }

    /// <summary>
    /// Purpose: When user changes SFX toggle, apply via AudioManager
    /// (AudioManager will save it through GameDataPrefs).
    /// </summary>
    private void OnSfxToggleChanged(bool enabled)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetSfxEnabled(enabled);
        else if (GameDataPrefs.Instance != null)
            GameDataPrefs.Instance.SetSfxEnabled(enabled);
    }

    /// <summary>
    /// Purpose: Subscribe so UI updates if something else changes prefs (another UI, code, etc).
    /// </summary>
    private void SubscribeToPrefsEvents()
    {
        if (GameDataPrefs.Instance == null) return;

        GameDataPrefs.Instance.OnMusicEnabledChanged += HandleMusicEnabledChanged;
        GameDataPrefs.Instance.OnSfxEnabledChanged += HandleSfxEnabledChanged;
    }

    private void UnsubscribeFromPrefsEvents()
    {
        if (GameDataPrefs.Instance == null) return;

        GameDataPrefs.Instance.OnMusicEnabledChanged -= HandleMusicEnabledChanged;
        GameDataPrefs.Instance.OnSfxEnabledChanged -= HandleSfxEnabledChanged;
    }

    private void HandleMusicEnabledChanged(bool enabled)
    {
        if (musicToggle == null) return;
        musicToggle.SetIsOnWithoutNotify(enabled);
    }

    private void HandleSfxEnabledChanged(bool enabled)
    {
        if (sfxToggle == null) return;
        sfxToggle.SetIsOnWithoutNotify(enabled);
    }
}
