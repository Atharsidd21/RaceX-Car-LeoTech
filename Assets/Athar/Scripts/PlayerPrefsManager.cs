using UnityEngine;

public static class PlayerPrefsManager
{
    // ================= PROFILE =================
    public const string USERNAME = "username";
    public const string PLAYER_AVATAR = "PLAYER_AVATAR_INDEX";
    public const string IS_GUEST = "IS_GUEST";
    public const string PROFILE_COMPLETED = "PROFILE_COMPLETED";

    // ================= NAVIGATION =================
    public const string SHOW_LEADERBOARD = "ShowLeaderBoard";
    public const string LEADERBOARD_RANK = "LeaderboardRank";
    public const string GOTO_HOME = "GoingToHome";
    public const string GOTO_LEVEL_SELECTION = "IsGoLevelSelection";
    public const string SELECTED_LEVEL = "SelectedLevel";

    // ================= VEHICLE =================
    public const string SELECTED_CAR = "Pointer";

    // ================= ECONOMY =================
    public const string CURRENCY = "Currency";
    public const string RACES_WON = "RacesWon";
    public const string RACES_LOST = "RacesLost";
    public const string CARS_OWNED = "CarsOwned";

    // ================= AUDIO =================
    public const string MUSIC_ENABLED = "MusicEnabled";
    public const string LEVEL_BG_VOLUME = "LevelBGVolume";

    // ================= GENERIC =================
    public static int GetInt(string key, int defaultValue = 0)
        => PlayerPrefs.GetInt(key, defaultValue);

    public static float GetFloat(string key, float defaultValue = 0f)
        => PlayerPrefs.GetFloat(key, defaultValue);

    public static string GetString(string key, string defaultValue = "")
        => PlayerPrefs.GetString(key, defaultValue);

    public static void SetInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
        PlayerPrefs.Save();
    }

    public static void SetFloat(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
        PlayerPrefs.Save();
    }

    public static void SetString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
        PlayerPrefs.Save();
    }

    // ================= HELPERS =================
    public static bool IsMusicEnabled()
        => GetInt(MUSIC_ENABLED, 1) == 1;

    public static void SetMusicEnabled(bool enabled)
        => SetInt(MUSIC_ENABLED, enabled ? 1 : 0);

    public static bool IsLevelUnlocked(int levelIndex)
        => GetInt($"LevelOpened_{levelIndex}", 0) == 1;

    public static void UnlockLevel(int levelIndex)
        => SetInt($"LevelOpened_{levelIndex}", 1);
}
