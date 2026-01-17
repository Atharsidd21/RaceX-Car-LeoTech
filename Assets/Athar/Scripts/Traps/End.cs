using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class End : MonoBehaviour
{
    [Header("Lap System")]
    public int totalLaps = 3;
    private int playerLapCount = 0;
    public TextMeshProUGUI lapText;
    [SerializeField] private int totalCheckpoints = 3;

    [Header("Lap Safety")]
    [SerializeField] private float lapTriggerCooldown = 2f;
    private Dictionary<GameObject, float> lastLapTriggerTime = new Dictionary<GameObject, float>();

    [Header("Cars")]
    public List<GameObject> raceCar = new List<GameObject>();
    public int totalCars;

    private bool finalLapStarted = false;
    private bool playerFinished = false;

    // ? FINAL LAP: Track finish order (only cars that completed all checkpoints)
    private List<GameObject> finalLapFinishOrder = new List<GameObject>();

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

    private void UpdateLapUI()
    {
        if (lapText != null)
            lapText.text = $"{Mathf.Min(playerLapCount + 1, totalLaps)} / {totalLaps}";
    }

    // =========================
    // FINISH LINE TRIGGER
    // =========================
    private void OnTriggerEnter(Collider other)
    {
        GameObject car = other.transform.root.gameObject;

        if (!car.CompareTag("Player") && !car.CompareTag("AI"))
            return;

        // ? Cooldown check per car
        if (lastLapTriggerTime.ContainsKey(car) &&
            Time.time - lastLapTriggerTime[car] < lapTriggerCooldown)
            return;

        lastLapTriggerTime[car] = Time.time;

        // ================= PLAYER =================
        if (car.CompareTag("Player"))
        {
            HandlePlayerCrossing(car);
        }
        // ================= AI =================
        else if (car.CompareTag("AI"))
        {
            HandleAICrossing(car);
        }
    }

    // =========================
    // PLAYER CROSSING LOGIC
    // =========================
    private void HandlePlayerCrossing(GameObject player)
    {
        if (playerFinished)
            return;

        playerLapCount++;
        Debug.Log($"?? Player completed lap {playerLapCount}/{totalLaps}");

        // Start final lap
        if (playerLapCount == totalLaps - 1)
        {
            StartFinalLap();
        }

        // Normal laps (not final lap yet)
        if (playerLapCount < totalLaps)
        {
            UpdateLapUI();
            return;
        }

        // ? FINAL LAP FINISH ATTEMPT
        if (playerLapCount == totalLaps)
        {
            // Check if player crossed all checkpoints
            bool hasAllCheckpoints = Checkpoint.HasCrossedAll(player, totalCheckpoints);

            if (hasAllCheckpoints)
            {
                // ? Valid finish!
                playerFinished = true;

                if (!finalLapFinishOrder.Contains(player))
                {
                    finalLapFinishOrder.Add(player);
                    int rank = finalLapFinishOrder.Count - 1; // 0-based rank

                    Debug.Log($"?? PLAYER FINISHED!");
                    Debug.Log($"? All checkpoints completed: YES");
                    Debug.Log($"?? Final Rank: {rank} (Position {rank + 1})");
                    Debug.Log($"?? Finish Order: {string.Join(", ", finalLapFinishOrder.ConvertAll(c => c.tag))}");

                    // Save rank and reward player
                    PlayerPrefs.SetInt(Menu.LeaderboardRank, rank);
                    PlayerPrefs.Save();

                    GameManager.Instance.RecordRaceResult(rank);
                    GameManager.Instance.RewardPlayerByRank(rank);

                    player.GetComponent<Controller>()?.OnGameOver();
                }
            }
            else
            {
                // ? Invalid finish - missing checkpoints
                Debug.LogWarning("?? PLAYER crossed finish line but MISSING CHECKPOINTS!");
                Debug.LogWarning("Player must complete all checkpoints before finishing!");

                // Don't count this lap, reset to continue racing
                playerLapCount--;
                UpdateLapUI();
            }
        }
    }

    // =========================
    // AI CROSSING LOGIC
    // =========================
    private void HandleAICrossing(GameObject aiCar)
    {
        // Only track AI finishes during final lap
        if (!finalLapStarted)
            return;

        // Check if already finished
        if (finalLapFinishOrder.Contains(aiCar))
            return;

        // ? Check if AI completed all checkpoints
        bool hasAllCheckpoints = Checkpoint.HasCrossedAll(aiCar, totalCheckpoints);

        if (hasAllCheckpoints)
        {
            // ? Valid finish!
            finalLapFinishOrder.Add(aiCar);
            int position = finalLapFinishOrder.Count;

            Debug.Log($"?? AI '{aiCar.name}' FINISHED in position {position}!");
            Debug.Log($"? All checkpoints completed: YES");

            // Disable AI
            RCC_AICarController ai = aiCar.GetComponent<RCC_AICarController>();
            if (ai != null)
            {
                ai.enabled = false;
            }
        }
        else
        {
            Debug.LogWarning($"?? AI '{aiCar.name}' crossed finish but MISSING CHECKPOINTS!");
        }
    }

    // =========================
    // START FINAL LAP
    // =========================
    private void StartFinalLap()
    {
        finalLapStarted = true;
        finalLapFinishOrder.Clear();

        // ? Reset all checkpoint trackers for final lap
        foreach (GameObject car in raceCar)
        {
            Checkpoint.ResetCar(car);
        }

        Debug.Log("?????? FINAL LAP STARTED! ??????");
        Debug.Log("All checkpoint progress reset - must cross all checkpoints to finish!");

        UpdateLapUI();
    }
}