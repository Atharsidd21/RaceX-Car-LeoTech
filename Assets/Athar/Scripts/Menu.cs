using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    // ================= PROFILE KEYS =================
    public static string nameStr = "username";
    public static string PlayerAvatarKey = "PLAYER_AVATAR_INDEX";
    public static string IsGuestKey = "IS_GUEST";
    public static string ProfileCompletedKey = "PROFILE_COMPLETED";

    // ================= NAVIGATION KEYS =================
    public static string ShowLeaderBoard = "ShowLeaderBoard";
    public static string LeaderboardRank = "LeaderboardRank";
    public static string GotoHome = "GoingToHome";
    public static string SelectedLevel = "SelectedLevel";
    public static string GotoLevelSelection = "IsGoLevelSelection";

    // ================= PROFILE UI =================
    [Header("Player Profile")]
    public Image avatarPreview;
    public Sprite[] avatarSprites;
    private int selectedAvatarIndex = 0;

    // ================= PANELS =================
    [Header("Panels")]
    public GameObject SplashScreen;
    public GameObject MainMenuScreen;
    public GameObject OptionPanel;
    public GameObject MainMenuPanel;
    public GameObject LevelPanel;
    public GameObject EnterNamePanel;
    public GameObject AvatarSelectionPanel;
    public GameObject LeaderboardPanel;

    // ================= UI ELEMENTS =================
    [Header("UI Elements")]
    public RectTransform logo;
    public Slider loadingBar;
    public Text loadingPercentageTextObj;
    public InputField enterNameInputFieldNameScreen;
    public List<GameObject> playerListLeaderboard;
    public AudioSource backGroundMusic;

    [Header("Timings")]
    public float smashDuration = 0.5f;
    public float delayBeforeLoading = 1f;
    public float loadFillDuration = 5f;

    // Dummy AI names
    public static List<string> playerName = new List<string>
    {
        "Liam","Rado","Kenny","William","Rahino3","Chad","Rachel",
        "Joey","Rocky","Will","Smith","Tom","Helen","Natlie",
        "Kim","Vicktor","Dyatlov"
    };

    string username;

    // ================= UNITY =================
    private void Awake()
    {
        DebugPlayerPrefsLocation();
        Debug.unityLogger.logEnabled = false;
    }

    private void Start()
    {
        MusicManager.Instance.PlayMusic();
        loadingBar.gameObject.SetActive(false);

        // 1️⃣ RETURNING FROM SELECT CAR → LEVEL SELECT
        if (PlayerPrefs.GetInt(GotoLevelSelection, 0) == 1)
        {
            PlayerPrefs.SetInt(GotoLevelSelection, 0);
            DisableAllPanels();
            LevelPanel.SetActive(true);
            return;
        }

        // 2️⃣ SHOW LEADERBOARD (AFTER RACE)
        if (PlayerPrefs.GetInt(ShowLeaderBoard, 0) == 1)
        {
            PlayerPrefs.SetInt(ShowLeaderBoard, 0);
            int rank = PlayerPrefs.GetInt(LeaderboardRank, 0);
            DisableAllPanels();
            LeaderboardPanel.SetActive(true);
            ShowLeaderboard(rank);
            return;
        }

        // 3️⃣ NORMAL ENTRY
        LoadPlayerProfile();
    }

    // ================= PROFILE FLOW =================
    public void OnEnterNameYes()
    {
        if (enterNameInputFieldNameScreen.text.Length <= 2)
            return;

        PlayerPrefs.SetString(nameStr, enterNameInputFieldNameScreen.text);

        EnterNamePanel.SetActive(false);
        AvatarSelectionPanel.SetActive(true);
    }

    public void SelectAvatar(int avatarIndex)
    {
        selectedAvatarIndex = avatarIndex;
        avatarPreview.sprite = avatarSprites[avatarIndex];
    }

    public void ConfirmProfile()
    {
        PlayerPrefs.SetInt(PlayerAvatarKey, selectedAvatarIndex);
        PlayerPrefs.SetInt(IsGuestKey, 0);
        PlayerPrefs.SetInt(ProfileCompletedKey, 1);
        PlayerPrefs.Save();

        AvatarSelectionPanel.SetActive(false);
        MainMenuPanel.SetActive(true);
    }

    public void PlayAsGuest()
    {
        PlayerPrefs.SetString(nameStr, "Guest");
        PlayerPrefs.SetInt(PlayerAvatarKey, 0);
        PlayerPrefs.SetInt(IsGuestKey, 1);
        PlayerPrefs.SetInt(ProfileCompletedKey, 1);
        PlayerPrefs.Save();

        DisableAllPanels();
        MainMenuPanel.SetActive(true);
    }

    void LoadPlayerProfile()
    {
        if (PlayerPrefs.GetInt(ProfileCompletedKey, 0) != 1)
        {
            DisableAllPanels();
            EnterNamePanel.SetActive(true);
            return;
        }

        username = PlayerPrefs.GetString(nameStr, "Guest");
        selectedAvatarIndex = PlayerPrefs.GetInt(PlayerAvatarKey, 0);

        enterNameInputFieldNameScreen.text = username;
        avatarPreview.sprite = avatarSprites[selectedAvatarIndex];

        DisableAllPanels();
        MainMenuPanel.SetActive(true);
    }

    // ================= LEADERBOARD =================
    public void ShowLeaderboard(int rank)
    {
        List<string> names = new List<string>(playerName);
        string currentUsername = PlayerPrefs.GetString(nameStr, "Guest");

        for (int i = 0; i < playerListLeaderboard.Count; i++)
        {
            Text nameText = playerListLeaderboard[i]
                .transform.GetChild(2)
                .GetChild(0)
                .GetComponent<Text>();

            if (i == rank)
            {
                nameText.text = currentUsername;
            }
            else
            {
                int ind = Random.Range(0, names.Count);
                nameText.text = names[ind];
                names.RemoveAt(ind);
            }
        }
    }

    // ================= NAVIGATION =================
    public void GarageBtnClicked()
    {
        SceneManager.LoadScene("SelectCar");
    }

    public void LevelsBtnClicked()
    {
        DisableAllPanels();
        LevelPanel.SetActive(true);
    }

    //Play button 
    public void PlayButtonClicked()
    {
        SceneManager.LoadScene(1); // Level selection scene
    }
    public void BackBtnClicked()
    {
        DisableAllPanels();
        MainMenuPanel.SetActive(true);
    }
    // Close button in level selection
    public void CloseBtnClicked()
    {
        LevelPanel.SetActive(false);
        OptionPanel.SetActive(true);
    }

    public void OptionBtnClicked()
    {
        DisableAllPanels();
        OptionPanel.SetActive(true);
    }

    public void QuitBtnClicked()
    {
        Application.Quit();
    }

    // ================= HELPERS =================
    void DisableAllPanels()
    {
        EnterNamePanel.SetActive(false);
        AvatarSelectionPanel.SetActive(false);
        MainMenuPanel.SetActive(false);
        LevelPanel.SetActive(false);
        OptionPanel.SetActive(false);
        LeaderboardPanel.SetActive(false);
    }
    
    



    // ================= DEBUG =================
    public void DebugPlayerPrefsLocation()
    {
        Debug.Log($"PlayerPrefs Path (Editor): HKEY_CURRENT_USER\\Software\\Unity\\UnityEditor\\{Application.companyName}\\{Application.productName}");
        Debug.Log($"username: {PlayerPrefs.GetString(nameStr, "NOT SET")}");
        Debug.Log($"avatar: {PlayerPrefs.GetInt(PlayerAvatarKey, -1)}");
        Debug.Log($"profileCompleted: {PlayerPrefs.GetInt(ProfileCompletedKey, -1)}");
    }
}
