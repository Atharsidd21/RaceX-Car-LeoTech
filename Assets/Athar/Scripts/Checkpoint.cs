using UnityEngine;
using System.Collections.Generic;

public class Checkpoint : MonoBehaviour
{
    [Tooltip("Unique index for this checkpoint (0, 1, 2...)")]
    public int checkpointIndex;

    // Tracks checkpoints crossed per car in FINAL LAP
    private static Dictionary<GameObject, HashSet<int>> checkpointTracker
        = new Dictionary<GameObject, HashSet<int>>();

    /// <summary>
    /// Reset checkpoint progress for a specific car (used when final lap starts)
    /// </summary>
    public static void ResetCar(GameObject car)
    {
        if (checkpointTracker.ContainsKey(car))
        {
            checkpointTracker[car].Clear();
            Debug.Log($"?? Checkpoint progress reset for {car.name}");
        }
    }

    /// <summary>
    /// Check if a car has crossed all required checkpoints
    /// </summary>
    public static bool HasCrossedAll(GameObject car, int totalCheckpoints)
    {
        if (!checkpointTracker.ContainsKey(car))
        {
            Debug.Log($"? {car.name} has NOT crossed any checkpoints yet (0/{totalCheckpoints})");
            return false;
        }

        int crossed = checkpointTracker[car].Count;
        bool hasAll = crossed >= totalCheckpoints;

        Debug.Log($"{(hasAll ? "?" : "?")} {car.name} checkpoints: {crossed}/{totalCheckpoints}");

        return hasAll;
    }

    /// <summary>
    /// Get current checkpoint count for a car (for debugging)
    /// </summary>
    public static int GetCheckpointCount(GameObject car)
    {
        if (!checkpointTracker.ContainsKey(car))
            return 0;

        return checkpointTracker[car].Count;
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject car = other.transform.root.gameObject;

        if (!car.CompareTag("Player") && !car.CompareTag("AI"))
            return;

        // Initialize tracker for this car if needed
        if (!checkpointTracker.ContainsKey(car))
        {
            checkpointTracker[car] = new HashSet<int>();
        }

        // Check if this checkpoint was already crossed
        bool isNewCheckpoint = checkpointTracker[car].Add(checkpointIndex);

        if (isNewCheckpoint)
        {
            int totalCrossed = checkpointTracker[car].Count;
            Debug.Log($"? {car.name} crossed checkpoint {checkpointIndex} ({totalCrossed} total)");
        }
        else
        {
            Debug.Log($"?? {car.name} crossed checkpoint {checkpointIndex} again (already counted)");
        }

    }
}