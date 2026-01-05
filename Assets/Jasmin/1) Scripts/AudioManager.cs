using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// AudioManager
/// ------------------------------------------------------------
/// Applies audio state (Mixer volumes + Play/Stop) ONLY.
/// Does NOT store/read PlayerPrefs. That is handled by GameDataPrefs.
/// 
/// Sync rules:
/// - OnEnable: subscribes to GameDataPrefs events and pulls current values.
/// - When UI toggles call AudioManager.SetMusicEnabled / SetSfxEnabled:
///     AudioManager forwards save to GameDataPrefs, then auto-updates via events.
/// </summary>
public class AudioManager : MonoBehaviour
{
    #region Singleton
    public static AudioManager Instance { get; private set; }

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

        BuildLookups();
    }
    #endregion

    #region Enums / Entry
    public enum AudioBus { Music, Sfx }

    public enum AudioId
    {
        Background,
        ButtonClick,
        OceanEntityDie,
        GarbageCollect,
        GameOver
    }

    [Serializable]
    public class AudioEntry
    {
        public AudioBus bus;
        public AudioId id;
        public AudioSource source;
        public AudioClip clip;
        public bool loop;
    }
    #endregion

    #region Inspector
    [Header("AudioMixer")]
    [SerializeField] private AudioMixer mixer;

    [Tooltip("Exposed parameter name in AudioMixer for Music volume (dB). Example: MusicVol")]
    [SerializeField] private string musicVolumeParam = "MusicVol";

    [Tooltip("Exposed parameter name in AudioMixer for SFX volume (dB). Example: SfxVol")]
    [SerializeField] private string sfxVolumeParam = "SfxVol";

    [Header("Mute Behaviour")]
    [SerializeField] private bool pauseMusicOnMute = true;
    [SerializeField] private bool stopSfxOnMute = true;

    [Header("Audio Entries")]
    [SerializeField] private List<AudioEntry> entries = new();
    #endregion

    #region Runtime State
    private readonly Dictionary<AudioId, AudioEntry> _map = new();

    private bool _musicEnabled = true;
    private bool _sfxEnabled = true;

    // Optional (kept for future, even if you only use toggle now)
    private float _musicVol01 = 1f;
    private float _sfxVol01 = 1f;
    #endregion

    #region Unity Lifecycle
    private void OnEnable()
    {
        if (GameDataPrefs.Instance != null)
        {
            GameDataPrefs.Instance.OnMusicEnabledChanged += HandleMusicEnabledChanged;
            GameDataPrefs.Instance.OnSfxEnabledChanged += HandleSfxEnabledChanged;

            GameDataPrefs.Instance.OnMusicVolumeChanged01 += HandleMusicVolumeChanged01;
            GameDataPrefs.Instance.OnSfxVolumeChanged01 += HandleSfxVolumeChanged01;

            SyncFromPrefs();
        }
        else
        {
            // If GameDataPrefs loads later, call AudioManager.Instance.SyncFromPrefs() after it spawns.
            ApplyMixerStates();
        }
    }

    private void OnDisable()
    {
        if (GameDataPrefs.Instance == null) return;

        GameDataPrefs.Instance.OnMusicEnabledChanged -= HandleMusicEnabledChanged;
        GameDataPrefs.Instance.OnSfxEnabledChanged -= HandleSfxEnabledChanged;

        GameDataPrefs.Instance.OnMusicVolumeChanged01 -= HandleMusicVolumeChanged01;
        GameDataPrefs.Instance.OnSfxVolumeChanged01 -= HandleSfxVolumeChanged01;
    }
    #endregion

    #region Sync
    /// <summary>
    /// Purpose: Pull current audio settings from GameDataPrefs and apply them.
    /// Call this if GameDataPrefs is created after AudioManager.
    /// </summary>
    public void SyncFromPrefs()
    {
        if (GameDataPrefs.Instance == null) return;

        _musicEnabled = GameDataPrefs.Instance.GetMusicEnabled();
        _sfxEnabled = GameDataPrefs.Instance.GetSfxEnabled();

        _musicVol01 = GameDataPrefs.Instance.GetMusicVolume01();
        _sfxVol01 = GameDataPrefs.Instance.GetSfxVolume01();

        ApplyMixerStates();
    }

    private void HandleMusicEnabledChanged(bool enabled)
    {
        _musicEnabled = enabled;
        ApplyMixerStates();
    }

    private void HandleSfxEnabledChanged(bool enabled)
    {
        _sfxEnabled = enabled;
        ApplyMixerStates();
    }

    private void HandleMusicVolumeChanged01(float v01)
    {
        _musicVol01 = Mathf.Clamp01(v01);
        ApplyMixerStates();
    }

    private void HandleSfxVolumeChanged01(float v01)
    {
        _sfxVol01 = Mathf.Clamp01(v01);
        ApplyMixerStates();
    }
    #endregion

    #region Public API - Toggle UI (saves via GameDataPrefs)
    /// <summary>
    /// Purpose: Enable/Disable music and save it via GameDataPrefs (no PlayerPrefs here).
    /// </summary>
    public void SetMusicEnabled(bool enabled)
    {
        if (GameDataPrefs.Instance != null)
            GameDataPrefs.Instance.SetMusicEnabled(enabled);
        else
        {
            _musicEnabled = enabled;
            ApplyMixerStates();
        }
    }

    /// <summary>
    /// Purpose: Enable/Disable SFX and save it via GameDataPrefs (no PlayerPrefs here).
    /// </summary>
    public void SetSfxEnabled(bool enabled)
    {
        if (GameDataPrefs.Instance != null)
            GameDataPrefs.Instance.SetSfxEnabled(enabled);
        else
        {
            _sfxEnabled = enabled;
            ApplyMixerStates();
        }
    }

    public void ToggleMusic() => SetMusicEnabled(!_musicEnabled);
    public void ToggleSfx() => SetSfxEnabled(!_sfxEnabled);

    public bool IsMusicEnabled() => _musicEnabled;
    public bool IsSfxEnabled() => _sfxEnabled;
    #endregion

    #region Public API - Play/Stop
    /// <summary>
    /// Purpose: Play a sound by AudioId. Respects Music/SFX enabled toggles.
    /// </summary>
    public void Play(AudioId id, float volume = 1f, bool oneShot = true)
    {
        if (!_map.TryGetValue(id, out var e)) return;
        if (e.source == null) return;

        var clip = e.clip != null ? e.clip : e.source.clip;
        if (clip == null) return;

        e.source.loop = e.loop;

        if (e.bus == AudioBus.Music && !_musicEnabled) return;
        if (e.bus == AudioBus.Sfx && !_sfxEnabled) return;

        if (oneShot && !e.loop)
            e.source.PlayOneShot(clip, volume);
        else
        {
            e.source.clip = clip;
            e.source.volume = volume;
            e.source.Play();
        }
    }

    /// <summary>Purpose: Stop a specific audio source.</summary>
    public void Stop(AudioId id)
    {
        if (_map.TryGetValue(id, out var e) && e.source != null)
            e.source.Stop();
    }

    /// <summary>Purpose: Stop all sounds of a bus type (Music or Sfx).</summary>
    public void StopAll(AudioBus bus)
    {
        foreach (var kv in _map)
        {
            var e = kv.Value;
            if (e?.source == null) continue;
            if (e.bus != bus) continue;
            e.source.Stop();
        }
    }

    /// <summary>Purpose: Pause all sounds of a bus type.</summary>
    public void PauseAll(AudioBus bus)
    {
        foreach (var kv in _map)
        {
            var e = kv.Value;
            if (e?.source == null) continue;
            if (e.bus != bus) continue;
            if (e.source.isPlaying) e.source.Pause();
        }
    }

    /// <summary>Purpose: Unpause all sounds of a bus type.</summary>
    public void UnpauseAll(AudioBus bus)
    {
        foreach (var kv in _map)
        {
            var e = kv.Value;
            if (e?.source == null) continue;
            if (e.bus != bus) continue;
            e.source.UnPause();
        }
    }
    #endregion

    #region Internal Apply / Mixer
    private void BuildLookups()
    {
        _map.Clear();
        foreach (var e in entries)
        {
            if (e == null) continue;
            if (!_map.ContainsKey(e.id))
                _map.Add(e.id, e);
        }
    }

    private void ApplyMixerStates()
    {
        float music01 = _musicEnabled ? _musicVol01 : 0f;
        float sfx01 = _sfxEnabled ? _sfxVol01 : 0f;

        ApplyMixerVolume(musicVolumeParam, music01);
        ApplyMixerVolume(sfxVolumeParam, sfx01);

        if (!_musicEnabled && pauseMusicOnMute) PauseAll(AudioBus.Music);
        if (_musicEnabled) UnpauseAll(AudioBus.Music);

        if (!_sfxEnabled && stopSfxOnMute) StopAll(AudioBus.Sfx);
    }

    private void ApplyMixerVolume(string param, float v01)
    {
        if (mixer == null) return;
        mixer.SetFloat(param, ToDecibels(v01));
    }

    // 0..1 => dB (0 => -80dB, 1 => 0dB)
    private float ToDecibels(float v01)
    {
        v01 = Mathf.Clamp01(v01);
        if (v01 <= 0.0001f) return -80f;
        return Mathf.Log10(v01) * 20f;
    }
    #endregion
}
