using System.Collections.Generic;
using UnityEngine;

public class AICarSpawner : MonoBehaviour
{
    [Header("AI Car Prefabs")]
    public GameObject[] aiCarPrefabs;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Player Vehicle List")]
    public VehicleLiist vehicleList; // SAME list used for player selection

    void Start()
    {

        //  MULTIPLAYER - NO AI CARS
        if (GameModeManager.IsMultiplayer)
        {
            Debug.Log("AI Spawner disabled (Multiplayer Mode)");
            return;
        }

        //  SINGLE PLAYER - NORMAL AI SPAWN
        SpawnUniqueAICarsExcludingPlayer();
        
    }

    void SpawnUniqueAICarsExcludingPlayer()
    {
   
        if (aiCarPrefabs.Length == 0 || spawnPoints.Length == 0)
        {
            Debug.LogWarning("AI Spawner: Missing prefabs or spawn points");
            return;
        }

        // ?? Get player selected car prefab
        int playerIndex = PlayerPrefs.GetInt("Pointer", 0);
        GameObject playerCarPrefab = vehicleList.Vehicals[playerIndex];

        // ?? Build available AI list excluding player car
        List<GameObject> availablePrefabs = new List<GameObject>();
        CarController playerCar =  playerCarPrefab.GetComponent<CarController>();

        foreach (GameObject prefab in aiCarPrefabs)
        {
            CarController aiCar = prefab.GetComponent<CarController>();

            if (aiCar == null || playerCar == null)
                continue;

            // Compare by CarName (NOT prefab reference)
            if (aiCar.CarName != playerCar.CarName)
            {
                availablePrefabs.Add(prefab);
            }
        }

        if (availablePrefabs.Count == 0)
        {
            Debug.LogError("AI Spawner: No AI cars left after excluding player car!");
            return;
        }

        int spawnCount = Mathf.Min(spawnPoints.Length, availablePrefabs.Count);

        for (int i = 0; i < spawnCount; i++)
        {
            int randomIndex = Random.Range(0, availablePrefabs.Count);
            GameObject selectedPrefab = availablePrefabs[randomIndex];

            GameObject ai = Instantiate(
                selectedPrefab,
                spawnPoints[i].position,
                spawnPoints[i].rotation
            );

            ai.tag = "AI"; // REQUIRED by End.cs
                           // ? REGISTER AI CAR FOR RANKING
            RaceProgressTracker tracker = ai.GetComponent<RaceProgressTracker>();

            if (tracker == null)
            {
                tracker = ai.AddComponent<RaceProgressTracker>();
            }

            RaceRankManager.Instance.RegisterCar(tracker);


            if (ai.GetComponent<RCC_AICarController>() == null)
            {
                Debug.LogError($"{selectedPrefab.name} is missing AICarController!");
            }

            // ? Remove so it can't spawn again
            availablePrefabs.RemoveAt(randomIndex);
        }


    }
}