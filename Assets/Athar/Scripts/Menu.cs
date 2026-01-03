using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public static string nameStr = "username";
    public static string ShowLeaderBoard = "ShowLeaderBoard";
    public static string LeaderboardRank = "LeaderboardRank";
    public static string GotoHome = "GoingToHome";
    public static string SelectedLevel = "SelectedLevel";
    public static string GotoLevelSelection = "IsGoLevelSelection";

    public GameObject SplashScreen;
    public GameObject MainMenuScreen;

    public GameObject OptionPanel;
    public GameObject MainMenuPanel;
    public GameObject LevelPanel;
    public GameObject EnterNamePanel;
    public GameObject LeaderboardPanel;
    public VehicleLiist ListOfVehicles; // Add reference to the car list


    [Header("UI Elements")]
    public RectTransform logo;
    public Slider loadingBar; // or Image for fill
    public Text loadingPercentageTextObj;
    public InputField enterNameInputFieldNameScreen;
    public List<GameObject> playerListLeaderboard;
    //public AudioSource voiceOver;

    [Header("Timings")]
    public float smashDuration = 0.5f;
    public float delayBeforeLoading = 1f;
    public float loadFillDuration = 5f;
    public Text loadingText;
    public AudioSource backGroundMusic;

    public static List<string> playerName = new List<string>
    {
        "Liam",
        "Rado",
        "Kenny",
        "William",
        "Rahino3",
        "Chad",
        "Rachel",
        "Joey",
        "Rocky",
        "Will",
        "Smith",
        "Tom",
        "Helen",
        "Natlie",
        "Kim",
        "Vicktor",
        "Dyatlov"
    };
    string username;

    public void Awake()
    {
        Debug.unityLogger.logEnabled = false;
    }

    private void Start()
    {
        MusicManager.Instance.PlayMusic(); // Ensure background music starts playing

        loadingBar.gameObject.SetActive(false);
        if (PlayerPrefs.GetInt(GotoHome, 0) == 1)
        {
            MainMenuPanel.gameObject.SetActive(true);
            PlayerPrefs.SetInt(GotoHome, 0);
            backGroundMusic.Play();
        }
        else if (PlayerPrefs.GetInt(ShowLeaderBoard, 0) == 3)//1)
        {
            //PlayerPrefs.SetInt(ShowLeaderBoard, 0);
            //ShowLeaderboard(PlayerPrefs.GetInt(LeaderboardRank, 0));
            //LeaderboardPanel.gameObject.SetActive(true);
        }
        else if (PlayerPrefs.GetInt(GotoLevelSelection, 0) == 1)
        {
            PlayerPrefs.SetInt(GotoLevelSelection, 0);
            LevelPanel.SetActive(true);
            backGroundMusic.Play();
        }
        else
        {
            StartCoroutine(PlaySplashSequence());
        }
        username = PlayerPrefs.GetString(nameStr, "");
        enterNameInputFieldNameScreen.text = username;
    }

    private IEnumerator PlaySplashSequence()
    {
        // --- Step 1: Smash Logo into center ---
        Vector3 startScale = Vector3.zero;
        Vector3 endScale = Vector3.one;
        float t = 0f;
        while (t < smashDuration)
        {
            t += Time.deltaTime;
            float progress = t / smashDuration;
            // Ease Out Back effect
            float overshoot = 1.2f;
            float eased = Mathf.Sin(progress * Mathf.PI * 0.5f);
            logo.localScale = Vector3.LerpUnclamped(startScale, endScale * overshoot, eased);
            yield return null;
        }
        logo.localScale = endScale;

        // --- Step 2: Play Voiceover ---
        //if (voiceOver) voiceOver.Play();

        yield return new WaitForSeconds(delayBeforeLoading);

        // --- Step 3: Start Loading Bar ---
        loadingBar.gameObject.SetActive(true);
        float timer = 0f;
        while (timer < loadFillDuration)
        {
            timer += Time.deltaTime;
            loadingBar.value = Mathf.Clamp01(timer / loadFillDuration);
            //loadingText.text = Mathf.FloorToInt(randomProgress * 100f) + "%";
            loadingPercentageTextObj.text = (int)(loadingBar.value * 100) + "%";
            yield return null;
        }

        // (Optional) Load next scene
        //SceneManager.LoadScene("MainMenu");
        SplashScreen.SetActive(false);
        MainMenuScreen.SetActive(true);
        backGroundMusic.Play();
        if (username == "" || username == default)
        {
            EnterNamePanel.SetActive(true);
        }
    }

    public void PlayButtonClicked()
    {
        //int selectedIndex = PlayerPrefs.GetInt("Pointer", 0);
        //GameObject selectedCar = ListOfVehicles.Vehicals[selectedIndex];
        //string carName = selectedCar.GetComponent<CarController>().CarName;
        //GarageBtnClicked();
        //  Only allow playing if the car is owned
        //if (PlayerPrefs.GetInt(carName, 0) == 1)
        //{
        //OnClickLetsPlayBtn();
        //LevelPanel.SetActive(true);
        SceneManager.LoadScene(1);
        //}
    }

    public void OnClickYesButtonNameButton()
    {
        if (enterNameInputFieldNameScreen.text == "" || enterNameInputFieldNameScreen.text.Length <= 2)
        {
            return;
        }
        username = enterNameInputFieldNameScreen.text;
        PlayerPrefs.SetString(nameStr, username);
        EnterNamePanel.SetActive(false);
        MainMenuPanel.SetActive(true);
    }

    public void OnClickCancelButtonNamePanel()
    {
        EnterNamePanel.SetActive(false);
        MainMenuPanel.SetActive(true);
    }

    public void ShowLeaderboard(int rank)
    {
        List<string> names = new List<string>();
        for (int i = 0; i < playerName.Count; i++)
        {
            names.Add(playerName[i]);
        }
        for (int i = 0; i < 5; i++)
        {
            if (i == rank)
            {
                if (string.IsNullOrEmpty(username))
                {
                    username = "username" + UnityEngine.Random.Range(000000, 999999);
                    PlayerPrefs.SetString(nameStr, username);
                }
                playerListLeaderboard[i].transform.GetChild(2).GetChild(0).GetComponent<Text>().text = username;
            }
            else
            {
                int ind = UnityEngine.Random.Range(0, names.Count() - 1);
                playerListLeaderboard[i].transform.GetChild(2).GetChild(0).GetComponent<Text>().text = names[ind];
                names.RemoveAt(ind);
            }
        }
    }

    public void OptionBtnClicked()
    {
        OptionPanel.SetActive(true);
        MainMenuPanel.SetActive(false);
    }
    public void OnClickLetsPlayBtn()
    {
        //Title.SetActive(false);
        MainMenuPanel.SetActive(true);
    }
    public void QuitBtnClicked()
    {
        Application.Quit();
    }
    public void GarageBtnClicked()
    {
        SceneManager.LoadScene("SelectCar");
    }
    public void LevelsBtnClicked()
    {
        LevelPanel.SetActive(true);
        OptionPanel.SetActive(false);
        MainMenuPanel.SetActive(false);
    }
    public void BackBtnClicked()
    {
        OptionPanel.SetActive(false);
        MainMenuPanel.SetActive(true);
    }
    public void CloseBtnClicked()
    {
        LevelPanel.SetActive(false);
        OptionPanel.SetActive(true);
    }
}