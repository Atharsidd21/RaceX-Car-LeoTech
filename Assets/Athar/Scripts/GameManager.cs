
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{   //Locking AI Cars 
    private bool forceLockCars = false;
    private bool raceFinished = false;



    // Coins for Reward
    [Header("Race Rewards")]
    [SerializeField] private int[] coinsByRank; //Setting coins based on rank
                                                // Setting up reward panel 
    [Header("Reward Panel")]
    [SerializeField] private GameObject rewardPanel;
    [SerializeField] private string rewardPrefix = "You earned";
    [SerializeField] private TextMeshProUGUI rewardText;
    [SerializeField] private string rewardSuffix = " coins earned!";
    [SerializeField] private TextMeshProUGUI totalCoinsText;
    [SerializeField] private TextMeshProUGUI resultTitleText;

    //Win/Loss Record
    private const string RACES_WON_KEY = "RacesWon";
    private const string RACES_LOST_KEY = "RacesLost";
    [Header("RCC Controls Canvas")]
    [SerializeField] private GameObject rccControlsCanvas;

   


    [Header("Leaderboard")]
    [SerializeField] private List<GameObject> playerListLeaderboard;
    [SerializeField] private Sprite[] avatarSprites;
    [SerializeField] private Sprite guestAvatar;
    [SerializeField] private string fixedGuestName = "Guest";

    // ADD THESE (Inspector fields)
    // [SerializeField] private Sprite[] avatarSprites;      // same avatars used in Menu

    public static GameManager Instance;
    [Header("Control UI")]
    private GameObject playerControlsUI;
    [SerializeField] private float controlsUIDelay = 0.4f;
    [SerializeField] float FadeDuration = 0.25f;
    [SerializeField] float PopUpValue = 40f;
    [SerializeField] float PopUpDistance = 0.35f;
    [SerializeField] private Ease PopEase = Ease.OutBack;

    [Header("Pause Panel Animation")]
    [SerializeField] private float pauseFadeDuration = 0.25f;
    [SerializeField] private float pausePopScale = 1f;
    [SerializeField] private float pauseStartScale = 0.85f;
    [SerializeField] private Ease pauseEase = Ease.OutBack;


    private RCC_Camera rccCamera;


    [Header("Enum States")]
    [SerializeField] private GameState currentState;


    [Header("UI")]
    public GameObject PausePanel;
    public GameObject PauseBtn;
    public GameObject GameOverPanel;
    public GameObject LevelCompletePanel;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI coinText;
    public GameObject LeaderboardPanel;
    //public List<GameObject> playerListLeaderboard;

    [Header("Race Start Countdown")]
    [SerializeField] private Image countdownImage;
    [SerializeField] private Sprite sprite3;
    [SerializeField] private Sprite sprite2;
    [SerializeField] private Sprite sprite1;
    [SerializeField] private Sprite spriteGo;
    [SerializeField] private float countdownDelay = 1f;

    [Header("Gameplay Settings")]
    [SerializeField] private float timer = 300f;
    [SerializeField] private float distanceToScoreFactor = 1f;

    private bool raceStarted = false;
    private bool isGameOver = false;

    private int currentScore = 0;
    private int coinScore = 0;
    private int currentHealth = 100;

    private float distanceTravelled = 0f;
    private Vector3 lastPosition;
    private int totalTargets = 0;


    private GameObject player;

    private const string CurrencyKey = "Currency";
    private string username = "";

    #region UNITY LIFECYCLE

    public enum GameState
    {
        Countdown,
        Playing,
        Paused,
        GameOver
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        StartCoroutine(RaceStartCountdown());


        coinScore = PlayerPrefs.GetInt(CurrencyKey, 0);
        username = PlayerPrefs.GetString(Menu.nameStr);

        // Lock player & AI immediately
        LockAllCars(true);

        // Start countdown directly
        currentState = GameState.Countdown;
        // StartRaceAfterCutscene(); // reuse existing countdown coroutine

        // Setup RCC camera for cinematic countdown
        StartCoroutine(SetupRCCCameraForCountdown());
    }
    private void FixedUpdate()
    {
        if (forceLockCars)
            LockAllCars(true);
    }


    private void Update()
    {
        if (player == null || isGameOver)
            return;

        HandleDistanceScore();

        if (!raceStarted)
            return;

        HandleTimer();
        if (player == null) return;

        RaceProgressTracker tracker = player.GetComponent<RaceProgressTracker>();
        if (tracker == null || tracker.finished) return;

    }

    #endregion

    #region
    private IEnumerator SetupRCCCameraForCountdown()
    {
        // Wait until RCC Camera exists
        while (rccCamera == null)
        {
            rccCamera = FindObjectOfType<RCC_Camera>();
            yield return null;
        }

        // Wait until RCC Camera has a player target
        while (rccCamera.cameraTarget == null ||
               rccCamera.cameraTarget.playerVehicle == null)
        {
            yield return null;
        }

        // Switch to CINEMATIC during countdown
        rccCamera.ChangeCamera(RCC_Camera.CameraMode.CINEMATIC);
    }



    /* public void ShowControlsUI()

     {
         if (playerControlsUI == null) return;

         playerControlsUI.SetActive(true);

         Canvas canvas = playerControlsUI.GetComponentInChildren<Canvas>(true);
         if (canvas == null) return;

         RectTransform rt = canvas.GetComponent<RectTransform>();
         CanvasGroup cg = canvas.GetComponent<CanvasGroup>();

         if (rt == null || cg == null) return;

         // Kill previous tweens
         DOTween.Kill(rt);
         DOTween.Kill(cg);

         // Initial state
         cg.alpha = 0f;

         Vector3 startPos = rt.anchoredPosition;
         rt.anchoredPosition = startPos + new Vector3(0, -PopUpDistance);

         // Fade in
         cg.DOFade(1f, FadeDuration).SetUpdate(true);

         // Small upward pop (THIS replaces scale)
         rt.DOAnchorPos(startPos, PopUpValue)
           .SetEase(PopEase)
           .SetUpdate(true);
     }*/
    public void ShowControlsUI()
    {
        if (playerControlsUI == null) return;

        playerControlsUI.SetActive(true);

        Canvas canvas = playerControlsUI.GetComponentInChildren<Canvas>(true);
        if (canvas == null) return;

        RectTransform rt = canvas.GetComponent<RectTransform>();
        CanvasGroup cg = canvas.GetComponent<CanvasGroup>();

        if (rt == null || cg == null) return;

        // Kill previous tweens (important)
        DOTween.Kill(rt);
        DOTween.Kill(cg);

        // Initial invisible state
        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;

        Vector2 startPos = rt.anchoredPosition;
        rt.anchoredPosition = startPos + new Vector2(0, -40f);

        // Fade in (unscaled time)
        cg.DOFade(1f, 0.25f)
          .SetUpdate(true)
          .OnComplete(() =>
          {
              cg.interactable = true;
              cg.blocksRaycasts = true;
          });

        // Pop-up movement
        rt.DOAnchorPos(startPos, 0.35f)
          .SetEase(Ease.OutBack)
          .SetUpdate(true);
    }



    /*public void StartRaceAfterCutscene()
    {
       // StartCoroutine(RaceStartCountdown());
    }*/

    private IEnumerator RaceStartCountdown()
    {
        // Ensure cars stay locked during countdown
        forceLockCars = true;

        LockAllCars(true);

        countdownImage.gameObject.SetActive(true);

        countdownImage.sprite = sprite3;
        AnimateCountdownImage();
        yield return new WaitForSeconds(countdownDelay);

        countdownImage.sprite = sprite2;
        AnimateCountdownImage();
        yield return new WaitForSeconds(countdownDelay);

        countdownImage.sprite = sprite1;
        AnimateCountdownImage();
        // SHOW CONTROLS SMOOTHLY HERE
        ShowControlsUI();
        yield return new WaitForSeconds(countdownDelay);

        countdownImage.sprite = spriteGo;
        AnimateCountdownImage();
        yield return new WaitForSeconds(0.6f);

        countdownImage.gameObject.SetActive(false);

        // START RACE
        LockAllCars(false);
        forceLockCars = false;

        raceStarted = true;
        currentState = GameState.Playing;
        // Switch RCC camera to TPS on race start
        if (rccCamera != null)
        {
            rccCamera.ChangeCamera(RCC_Camera.CameraMode.TPS);
        }


        // Adding a small delay befroe showing controls UI
        // if (playerControlsUI != null)
        //   playerControlsUI.SetActive(true);

    }

    #endregion

    #region PLAYER ASSIGNMENT

    public void AssignPlayer(GameObject car)
    {
        player = car;
        lastPosition = player.transform.position;
        RaceRankManager.Instance.RegisterCar(car.GetComponent<RaceProgressTracker>());

        // Use RCC canvas as player controls UI
        playerControlsUI = rccControlsCanvas;

        if (playerControlsUI != null)
        {
            playerControlsUI.SetActive(false); // ensure hidden at start
        }
        // Find controls UI inside spawned car

        /* Transform controls = player.transform.Find("ControlsUI");

         if (controls == null)
         {
             controls = player.transform.GetComponentInChildren<Canvas>(true)?.transform;
         }

         if (controls != null)
         {
             playerControlsUI = controls.gameObject;

             // HIDE IMMEDIATELY
             playerControlsUI.SetActive(false);
             //Debug.Log("Controls UI cached and hidden");
         }*/

    }

    #endregion

    #region GAMEPLAY LOGIC

    private void HandleDistanceScore()
    {
        float distance = Vector3.Distance(player.transform.position, lastPosition);

        if (distance > 0.01f)
        {
            distanceTravelled += distance;

            int newScore = Mathf.FloorToInt(distanceTravelled * distanceToScoreFactor);
            if (newScore > currentScore)
            {
                currentScore = newScore;
                UpdateScoreUI();
            }

            lastPosition = player.transform.position;
        }
    }

    private void HandleTimer()
    {
        timer -= Time.deltaTime;
        timerText.text = FormatTime(timer);

        if (timer <= 0)
            ShowGameOver();
    }

    private string FormatTime(float timeInSeconds)
    {
        timeInSeconds = Mathf.Max(0, timeInSeconds);
        int minutes = Mathf.FloorToInt(timeInSeconds / 60);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);
        return $"{minutes:00}:{seconds:00}";
    }

    private void UpdateScoreUI()
    {
        scoreText.text = $": {currentScore}";
        coinText.text = $": {coinScore}";
        healthText.text = $"Health: {currentHealth}";
    }
    
   

    #endregion

    #region DAMAGE / HEALTH / COINS

    public void AddCoins(int value)
    {
        coinScore += value;
        PlayerPrefs.SetInt(CurrencyKey, coinScore);
        PlayerPrefs.Save();
        UpdateScoreUI();
    }

    //Rewarding coins based on rank
    /*public void RewardPlayerByRank(int rank)
    {
        Debug.Log("🟢 GAMEMANAGER RECEIVED RANK: " + rank);

        int rewardCoins = 0;

        if (rank >= 0 && rank < coinsByRank.Length)
        {
            rewardCoins = coinsByRank[rank];
        }

        // Load existing currency
        int currentCoins = PlayerPrefs.GetInt(CurrencyKey, 0);

        // Add reward
        currentCoins += rewardCoins;

        // Save currency
        PlayerPrefs.SetInt(CurrencyKey, currentCoins);
        PlayerPrefs.Save();

        // Update reward UI text
        rewardText.text = $"{rewardPrefix} {rewardCoins} {rewardSuffix}";
        totalCoinsText.text = $"Total Coins: {currentCoins}";

        // Show reward panel
        rewardPanel.SetActive(true);
    }*/
    public void RewardPlayerByRank(int rank)
    {
        Debug.Log("GAMEMANAGER RECEIVED RANK: " + rank);

        // Calculate coins
        int rewardCoins = 0;
        if (rank >= 0 && rank < coinsByRank.Length)
            rewardCoins = coinsByRank[rank];

        // Update currency
        int currentCoins = PlayerPrefs.GetInt(CurrencyKey, 0);
        currentCoins += rewardCoins;
        PlayerPrefs.SetInt(CurrencyKey, currentCoins);
        PlayerPrefs.Save();

        // --------------------
        // WIN / LOSE LOGIC
        // --------------------
        bool isWin = rank <= 2;

        if (isWin)
        {
            resultTitleText.text = "YOU WIN!";
         //   resultTitleText.color = Color.yellow;
        }
        else
        {
            resultTitleText.text = "YOU LOSE";
          //  resultTitleText.color = Color.yellow;
        }

        // Reward text
        rewardText.text = $"{rewardPrefix} {rewardCoins} {rewardSuffix}";
        totalCoinsText.text = $"Total Coins: {currentCoins}";

        // Show panel
        rewardPanel.SetActive(true);
    }

    //Adding Buttons of Reward Panel
    public void OnClickViewLeaderboardFromReward()
    {
        // Hide reward panel
        if (rewardPanel != null)
            rewardPanel.SetActive(false);

        // Show leaderboard using saved rank
        int rank = PlayerPrefs.GetInt(Menu.LeaderboardRank, 0);
        ShowLeaderboardUI(rank);
    }
    public void OnClickContinueFromReward()
    {
        // Reset time
        Time.timeScale = 1f;

        // Clean up reward panel
        if (rewardPanel != null)
            rewardPanel.SetActive(false);

        // Go back to Main Menu
        PlayerPrefs.SetInt(Menu.GotoLevelSelection, 1);
        //SceneManager.LoadScene(0);
        SceneTransition.Instance.LoadSceneSlide(0, true); //Transition 2

    }

    public void DamageCar()
    {
        currentHealth -= 10;
        currentHealth = Mathf.Clamp(currentHealth, 0, 100);
        currentScore = Mathf.Max(0, currentScore - 5);
        UpdateScoreUI();

        if (currentHealth <= 0)
        {
            ShowGameOver();
            return;
        }

        Vector3 offset = player.transform.forward * -3f;
        player.transform.position += offset;

        Controller controller = player.GetComponent<Controller>();
        if (controller != null)
            controller.ApplyStop();
    }

    public void HealCar()
    {
        if (currentHealth < 100)
        {
            currentHealth += 10;
            currentHealth = Mathf.Clamp(currentHealth, 0, 100);
            UpdateScoreUI();
        }
    }

    #endregion

    #region GAME STATES / UI

    public void ShowGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        Controller controller = player?.GetComponent<Controller>();
        if (controller != null)
            controller.OnGameOver();

        GameOverPanel.SetActive(true);
        Time.timeScale = 0f;
        if (playerControlsUI != null)
            playerControlsUI.SetActive(false);

    }

    public void ShowLevelComplete()
    {
        if (isGameOver) return;
        isGameOver = true;

        Controller controller = player?.GetComponent<Controller>();
        if (controller != null)
            controller.OnGameOver();

        //LevelCompletePanel.SetActive(true);
        // ShowLeaderboard(PlayerPrefs.GetInt(Menu.LeaderboardRank));
        //LeaderboardPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    //showing pause menu
    private void ShowPausePanel()
    {
        if (PausePanel == null) return;

        PausePanel.SetActive(true);

        CanvasGroup cg = PausePanel.GetComponent<CanvasGroup>();
        RectTransform rt = PausePanel.GetComponent<RectTransform>();

        if (cg == null || rt == null) return;

        // Kill previous tweens
        DOTween.Kill(cg);
        DOTween.Kill(rt);

        // Initial state
        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;

        rt.localScale = Vector3.one * 0.75f;

        // Fade in (unscaled time)
        cg.DOFade(1f, 0.25f)
          .SetUpdate(true)
          .OnComplete(() =>
          {
              cg.interactable = true;
              cg.blocksRaycasts = true;
          });

        // Pop effect
        rt.DOScale(1f, 0.4f)
          .SetEase(Ease.OutBack)
          .SetUpdate(true);
    }

    //hiding pause menu
    private void HidePausePanel()
    {
        if (PausePanel == null) return;

        CanvasGroup cg = PausePanel.GetComponent<CanvasGroup>();
        RectTransform rt = PausePanel.GetComponent<RectTransform>();

        if (cg == null || rt == null) return;

        DOTween.Kill(cg);
        DOTween.Kill(rt);

        cg.interactable = false;
        cg.blocksRaycasts = false;

        cg.DOFade(0f, 0.2f)
          .SetUpdate(true);

        rt.DOScale(0.85f, 0.2f)
          .SetEase(Ease.InBack)
          .SetUpdate(true)
          .OnComplete(() =>
          {
              PausePanel.SetActive(false);
          });
    }
    public void OnClickPauseBtn()
    {
        PauseBtn.SetActive(false);

        player?.GetComponent<Controller>()?.OnPause();

        Time.timeScale = 0f;

        if (playerControlsUI != null)
            playerControlsUI.SetActive(false);

        ShowPausePanel(); // ✅ animate ROOT
    }
    public void OnClickResumeBtn()
    {
        HidePausePanel(); // ✅ animate ROOT

        PauseBtn.SetActive(true);
        Time.timeScale = 1f;

        player?.GetComponent<Controller>()?.OnResume();

        ShowControlsUI();
    }

    /* public void OnClickPauseBtn()
     {
        PausePanel.SetActive(true);
         PauseBtn.SetActive(false);
         player?.GetComponent<Controller>()?.OnPause();
         Time.timeScale = 0f;
         if (playerControlsUI != null)
             playerControlsUI.SetActive(false);

     }

     public void OnClickResumeBtn()
     {

       PausePanel.SetActive(false);
         PauseBtn.SetActive(true);
         Time.timeScale = 1f;
         player?.GetComponent<Controller>()?.OnResume();
         if (playerControlsUI != null)
             playerControlsUI.SetActive(true);
     }*/

    public void OnClickRestartBtn()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitBtn()
    {
        Time.timeScale = 1f;

        PlayerPrefs.SetInt(Menu.GotoHome, 1);
        //  SceneManager.LoadSceneAsync(0);
        SceneTransition.Instance.LoadSceneSlide(0, true); //Transition 2

    }

    public void OnClickNextLevelBtn()
    {
        Time.timeScale = 1f;
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            PlayerPrefs.SetInt(Menu.ShowLeaderBoard, 1);
            SceneManager.LoadScene(nextSceneIndex);
        }
    }
    public void TargetDestroyed()
    {
        totalTargets--;
    }

    public void CollectCoin()
    {
        AddCoins(5); // Coins per target
    }
    //recording win/loss
    public void RecordRaceResult(int rank)
    {
        // Top 3 = WIN
        if (rank <= 2)
        {
            int wins = PlayerPrefs.GetInt(RACES_WON_KEY, 0);
            PlayerPrefs.SetInt(RACES_WON_KEY, wins + 1);
        }
        else
        {
            int losses = PlayerPrefs.GetInt(RACES_LOST_KEY, 0);
            PlayerPrefs.SetInt(RACES_LOST_KEY, losses + 1);
        }

        PlayerPrefs.Save();
    }

    #endregion

    #region LEADERBOARD

    #region PLAYER PROFILE (FROM MENU)

    string playerName;
    int avatarIndex;
    bool isGuest;

    void LoadPlayerProfile()
    {
        playerName = PlayerPrefs.GetString(Menu.nameStr, "Guest");
        avatarIndex = PlayerPrefs.GetInt(Menu.PlayerAvatarKey, 0);
        isGuest = PlayerPrefs.GetInt(Menu.IsGuestKey, 0) == 1;
    }

    #endregion
    public void ShowLeaderboard(int rank)
    {
        LoadPlayerProfile();

        List<string> aiNames = new List<string>
    {
        "Liam","Rado","Kenny","William","Rachel","Joey","Rocky"
    };

        string playerName = PlayerPrefs.GetString(Menu.nameStr, "Guest");
        int avatarIndex = PlayerPrefs.GetInt(Menu.PlayerAvatarKey, 0);
        bool isGuest = PlayerPrefs.GetInt(Menu.IsGuestKey, 0) == 1;

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
                // PLAYER ROW
                nameText.text = playerName;

                avatarImage.sprite = isGuest
                    ? guestAvatar
                    : avatarSprites[Mathf.Clamp(avatarIndex, 0, avatarSprites.Length - 1)];
            }
            else
            {
                // AI ROW
                int rnd = Random.Range(0, aiNames.Count);
                nameText.text = aiNames[rnd];
                aiNames.RemoveAt(rnd);

                avatarImage.sprite =
                    avatarSprites[Random.Range(0, avatarSprites.Length)];
            }
        }
    }





    // adding a new method 
    public string GetPlayerName()
    {
        string name = PlayerPrefs.GetString(Menu.nameStr, "");
        if (string.IsNullOrEmpty(name))
        {
            // name = "Player";

            name = "username" + Random.Range(100000, 999999);
            PlayerPrefs.SetString(Menu.nameStr, name);
        }
        return name;
    }
    public void ShowLeaderboardUI(int rank)
    {
        Debug.Log("[GAMEMANAGER] Showing leaderboard with rank = " + rank);

        LeaderboardPanel.SetActive(true);
        ShowLeaderboard(rank);
        Time.timeScale = 0f;
    }

    public void OnLeaderboardContinue()
    {
        Time.timeScale = 1f;
        LeaderboardPanel.SetActive(false);
        ShowLevelComplete();
    }

    #endregion

    #region UTILITIES

    /*  public void LockAllCars(bool lockMovement)
      {
          if (player != null)
          {
              Controller controller = player.GetComponent<Controller>();
              if (controller != null)
                  controller.enabled = !lockMovement;
          }

          RCC_AICarController[] aiCars = FindObjectsOfType<RCC_AICarController>();
          foreach (var ai in aiCars)
              ai.enabled = !lockMovement;
      }*/
    public void LockAllCars(bool lockMovement)
    {
        // PLAYER
        if (player != null)
        {
            Controller controller = player.GetComponent<Controller>();
            if (controller != null)
                controller.enabled = !lockMovement;
        }

        // AI CARS
        RCC_AICarController[] aiCars = FindObjectsOfType<RCC_AICarController>();

        foreach (var ai in aiCars)
        {
            ai.enabled = !lockMovement;

            RCC_CarControllerV3 car = ai.GetComponent<RCC_CarControllerV3>();
            if (car != null)
            {
                if (lockMovement)
                {
                    // FULL STOP
                    car.throttleInput = 0f;
                    car.brakeInput = 1f;
                    car.handbrakeInput = 1f;

                    Rigidbody rb = car.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.linearVelocity = Vector3.zero;
                        rb.angularVelocity = Vector3.zero;
                    }
                }
                else
                {
                    // RELEASE BRAKES
                    car.brakeInput = 0f;
                    car.handbrakeInput = 0f;
                }
            }
        }
    }


    private void AnimateCountdownImage()
    {
        RectTransform rect = countdownImage.rectTransform;
        rect.localScale = Vector3.zero;

        rect.DOScale(1.2f, 0.25f)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                rect.DOScale(1f, 0.15f);
            });
    }

    #endregion
}