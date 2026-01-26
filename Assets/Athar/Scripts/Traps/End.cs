using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class End : MonoBehaviour
{
    [Header("Lap Settings")]
    public int totalLaps = 3;
    public TextMeshProUGUI lapText;

    [Header("Lap Safety")]
    [SerializeField] private float lapTriggerCooldown = 2f;
    private float lastLapTime = -10f;

    private bool raceFinished = false;
    private bool raceStarted = false;

    private void Start()
    {
        UpdateLapUI(0);
    }

    // =========================
    // FINISH LINE TRIGGER
    // =========================
    /* private void OnTriggerEnter(Collider other)
     {
         if (raceFinished) return;

         GameObject car = other.transform.root.gameObject;

         if (!car.CompareTag("Player") && !car.CompareTag("AI"))
             return;

         // Prevent double-trigger
         if (Time.time - lastLapTime < lapTriggerCooldown)
             return;

         lastLapTime = Time.time;

         RaceProgressTracker tracker = car.GetComponent<RaceProgressTracker>();
         if (tracker == null || tracker.finished)
             return;

         // ? Increment lap for BOTH Player & AI
         tracker.currentLap++;

         Debug.Log($"{car.name} completed lap {tracker.currentLap}");

         // =========================
         // PLAYER-ONLY LOGIC
         // =========================
         if (car.CompareTag("Player"))
         {
             UpdateLapUI(tracker.currentLap);

             if (tracker.currentLap >= totalLaps)
             {
                 FinishPlayer(tracker);
             }
         }
     }*/
    private void OnTriggerEnter(Collider other)
    {
        if (raceFinished) return;

        GameObject car = other.transform.root.gameObject;

        if (!car.CompareTag("Player") && !car.CompareTag("AI"))
            return;

        // Cooldown to prevent double trigger
        if (Time.time - lastLapTime < lapTriggerCooldown)
            return;

        lastLapTime = Time.time;

        RaceProgressTracker tracker = car.GetComponent<RaceProgressTracker>();
        if (tracker == null || tracker.finished)
            return;

        // ?? IGNORE FIRST CROSSING (RACE START)
        if (!raceStarted)
        {
            raceStarted = true;
            Debug.Log("?? Race started — ignoring first finish line crossing");
            return;
        }

        // ? VALID LAP COMPLETION
        tracker.currentLap++;

        Debug.Log($"{car.name} completed lap {tracker.currentLap}");

        // PLAYER LOGIC
        if (car.CompareTag("Player"))
        {
            UpdateLapUI(tracker.currentLap);

            if (tracker.currentLap >= totalLaps)
            {
                FinishPlayer(tracker);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        OnTriggerEnter(other);
    }

    // =========================
    // FINISH PLAYER
    // =========================
    private void FinishPlayer(RaceProgressTracker tracker)
    {
        if (raceFinished) return;
        raceFinished = true;

        tracker.finished = true;

        int rank = RaceRankManager.Instance.GetRank(tracker);

        Debug.Log("?? PLAYER FINISHED WITH RANK: " + (rank + 1));

        PlayerPrefs.SetInt(Menu.LeaderboardRank, rank);
        PlayerPrefs.Save();

        GameManager.Instance.RecordRaceResult(rank);
        GameManager.Instance.RewardPlayerByRank(rank);
        UnlockNextLevel(rank);
    }

    // =========================
    // LAP UI (ORIGINAL LOGIC)
    // =========================
    private void UpdateLapUI(int completedLaps)
    {
        if (lapText == null) return;

        int displayLap = Mathf.Min(completedLaps + 1, totalLaps);
        lapText.text = $"{displayLap} / {totalLaps}";
    }

    // =========================
    // LEVEL UNLOCK
    // =========================
    private void UnlockNextLevel(int rank)
    {
        if (rank <= 2)
        {
            int currentLevel = SceneManager.GetActiveScene().buildIndex;
            int nextLevel = currentLevel + 1;

            PlayerPrefs.SetInt("LevelOpened_" + nextLevel, 1);
            PlayerPrefs.Save();

            Debug.Log($"?? Level {nextLevel} unlocked");
        }
    }
}
