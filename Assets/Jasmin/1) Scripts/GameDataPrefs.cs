using System;
using UnityEngine;

/// <summary>
/// GameDataPrefs
/// ------------------------------------------------------------
/// Central PlayerPrefs data store for your game (ONLY place that touches PlayerPrefs).
///
/// Stores:
///  - PlayerName
///  - Coins
///  - LastSelectedCarId
///  - SelectedAvatarId
///  - MusicEnabled / SfxEnabled (toggle)
///  - (Optional) MusicVolume01 / SfxVolume01 (kept for future, even if no sliders)
///
/// Features:
///  - Singleton + DontDestroyOnLoad
///  - EnsureDefaults() on first run
///  - Events so other systems (AudioManager, UI) can auto-sync
/// </summary>
public class GameDataPrefs : MonoBehaviour
{
    #region Singleton
    public static GameDataPrefs Instance { get; private set; }

    [Header("Singleton")]
    [SerializeField] private bool dontDestroyOnLoad = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        if (dontDestroyOnLoad) DontDestroyOnLoad(gameObject);

        EnsureDefaults();
    }
    #endregion

    #region Events (Sync hooks)
    public event Action<bool> OnMusicEnabledChanged;
    public event Action<bool> OnSfxEnabledChanged;

    // Optional future use (volume)
    public event Action<float> OnMusicVolumeChanged01;
    public event Action<float> OnSfxVolumeChanged01;
    #endregion

    #region PlayerPrefs Keys
    private const string KEY_HAS_SAVE = "GD_HAS_SAVE";

    private const string KEY_PLAYER_NAME = "GD_PLAYER_NAME";
    private const string KEY_COINS = "GD_COINS";
    private const string KEY_LAST_CAR_ID = "GD_LAST_CAR_ID";
    private const string KEY_AVATAR_ID = "GD_AVATAR_ID";

    private const string KEY_MUSIC_ENABLED = "GD_MUSIC_ENABLED";
    private const string KEY_SFX_ENABLED = "GD_SFX_ENABLED";

    // Optional
    private const string KEY_MUSIC_VOL01 = "GD_MUSIC_VOL01";
    private const string KEY_SFX_VOL01 = "GD_SFX_VOL01";
    #endregion

    #region Defaults
    [Header("Defaults")]
    [SerializeField] private string defaultPlayerName = "Player";
    [SerializeField] private int defaultCoins = 0;
    [SerializeField] private int defaultCarId = 0;
    [SerializeField] private int defaultAvatarId = 0;
    [SerializeField] private bool defaultMusicEnabled = true;
    [SerializeField] private bool defaultSfxEnabled = true;

    [Header("Optional Defaults (0..1)")]
    [Range(0f, 1f)][SerializeField] private float defaultMusicVolume01 = 1f;
    [Range(0f, 1f)][SerializeField] private float defaultSfxVolume01 = 1f;
    #endregion

    #region General
    /// <summary>Purpose: returns true if defaults/save were initialized before.</summary>
    public bool HasSave() => PlayerPrefs.GetInt(KEY_HAS_SAVE, 0) == 1;

    /// <summary>
    /// Purpose: Initialize defaults on first run so game always has valid values.
    /// </summary>
    public void EnsureDefaults()
    {
        if (HasSave()) return;

        PlayerPrefs.SetString(KEY_PLAYER_NAME, defaultPlayerName);
        PlayerPrefs.SetInt(KEY_COINS, Mathf.Max(0, defaultCoins));
        PlayerPrefs.SetInt(KEY_LAST_CAR_ID, defaultCarId);
        PlayerPrefs.SetInt(KEY_AVATAR_ID, defaultAvatarId);

        PlayerPrefs.SetInt(KEY_MUSIC_ENABLED, defaultMusicEnabled ? 1 : 0);
        PlayerPrefs.SetInt(KEY_SFX_ENABLED, defaultSfxEnabled ? 1 : 0);

        PlayerPrefs.SetFloat(KEY_MUSIC_VOL01, Mathf.Clamp01(defaultMusicVolume01));
        PlayerPrefs.SetFloat(KEY_SFX_VOL01, Mathf.Clamp01(defaultSfxVolume01));

        PlayerPrefs.SetInt(KEY_HAS_SAVE, 1);
        PlayerPrefs.Save();
    }

    /// <summary>Purpose: Force PlayerPrefs.Save() now (safe on mobile).</summary>
    public void SaveNow() => PlayerPrefs.Save();

    /// <summary>
    /// Purpose: Deletes all saved data and re-applies defaults (debug/reset button).
    /// </summary>
    public void ResetAll()
    {
        PlayerPrefs.DeleteKey(KEY_HAS_SAVE);

        PlayerPrefs.DeleteKey(KEY_PLAYER_NAME);
        PlayerPrefs.DeleteKey(KEY_COINS);
        PlayerPrefs.DeleteKey(KEY_LAST_CAR_ID);
        PlayerPrefs.DeleteKey(KEY_AVATAR_ID);

        PlayerPrefs.DeleteKey(KEY_MUSIC_ENABLED);
        PlayerPrefs.DeleteKey(KEY_SFX_ENABLED);

        PlayerPrefs.DeleteKey(KEY_MUSIC_VOL01);
        PlayerPrefs.DeleteKey(KEY_SFX_VOL01);

        PlayerPrefs.Save();

        EnsureDefaults();

        // Fire events so listeners refresh
        OnMusicEnabledChanged?.Invoke(GetMusicEnabled());
        OnSfxEnabledChanged?.Invoke(GetSfxEnabled());
        OnMusicVolumeChanged01?.Invoke(GetMusicVolume01());
        OnSfxVolumeChanged01?.Invoke(GetSfxVolume01());
    }
    #endregion

    #region Player Profile
    /// <summary>Purpose: Get stored player name.</summary>
    public string GetPlayerName() => PlayerPrefs.GetString(KEY_PLAYER_NAME, defaultPlayerName);

    /// <summary>Purpose: Save player name.</summary>
    public void SetPlayerName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) name = defaultPlayerName;

        PlayerPrefs.SetString(KEY_PLAYER_NAME, name);
        PlayerPrefs.SetInt(KEY_HAS_SAVE, 1);
        PlayerPrefs.Save();
    }

    /// <summary>Purpose: Get selected avatar id/index.</summary>
    public int GetSelectedAvatarId() => PlayerPrefs.GetInt(KEY_AVATAR_ID, defaultAvatarId);

    /// <summary>Purpose: Save selected avatar id/index.</summary>
    public void SetSelectedAvatarId(int avatarId)
    {
        PlayerPrefs.SetInt(KEY_AVATAR_ID, avatarId);
        PlayerPrefs.SetInt(KEY_HAS_SAVE, 1);
        PlayerPrefs.Save();
    }
    #endregion

    #region Currency / Coins
    /// <summary>Purpose: Get coins value.</summary>
    public int GetCoins() => PlayerPrefs.GetInt(KEY_COINS, defaultCoins);

    /// <summary>Purpose: Set coins to exact value (>=0).</summary>
    public void SetCoins(int amount)
    {
        amount = Mathf.Max(0, amount);
        PlayerPrefs.SetInt(KEY_COINS, amount);
        PlayerPrefs.SetInt(KEY_HAS_SAVE, 1);
        PlayerPrefs.Save();
    }

    /// <summary>Purpose: Add/subtract coins safely (clamps to >=0). Returns new value.</summary>
    public int AddCoins(int delta)
    {
        int newValue = Mathf.Max(0, GetCoins() + delta);
        SetCoins(newValue);
        return newValue;
    }

    /// <summary>Purpose: True if player has enough coins.</summary>
    public bool HasEnoughCoins(int cost) => GetCoins() >= Mathf.Max(0, cost);

    /// <summary>Purpose: Spend coins if possible. Returns true if successful.</summary>
    public bool SpendCoins(int cost)
    {
        cost = Mathf.Max(0, cost);
        if (!HasEnoughCoins(cost)) return false;

        SetCoins(GetCoins() - cost);
        return true;
    }
    #endregion

    #region Car Selection
    /// <summary>Purpose: Get last selected car id/index.</summary>
    public int GetLastSelectedCarId() => PlayerPrefs.GetInt(KEY_LAST_CAR_ID, defaultCarId);

    /// <summary>Purpose: Save last selected car id/index.</summary>
    public void SetLastSelectedCarId(int carId)
    {
        PlayerPrefs.SetInt(KEY_LAST_CAR_ID, carId);
        PlayerPrefs.SetInt(KEY_HAS_SAVE, 1);
        PlayerPrefs.Save();
    }
    #endregion

    #region Audio (Toggle-based + optional volume)
    /// <summary>Purpose: Get music enabled (toggle).</summary>
    public bool GetMusicEnabled() => PlayerPrefs.GetInt(KEY_MUSIC_ENABLED, defaultMusicEnabled ? 1 : 0) == 1;

    /// <summary>Purpose: Save music enabled (toggle) + notify listeners.</summary>
    public void SetMusicEnabled(bool enabled)
    {
        PlayerPrefs.SetInt(KEY_MUSIC_ENABLED, enabled ? 1 : 0);
        PlayerPrefs.SetInt(KEY_HAS_SAVE, 1);
        PlayerPrefs.Save();
        OnMusicEnabledChanged?.Invoke(enabled);
    }

    /// <summary>Purpose: Get sfx enabled (toggle).</summary>
    public bool GetSfxEnabled() => PlayerPrefs.GetInt(KEY_SFX_ENABLED, defaultSfxEnabled ? 1 : 0) == 1;

    /// <summary>Purpose: Save sfx enabled (toggle) + notify listeners.</summary>
    public void SetSfxEnabled(bool enabled)
    {
        PlayerPrefs.SetInt(KEY_SFX_ENABLED, enabled ? 1 : 0);
        PlayerPrefs.SetInt(KEY_HAS_SAVE, 1);
        PlayerPrefs.Save();
        OnSfxEnabledChanged?.Invoke(enabled);
    }

    /// <summary>Purpose: Get music volume 0..1 (optional future use).</summary>
    public float GetMusicVolume01() => Mathf.Clamp01(PlayerPrefs.GetFloat(KEY_MUSIC_VOL01, defaultMusicVolume01));

    /// <summary>Purpose: Save music volume 0..1 (optional future use) + notify listeners.</summary>
    public void SetMusicVolume01(float v01)
    {
        v01 = Mathf.Clamp01(v01);
        PlayerPrefs.SetFloat(KEY_MUSIC_VOL01, v01);
        PlayerPrefs.SetInt(KEY_HAS_SAVE, 1);
        PlayerPrefs.Save();
        OnMusicVolumeChanged01?.Invoke(v01);
    }

    /// <summary>Purpose: Get sfx volume 0..1 (optional future use).</summary>
    public float GetSfxVolume01() => Mathf.Clamp01(PlayerPrefs.GetFloat(KEY_SFX_VOL01, defaultSfxVolume01));

    /// <summary>Purpose: Save sfx volume 0..1 (optional future use) + notify listeners.</summary>
    public void SetSfxVolume01(float v01)
    {
        v01 = Mathf.Clamp01(v01);
        PlayerPrefs.SetFloat(KEY_SFX_VOL01, v01);
        PlayerPrefs.SetInt(KEY_HAS_SAVE, 1);
        PlayerPrefs.Save();
        OnSfxVolumeChanged01?.Invoke(v01);
    }
    #endregion
}