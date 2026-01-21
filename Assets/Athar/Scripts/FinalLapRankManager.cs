using System.Collections.Generic;
using UnityEngine;

public class FinalLapRankManager : MonoBehaviour
{
    public static FinalLapRankManager Instance;
    private HashSet<GameObject> eligibleCars = new HashSet<GameObject>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // =========================
    // SETTINGS
    // =========================
    [Header("Checkpoint Settings")]
    [SerializeField] private int totalCheckpoints = 3;

    // =========================
    // STATE
    // =========================
    private bool finalLapActive = false;

    // Finish order (0 = 1st place)
    private List<GameObject> finishOrder = new List<GameObject>();

    // ?? STORED RANKS
    private Dictionary<GameObject, int> carRanks =
        new Dictionary<GameObject, int>();

    // =========================
    // FINAL LAP CONTROL
    // =========================
    /* public void StartFinalLap(List<GameObject> allCars)
     {
         finalLapActive = true;
         finishOrder.Clear();
         carRanks.Clear();
         eligibleCars.Clear();

         foreach (GameObject car in allCars)
         {
             Checkpoint.ResetCar(car);
             eligibleCars.Add(car); // ? only these cars can rank

         }

         Debug.Log("?? FINAL LAP STARTED — Rank tracking enabled");
     }*/
    public void StartFinalLap(List<GameObject> allCars)
    {
        finalLapActive = true;
        finishOrder.Clear();
        carRanks.Clear();
        eligibleCars.Clear();

        // ?? AUTO-CALCULATE CHECKPOINT COUNT
        //totalCheckpoints = FindObjectsOfType<Checkpoint>().Length;

        foreach (GameObject car in allCars)
        {
           // Checkpoint.ResetCar(car);
            eligibleCars.Add(car);
        }

        Debug.Log($"FINAL LAP STARTED — Total checkpoints = {totalCheckpoints}");
    }

    // =========================
    // FINISH REGISTRATION
    // =========================
    public void TryRegisterFinish(GameObject car)
    {
        if (!finalLapActive)
            return;

        if (!eligibleCars.Contains(car))
            return;


        if (carRanks.ContainsKey(car))
            return;
        //Debug.Log($"?? {car.name} CP COUNT = {Checkpoint.GetCheckpointCount(car)}");

       /* if (!Checkpoint.HasCrossedAll(car, totalCheckpoints))
        {
            Debug.LogWarning($"? {car.name} missing checkpoints in final lap");
            return;
        }*/

        /* finishOrder.Add(car);

         int rank = finishOrder.Count - 1; // 0-based rank
         carRanks[car] = rank;*/
        int rank = finishOrder.Count;
        finishOrder.Add(car);
        carRanks[car] = rank;
        Debug.Log($"?? {car.name} FINISHED — Rank {rank + 1}");
    }

    // =========================
    // RANK ACCESS
    // =========================
    public bool HasFinished(GameObject car)
    {
        return carRanks.ContainsKey(car);
    }

    public int GetRank(GameObject car)
    {
        if (!carRanks.ContainsKey(car))
            return -1;

        return carRanks[car];
    }

    public List<GameObject> GetFinishOrder()
    {
        return new List<GameObject>(finishOrder);
    }
    public void DebugPrintAllRanks()
    {
        Debug.Log("===== FINAL RANKS =====");
        foreach (var kvp in carRanks)
        {
            Debug.Log($"{kvp.Key.name} => Rank {kvp.Value}");
        }
    }

}
