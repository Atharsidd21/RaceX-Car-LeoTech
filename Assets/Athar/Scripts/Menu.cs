using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public static Menu Instance;
    private bool isEditingProfile = false;

    // ================= PROFILE KEYS =================
    public static string nameStr = "username";
    public static string PlayerAvatarKey = "PLAYER_AVATAR_INDEX";
    public static string IsGuestKey = "IS_GUEST";
    public static string ProfileCompletedKey = "PROFILE_COMPLETED";
    private static bool splashPlayedThisSession = false;

    // ================= NAVIGATION KEYS =================
    public static string ShowLeaderBoard = "ShowLeaderBoard";
    public static string LeaderboardRank = "LeaderboardRank";
    public static string GotoHome = "GoingToHome";
    public static string SelectedLevel = "SelectedLevel";
    public static string GotoLevelSelection = "IsGoLevelSelection";

    // ================= PROFILE UI =================
    [Header("Player Profile UI")]
    public Image avatarPreview;
    public Sprite[] avatarSprites;
    private int selectedAvatarIndex = 0;
    [SerializeField] private Image profileAvatarImage;
    [SerializeField] private TextMeshProUGUI profileNameText;
    [SerializeField] private TextMeshProUGUI profileCoinsText;
    [Header("Level Menu Profile UI")]
    [SerializeField] private TextMeshProUGUI levelProfileNameText;
    [SerializeField] private Image levelProfileAvatarImage;
    [SerializeField] private TextMeshProUGUI levelProfileCoinsText;
    [Header("Player Profile Views")]
    [SerializeField] private PlayerProfileView mainMenuProfileView;
    [SerializeField] private PlayerProfileView levelMenuProfileView;


    // ================= FIXED GUEST =================
    [Header("Fixed Guest Profile UI")]
    [SerializeField] private string fixedGuestName = "Guest";
    [SerializeField] private Sprite fixedGuestAvatar;
    //Edit Player Profile
    [Header("Edit Profile UI")]
    [SerializeField] private GameObject EditProfilePanel;
    [SerializeField] private Image editProfileAvatarImage;
    [SerializeField] private TMP_InputField editNameInputField;
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private TextMeshProUGUI carsOwnedText;
    [SerializeField] private TextMeshProUGUI racesWonText;
    [SerializeField] private TextMeshProUGUI racesLostText;
    private int savedAvatarIndex;
    private string savedPlayerName;

    // ================= PANELS =================
    [Header("Panels")]
    public GameObject SplashScreen;
    public GameObject MainMenuScreen;
    // public GameObject OptionPanel;
    public GameObject MainMenuPanel;
    public GameObject LevelPanel;
    // public GameObject PlayerProfilePanel;
    public GameObject EnterNamePanel;
    public GameObject AvatarSelectionPanel;
    //  public GameObject LeaderboardPanel;

    // ================= UI ELEMENTS =================
    [Header("UI Elements")]
    public RectTransform logo;
    public Slider loadingBar;
    public Text loadingPercentageTextObj;
    [SerializeField] private float logoAnimDuration = 0.6f;
    [SerializeField] private float loadingDuration = 2.5f;

    public InputField enterNameInputFieldNameScreen;

    // public List<GameObject> playerListLeaderboard;
    [Header("Audio Button")]
    [SerializeField] private Button musicButton;
    [SerializeField] private Image musicButtonImage;
    [SerializeField] private Sprite musicOnSprite;
    [SerializeField] private Sprite musicOffSprite;

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
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        DebugPlayerPrefsLocation();
        //  Debug.unityLogger.logEnabled = false;
    }

    private void Start()
    {


        Debug.Log("Menu Start");
        // PlayerPrefs.DeleteAll();
        // ALWAYS show splash first
        // StartCoroutine(SplashFlow());
        UpdateMusicButtonUI();//for audio button

        MusicManager.Instance.PlayMusic();

        //   Time.timeScale = 1f;

        // Always hide panels initially
        // SplashScreen.SetActive(false);
        //  MainMenuPanel.SetActive(false);
        // PlayerProfilePanel.SetActive(false);

        // LevelPanel.SetActive(false);

        // loadingBar.gameObject.SetActive(false);

        // ✅ Play splash ONLY once per app launch
        if (!splashPlayedThisSession)
        {
            Debug.Log("Playing splash screen");
            splashPlayedThisSession = true;
            SplashScreen.SetActive(true);
            StartCoroutine(SplashFlow());
            return;
        }
        else
        {
            Debug.Log("Skipping splash screen");
        }

        // 🔹 Normal navigation (NO splash)
        HandlePostSplashNavigation();

        // loadingBar.gameObject.SetActive(false);

        // 1️⃣ RETURNING FROM SELECT CAR → LEVEL SELECT
        /*if (PlayerPrefs.GetInt(GotoLevelSelection, 0) == 1)
         {
             PlayerPrefs.SetInt(GotoLevelSelection, 0);
             DisableAllPanels();
             LevelPanel.SetActive(true);
             return;
         }


         // 2️⃣ SHOW LEADERBOARD (AFTER RACE)
         /* if (PlayerPrefs.GetInt(ShowLeaderBoard, 0) == 1)
          {
              PlayerPrefs.SetInt(ShowLeaderBoard, 0);
              int rank = PlayerPrefs.GetInt(LeaderboardRank, 0);
              DisableAllPanels();
              LeaderboardPanel.SetActive(true);
              ShowLeaderboard(rank);
              return;
          }*/
        // Time.timeScale = 1f;

        /* Hide everything initially
        SplashScreen.SetActive(true);
        MainMenuPanel.SetActive(false);
        LevelPanel.SetActive(false);

        loadingBar.gameObject.SetActive(false);

        // 3️⃣ NORMAL ENTRY
      //  LoadPlayerProfile();*/

        // this part changed by rajan. for multiplayer.
        if (!PhotonNetwork.InRoom)
        {
            GameModeManager.IsMultiplayer = false;
        }

        if (GameModeManager.IsMultiplayer &&
        PhotonNetwork.InRoom &&
        PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("GoToLevelSelect"))
        {
            LevelsBtnClicked(); // tumhara existing method
            PhotonNetwork.CurrentRoom.CustomProperties.Remove("GoToLevelSelect");
        }

    }
    //Splash Screen
    private IEnumerator SplashFlow()
    {

        Debug.Log("SplashFlow started");

        // 🔒 Safety reset
        Time.timeScale = 1f;

        // --- Initial State ---
        SplashScreen.SetActive(true);
        MainMenuScreen.SetActive(false);

        loadingBar.gameObject.SetActive(true);
        loadingBar.value = 0f;
        loadingPercentageTextObj.text = "0%";

        // Logo starts hidden
        logo.localScale = Vector3.zero;

        // --- Step 1: Logo scale-in animation ---
        float t = 0f;
        while (t < logoAnimDuration)
        {
            t += Time.deltaTime;
            float progress = t / logoAnimDuration;

            // Smooth ease-out
            float eased = Mathf.Sin(progress * Mathf.PI * 0.5f);
            logo.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, eased);

            yield return null;
        }

        logo.localScale = Vector3.one;

        // --- Small pause ---
        yield return new WaitForSeconds(0.3f);

        // --- Step 2: Fake loading bar ---
        float timer = 0f;
        while (timer < loadingDuration)
        {
            timer += Time.deltaTime;
            float normalized = Mathf.Clamp01(timer / loadingDuration);

            loadingBar.value = normalized;
            loadingPercentageTextObj.text = Mathf.RoundToInt(normalized * 100f) + "%";

            yield return null;
        }

        loadingBar.value = 1f;
        loadingPercentageTextObj.text = "100%";

        // --- Step 3: Switch to Main Menu ---
        yield return new WaitForSeconds(0.2f);

        SplashScreen.SetActive(false);
        HandlePostSplashNavigation();

    }

    private void HandlePostSplashNavigation()
    {
        // DisableAllPanels();

        // 1️⃣ If profile not completed → force name entry
        if (PlayerPrefs.GetInt(ProfileCompletedKey, 0) != 1)
        {
            Debug.Log("Profile not completed, showing Enter Name panel");
            EnterNamePanel.SetActive(true);
            return;
        }

        // 2️⃣ Return from race → Main Menu
        if (PlayerPrefs.GetInt(GotoHome, 0) == 1)
        {
            PlayerPrefs.SetInt(GotoHome, 0);
            MainMenuPanel.SetActive(true);
            UpdatePlayerProfileUI();
            return;
        }

        // 3️⃣ Return from car selection → Level menu
        if (PlayerPrefs.GetInt(GotoLevelSelection, 0) == 1)
        {
            PlayerPrefs.SetInt(GotoLevelSelection, 0);
            LevelPanel.SetActive(true);
            UpdatePlayerProfileUI();
            return;
        }

        // 4️⃣ Default → Main Menu
        MainMenuPanel.SetActive(true);
        UpdatePlayerProfileUI();

    }

    // ================= PROFILE FLOW =================
    public void OnEnterNameYes()
    {
        if (enterNameInputFieldNameScreen.text.Length <= 2)
            return;

        PlayerPrefs.SetString(nameStr, enterNameInputFieldNameScreen.text);
        PlayerPrefs.SetInt(IsGuestKey, 0);// IMPORTANT
        PlayerPrefs.Save();

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

        savedAvatarIndex = selectedAvatarIndex; // sync saved state
        AvatarSelectionPanel.SetActive(false);
        if (isEditingProfile)
        {
            // 🟢 EDIT MODE
            isEditingProfile = false;
            EditProfilePanel.SetActive(true);
            LoadEditProfileData();   // refresh avatar + name
                                     //  LoadPlayerProfile();

        }
        else
        {
            // 🟢 FIRST-TIME PROFILE CREATION
            MainMenuPanel.SetActive(true);
            UpdatePlayerProfileUI();
        }
        /*  MainMenuPanel.SetActive(true);
          UpdatePlayerProfileUI();   // REQUIRED
          EditProfilePanel.SetActive(false);
          LoadPlayerProfile();
          LoadEditProfileData();*/
    }

    public void PlayAsGuest()
    {
        PlayerPrefs.SetString(nameStr, fixedGuestName);
        PlayerPrefs.SetInt(IsGuestKey, 1);
        PlayerPrefs.SetInt(ProfileCompletedKey, 1);
        PlayerPrefs.Save();

        DisableAllPanels();
        MainMenuPanel.SetActive(true);
        UpdatePlayerProfileUI();
    }

    void LoadPlayerProfile()
    {
        if (PlayerPrefs.GetInt(ProfileCompletedKey, 0) != 1)
        {
            DisableAllPanels();
            EnterNamePanel.SetActive(true);
            return;
        }

        username = PlayerPrefs.GetString(nameStr, fixedGuestName);
        // username = PlayerPrefs.GetString(nameStr, "");

        selectedAvatarIndex = PlayerPrefs.GetInt(PlayerAvatarKey, 0);

        enterNameInputFieldNameScreen.text = username;
        avatarPreview.sprite = avatarSprites[selectedAvatarIndex];

        DisableAllPanels();
        MainMenuPanel.SetActive(true);
    }
    //Player Profile Display
    /* private void UpdatePlayerProfileUI()
     {
         // Player name
         string playerName = PlayerPrefs.GetString(nameStr, "Guest");
         profileNameText.text = playerName;

         // Coins
         int coins = PlayerPrefs.GetInt("Currency", 0);
         profileCoinsText.text = coins.ToString();

         // Avatar
         bool isGuest = PlayerPrefs.GetInt(IsGuestKey, 0) == 1;
         int avatarIndex = PlayerPrefs.GetInt(PlayerAvatarKey, 0);

         if (isGuest)
         {
             profileAvatarImage.sprite = fixedGuestAvatar;
         }
         else
         {
             avatarIndex = Mathf.Clamp(avatarIndex, 0, avatarSprites.Length - 1);
             profileAvatarImage.sprite = avatarSprites[avatarIndex];
         }
         // --- Main Menu Profile UI ---
         if (profileNameText != null)
             profileNameText.text = playerName;

         if (profileCoinsText != null)
             profileCoinsText.text = coins.ToString();

         if (profileAvatarImage != null)
             profileAvatarImage.sprite = avatarSprite;

         // --- Level Menu Profile UI ---
         if (levelProfileNameText != null)
             levelProfileNameText.text = playerName;

         if (levelProfileCoinsText != null)
             levelProfileCoinsText.text = coins.ToString();

         if (levelProfileAvatarImage != null)
             levelProfileAvatarImage.sprite = avatarSprite;
     }*/
    /* private void UpdatePlayerProfileUI()
      {
          // --- Read data once ---
          string playerName = PlayerPrefs.GetString(nameStr, "Guest");
          int coins = PlayerPrefs.GetInt("Currency", 0);
          bool isGuest = PlayerPrefs.GetInt(IsGuestKey, 0) == 1;
          int avatarIndex = PlayerPrefs.GetInt(PlayerAvatarKey, 0);

          Sprite avatarSprite;
          if (isGuest)
          {
              avatarSprite = fixedGuestAvatar;
          }
          else
          {
              avatarIndex = Mathf.Clamp(avatarIndex, 0, avatarSprites.Length - 1);
              avatarSprite = avatarSprites[avatarIndex];
          }

          // --- Main Menu Profile UI ---
          if (profileNameText != null)
              profileNameText.text = playerName;

          if (profileCoinsText != null)
              profileCoinsText.text = coins.ToString();

          if (profileAvatarImage != null)
              profileAvatarImage.sprite = avatarSprite;

          // --- Level Menu Profile UI ---
          if (levelProfileNameText != null)
              levelProfileNameText.text = playerName;

          if (levelProfileCoinsText != null)
              levelProfileCoinsText.text = coins.ToString();

          if (levelProfileAvatarImage != null)
              levelProfileAvatarImage.sprite = avatarSprite;
      }*/

    private void UpdatePlayerProfileUI()
    {
        string playerName = PlayerPrefs.GetString(nameStr, "Guest");
        int coins = PlayerPrefs.GetInt("Currency", 0);
        bool isGuest = PlayerPrefs.GetInt(IsGuestKey, 0) == 1;
        int avatarIndex = PlayerPrefs.GetInt(PlayerAvatarKey, 0);

        Sprite avatarSprite = isGuest
            ? fixedGuestAvatar
            : avatarSprites[Mathf.Clamp(avatarIndex, 0, avatarSprites.Length - 1)];

        mainMenuProfileView?.Refresh(playerName, coins, avatarSprite);
        levelMenuProfileView?.Refresh(playerName, coins, avatarSprite);
    }


    //Edit Player Profile
    public void OnProfileAvatarClicked()
    {
        // DisableAllPanels();
        EditProfilePanel.SetActive(true);
        LoadEditProfileData();

    }
    private void LoadEditProfileData()
    {
        // Save current profile state
        savedPlayerName = PlayerPrefs.GetString(nameStr, fixedGuestName);
        savedAvatarIndex = PlayerPrefs.GetInt(PlayerAvatarKey, 0);

        // Apply to edit UI
        editNameInputField.text = savedPlayerName;
        editNameInputField.interactable = false;

        selectedAvatarIndex = savedAvatarIndex;

        bool isGuest = PlayerPrefs.GetInt(IsGuestKey, 0) == 1;
        editProfileAvatarImage.sprite = isGuest
            ? fixedGuestAvatar
            : avatarSprites[Mathf.Clamp(savedAvatarIndex, 0, avatarSprites.Length - 1)];

        // Stats
        coinsText.text = PlayerPrefs.GetInt("Currency", 0).ToString();
        carsOwnedText.text = PlayerPrefs.GetInt("CarsOwned", 0).ToString();
        racesWonText.text = PlayerPrefs.GetInt("RacesWon", 0).ToString();
        racesLostText.text = PlayerPrefs.GetInt("RacesLost", 0).ToString();
    }

    //Inside Edit Profile Panel
    public void OnEditNameClicked()
    {
        editNameInputField.interactable = true;
        editNameInputField.ActivateInputField();
    }
    public void OnSaveProfileChanges()
    {
        string newName = editNameInputField.text;

        if (newName.Length > 2)
        {
            PlayerPrefs.SetString(nameStr, newName);
            PlayerPrefs.SetInt(PlayerAvatarKey, selectedAvatarIndex);
            PlayerPrefs.SetInt(IsGuestKey, 0);
            PlayerPrefs.Save();
        }

        UpdatePlayerProfileUI();
        EditProfilePanel.SetActive(false);

    }

    public void OnCancelEditProfile()
    {
        // Revert name
        editNameInputField.text = savedPlayerName;
        editNameInputField.interactable = false;

        // Revert avatar
        selectedAvatarIndex = savedAvatarIndex;
        editProfileAvatarImage.sprite =
            avatarSprites[Mathf.Clamp(savedAvatarIndex, 0, avatarSprites.Length - 1)];



        EditProfilePanel.SetActive(false);
        LoadEditProfileData();
    }


    public void OnEditAvatarClicked()
    {
        isEditingProfile = true;
        EditProfilePanel.SetActive(false);
        AvatarSelectionPanel.SetActive(true);
    }
    public void OnCloseEditProfile()
    {
        EditProfilePanel.SetActive(false);
        MainMenuPanel.SetActive(true);
        UpdatePlayerProfileUI();
    }



    // ================= NAVIGATION =================
    public void GarageBtnClicked()
    {
        GameModeManager.IsMultiplayer = false;      //changes by rajan for multiplayer
        //SceneManager.LoadScene("SelectCar");
        //SceneTransition.LoadScene("SelectCar"); Transition 1
        SceneTransition.Instance.LoadSceneSlide(1, true); //Transition 2
                                                          // SceneTransition.Instance.LoadSceneFast(1);
        

    }

    public void LevelsBtnClicked()
    {
        // DisableAllPanels();
        LevelPanel.SetActive(true);
        // PlayerProfileController.Instance?.RefreshProfile(levelMenuProfileView);

    }

    public void PlayButtonClicked()
    {
        GameModeManager.IsMultiplayer = false;
        // SceneManager.LoadScene(1);
        SceneTransition.Instance.LoadSceneSlide(1, true); //Transition 2

    }

    // Multiplayer Button Clicked
    //changes by rajan for multiplayer
    public void OnMultiplayerClick()
    {
        GameModeManager.IsMultiplayer = true;
        SceneManager.LoadScene("MultiplayerLobby");
    }


    public void BackBtnClicked()
    {
        DisableAllPanels();
        MainMenuPanel.SetActive(true);
        UpdatePlayerProfileUI();

    }

    public void CloseBtnClicked()
    {
        LevelPanel.SetActive(false);
        MainMenuPanel.SetActive(true);
        UpdatePlayerProfileUI();


        // OptionPanel.SetActive(true);
    }

    public void OptionBtnClicked()
    {
        DisableAllPanels();
        //  OptionPanel.SetActive(true);
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
        // OptionPanel.SetActive(false);
    }
    // For on/off music button
    public void OnMusicButtonClicked()
    {
        bool isMusicOn = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;

        isMusicOn = !isMusicOn; // toggle

        PlayerPrefs.SetInt("MusicEnabled", isMusicOn ? 1 : 0);
        PlayerPrefs.Save();

        MusicManager.Instance.ApplyMusicState();
        UpdateMusicButtonUI();
    }
    private void UpdateMusicButtonUI()
    {
        bool isMusicOn = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;

        musicButtonImage.sprite = isMusicOn
            ? musicOnSprite
            : musicOffSprite;
    }

    public void DebugPlayerPrefsLocation()
    {
        Debug.Log($"PlayerPrefs Path (Editor): HKEY_CURRENT_USER\\Software\\Unity\\UnityEditor\\{Application.companyName}\\{Application.productName}");
        Debug.Log($"username: {PlayerPrefs.GetString(nameStr, "NOT SET")}");
        Debug.Log($"avatar: {PlayerPrefs.GetInt(PlayerAvatarKey, -1)}");
        Debug.Log($"profileCompleted: {PlayerPrefs.GetInt(ProfileCompletedKey, -1)}");
    }
}