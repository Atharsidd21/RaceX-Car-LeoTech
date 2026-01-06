using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public static Menu Instance;

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
    [Header("Player Profile")]
    public Image avatarPreview;
    public Sprite[] avatarSprites;
    private int selectedAvatarIndex = 0;

    // ================= FIXED GUEST =================
    [Header("Fixed Guest Profile")]
    [SerializeField] private string fixedGuestName = "Guest";
    [SerializeField] private Sprite fixedGuestAvatar;

    // ================= PANELS =================
    [Header("Panels")]
    public GameObject SplashScreen;
    public GameObject MainMenuScreen;
    public GameObject OptionPanel;
    public GameObject MainMenuPanel;
    public GameObject LevelPanel;
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
        Debug.unityLogger.logEnabled = false;
    }

    private void Start()
    {
       //PlayerPrefs.DeleteAll();
        // ALWAYS show splash first
       // StartCoroutine(SplashFlow());
        MusicManager.Instance.PlayMusic();

        Time.timeScale = 1f;

        // Always hide panels initially
        SplashScreen.SetActive(false);
        MainMenuPanel.SetActive(false);
        LevelPanel.SetActive(false);

        loadingBar.gameObject.SetActive(false);

        // ✅ Play splash ONLY once per app launch
        if (!splashPlayedThisSession)
        {
            splashPlayedThisSession = true;
            SplashScreen.SetActive(true);
            StartCoroutine(SplashFlow());
            return;
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
        if (PlayerPrefs.GetInt(GotoHome, 0) == 1)
        {
            PlayerPrefs.SetInt(GotoHome, 0);
            MainMenuPanel.SetActive(true);
            backGroundMusic.Play();
            LoadPlayerProfile();
            
            return;
        }

        if (PlayerPrefs.GetInt(GotoLevelSelection, 0) == 1)
        {
            PlayerPrefs.SetInt(GotoLevelSelection, 0);
            LevelPanel.SetActive(true);
            backGroundMusic.Play();
            return;
        }

        // Default
        MainMenuPanel.SetActive(true);
       // backGroundMusic.Play();
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
        PlayerPrefs.SetString(nameStr, fixedGuestName);
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

        username = PlayerPrefs.GetString(nameStr, fixedGuestName);
        selectedAvatarIndex = PlayerPrefs.GetInt(PlayerAvatarKey, 0);

        enterNameInputFieldNameScreen.text = username;
        avatarPreview.sprite = avatarSprites[selectedAvatarIndex];

        DisableAllPanels();
        MainMenuPanel.SetActive(true);
    }

    //================ LEADERBOARD =================
  /*  public void ShowLeaderboard(int rank)
    {
        List<string> aiNames = new List<string>(playerName);
        List<Sprite> avatars = new List<Sprite>(avatarSprites);


        bool isGuest = PlayerPrefs.GetInt(IsGuestKey, 0) == 1;

        string playerUsername = PlayerPrefs.GetString(nameStr, fixedGuestName);
        int playerAvatarIndex = PlayerPrefs.GetInt(PlayerAvatarKey, 0);

        for (int i = 0; i < playerListLeaderboard.Count; i++)
        {
            GameObject row = playerListLeaderboard[i];

            Text nameText = row.transform
                .GetChild(2)
                .GetChild(0)
                .GetComponent<Text>();

            Image avatarImage = row.transform
                .GetChild(1)
                .GetComponent<Image>();

            if (i == rank)
            {
                // ✅ PLAYER / GUEST ROW
                nameText.text = playerUsername;

                if (isGuest)
                {
                    // FIXED guest avatar
                    avatarImage.sprite = fixedGuestAvatar;
                }
                else
                {
                    // Player avatar from PlayerPrefs
                    avatarImage.sprite = avatarSprites[playerAvatarIndex];
                }
            }
            else
            {
                // ✅ AI ROW (unchanged)
                int ind = Random.Range(0, aiNames.Count);
                nameText.text = aiNames[ind];
                aiNames.RemoveAt(ind);

                avatarImage.sprite = avatarSprites[
                    Random.Range(0, avatarSprites.Length)
                ];
            }
        }
    }*/


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

    public void PlayButtonClicked()
    {
        SceneManager.LoadScene(1);
    }

    public void BackBtnClicked()
    {
        DisableAllPanels();
        MainMenuPanel.SetActive(true);
    }

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
      //  LeaderboardPanel.SetActive(false);
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
