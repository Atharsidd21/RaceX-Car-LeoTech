using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;
    [Header("Control UI")]
    private GameObject playerControlsUI;
    [SerializeField] private float controlsUIDelay = 0.4f;
    [SerializeField] float FadeDuration = 0.25f;
    [SerializeField] float PopUpValue = 40f;
    [SerializeField] float PopUpDistance = 0.35f;
    [SerializeField] private Ease PopEase = Ease.OutBack;

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
    public List<GameObject> playerListLeaderboard;

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
        coinScore = PlayerPrefs.GetInt(CurrencyKey, 0);
        username = PlayerPrefs.GetString(Menu.nameStr);

        // Lock player & AI immediately
        LockAllCars(true);

        // Start countdown directly
        currentState = GameState.Countdown;
        StartRaceAfterCutscene(); // reuse existing countdown coroutine

        // Setup RCC camera for cinematic countdown
        StartCoroutine(SetupRCCCameraForCountdown());
    }



    private void Update()
    {
        if (player == null || isGameOver)
            return;

        HandleDistanceScore();

        if (!raceStarted)
            return;

        HandleTimer();
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



    public void ShowControlsUI()

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
    }



    public void StartRaceAfterCutscene()
    {
        StartCoroutine(RaceStartCountdown());
    }

    private IEnumerator RaceStartCountdown()
    {
        // Ensure cars stay locked during countdown
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
        yield return new WaitForSeconds(countdownDelay);

        countdownImage.sprite = spriteGo;
        AnimateCountdownImage();
        yield return new WaitForSeconds(0.6f);

        countdownImage.gameObject.SetActive(false);

        // START RACE
        LockAllCars(false);
        raceStarted = true;
        currentState = GameState.Playing;
        // Switch RCC camera to TPS on race start
        if (rccCamera != null)
        {
            rccCamera.ChangeCamera(RCC_Camera.CameraMode.TPS);
        }


        // Adding a small delay befroe showing controls UI
        if (playerControlsUI != null)
            playerControlsUI.SetActive(true);

    }

    #endregion

    #region PLAYER ASSIGNMENT

    public void AssignPlayer(GameObject car)
    {
        player = car;
        lastPosition = player.transform.position;
        // Find controls UI inside spawned car

        Transform controls = player.transform.Find("ControlsUI");

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
        }

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

        LevelCompletePanel.SetActive(true);
        LeaderboardPanel.SetActive(false);
    }

    public void OnClickPauseBtn()
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
    }

    public void OnClickRestartBtn()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitBtn()
    {
        PlayerPrefs.SetInt(Menu.GotoHome, 1);
        SceneManager.LoadSceneAsync(0);
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


    #endregion

    #region LEADERBOARD

    public void ShowLeaderboard(int rank)
    {
        List<string> names = new List<string>(Menu.playerName);

        for (int i = 0; i < 5; i++)
        {
            if (i == rank)
            {
              
                playerListLeaderboard[i].transform.GetChild(2).GetChild(0).GetComponent<Text>().text = username;
            }
            else
            {
                int ind = Random.Range(0, names.Count);
                playerListLeaderboard[i].transform.GetChild(2).GetChild(0).GetComponent<Text>().text = names[ind];
                names.RemoveAt(ind);
            }
        }
    }
    // adding a new method 
    public string GetPlayerName()
    {
        string name = PlayerPrefs.GetString(Menu.nameStr,"");
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

    public void LockAllCars(bool lockMovement)
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