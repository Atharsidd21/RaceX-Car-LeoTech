using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class End : MonoBehaviour
{
    // =========================
    // LAP SYSTEM
    // =========================
    [Header("Lap System")]
    public int totalLaps = 3;
    private int playerLapCount = 0;
    public TextMeshProUGUI lapText;
    private bool finalLapStarted = false;


    [Header("Lap Safety")]
    [SerializeField] private float lapTriggerCooldown = 2f;
    private Dictionary<GameObject, float> lastLapTriggerTime =
        new Dictionary<GameObject, float>();

    // =========================
    // CARS
    // =========================
    [Header("Cars")]
    public List<GameObject> raceCar = new List<GameObject>();
    public int totalCars;

    // =========================
    // UNITY
    // =========================
    private void Start()
    {
        StartCoroutine(SetupRaceCars());
        UpdateLapUI();
        
    }

    private IEnumerator SetupRaceCars()
    {
        yield return new WaitForEndOfFrame();

        if (raceCar.Count == 0)
        {
            GameObject[] aiCars = GameObject.FindGameObjectsWithTag("AI");
            raceCar.AddRange(aiCars);
        }

        raceCar.Add(CarSpawn.instance.owncar);
        totalCars = raceCar.Count;

        Debug.Log($"?? Total cars in race: {totalCars}");
        

    }

    // =========================
    // UI
    // =========================
    private void UpdateLapUI()
    {
        if (lapText != null)
        {
            lapText.text = $"{Mathf.Min(playerLapCount + 1, totalLaps)} / {totalLaps}";
        }
    }

    // =========================
    // FINISH LINE TRIGGER
    // =========================
    private void OnTriggerEnter(Collider other)
    {
        GameObject car = other.transform.root.gameObject;

        if (!car.CompareTag("Player") && !car.CompareTag("AI"))
            return;

        // Cooldown per car
        if (lastLapTriggerTime.ContainsKey(car) &&
            Time.time - lastLapTriggerTime[car] < lapTriggerCooldown)
            return;

        lastLapTriggerTime[car] = Time.time;

        if (car.CompareTag("Player"))
        {
            HandlePlayerCrossing(car);
        }
        else
        {
            HandleAICrossing(car);
        }
    }

    // =========================
    // PLAYER CROSSING
    // =========================
    private void HandlePlayerCrossing(GameObject player)
    {
        playerLapCount++;

        Debug.Log($"?? Player lap {playerLapCount}/{totalLaps}");
        
        if (!finalLapStarted && playerLapCount == totalLaps - 1)
        {
            finalLapStarted = true;

            Debug.Log("?? END.CS FINAL LAP STARTED");

            FinalLapRankManager.Instance.StartFinalLap(raceCar);
        }
        // ?? FINAL LAP START
        /* if (playerLapCount == totalLaps - 1)
         {
             FinalLapRankManager.Instance.StartFinalLap(raceCar);
         }*/


        // NORMAL LAPS
        if (playerLapCount < totalLaps)
        {
            UpdateLapUI();
            return;
        }

        // =========================
        // FINAL LAP FINISH
        // =========================
        FinalLapRankManager.Instance.TryRegisterFinish(player);

        if (!FinalLapRankManager.Instance.HasFinished(player))
        {
            playerLapCount--;
            UpdateLapUI();
            return;
        }

        int rank = FinalLapRankManager.Instance.GetRank(player);


        Debug.Log("?? END.CS RECEIVED STORED RANK: " + rank);

        UnlockNextLevel(rank);

        PlayerPrefs.SetInt(Menu.LeaderboardRank, rank);
        PlayerPrefs.Save();

        GameManager.Instance.RecordRaceResult(rank);
        GameManager.Instance.RewardPlayerByRank(rank);

    }

    // =========================
    // AI CROSSING
    // =========================
    private void HandleAICrossing(GameObject aiCar)
    {
        // AI rank is handled entirely by RankManager
        FinalLapRankManager.Instance.TryRegisterFinish(aiCar);
        Debug.Log("?? AI HIT FINISH: " + aiCar.name);

    }

    // =========================
    // LEVEL UNLOCK LOGIC
    // =========================
    private void UnlockNextLevel(int rank)
    {
        // Example rule: Top 3 unlock next level
        if (rank <= 2)
        {
            int currentLevel = SceneManager.GetActiveScene().buildIndex;
            int nextLevel = currentLevel + 1;

            PlayerPrefs.SetInt("LevelOpened_" + nextLevel, 1);
            PlayerPrefs.Save();

            Debug.Log($"?? Level {nextLevel} unlocked!");
        }
        else
        {
            Debug.Log("? Level not unlocked (rank too low)");
        }
    }
}
