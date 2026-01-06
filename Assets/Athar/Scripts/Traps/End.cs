using NUnit.Framework;
using TMPro;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class End : MonoBehaviour
{
    [Header("Lap Syatem")]
    public int totalLaps = 3;
    private int playerLapCount = 0;
    public TextMeshProUGUI lapText;
    private bool raceFinished = false;

    [Header("Lap Safety")]
    [SerializeField] private float lapTriggerCooldown = 2f;
    private float lastLapTime = -10f;



    public GameObject gameOver;
    public Transform player;
    public Transform ai;
    public Transform finishLine;

    private List<string> finishOrder = new List<string>();
    public List<GameObject> raceCar = new List<GameObject>();

    public int totalCars;

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
            GameObject[] cars = GameObject.FindGameObjectsWithTag("AI");
            raceCar.AddRange(cars);
        }

        raceCar.Add(CarSpawn.instance.owncar);


        totalCars = raceCar.Count;
    }

    // Update the lap UI 
    void UpdateLapUI()
    {
        if (lapText != null)
            lapText.text = $"LAP {playerLapCount + 1} / {totalLaps}";
    }



    private void OnTriggerEnter(Collider other)
    {
        GameObject gm = other.transform.root.gameObject;
        //Debug.Log("Car Trigger" + gm.name + " " + gameObject.name);

        if (gm.CompareTag("Player") || (gm.CompareTag("AI") && !finishOrder.Contains(gm.name)))

        {
            // ? Prevent double trigger
            if (Time.time - lastLapTime < lapTriggerCooldown)
                return;

            lastLapTime = Time.time;

            finishOrder.Add(gm.name);
            int position = finishOrder.Count;

            if (gm.CompareTag("Player"))
            {
                if (raceFinished)
                    return;

                playerLapCount++;
                UpdateLapUI();

                // NOT final lap ? just continue race
                if (playerLapCount < totalLaps)
                {
                    Debug.Log($"Lap {playerLapCount}/{totalLaps}");
                    return;
                }

                // FINAL LAP ? original race finish logic
                raceFinished = true;
                if (!finishOrder.Contains(gm.name))
                    finishOrder.Add(gm.name);

                int rank = finishOrder.IndexOf(gm.name);
                Debug.Log("PLAYER FINISH RANK = " + rank);

                PlayerPrefs.SetInt(Menu.LeaderboardRank, rank);
               // PlayerPrefs.SetInt(Menu.ShowLeaderBoard, 1);
                PlayerPrefs.Save();

                GameManager.Instance.RewardPlayerByRank(rank);

               // GameManager.Instance.ShowLeaderboardUI(rank);
                CarSpawn.instance.owncar.GetComponent<Controller>().OnGameOver();
            }

            //Debug.Log("Car Trigger ++++++++++" + gm.name);
            if (gm.CompareTag("AI"))
            {
                EndcarMovement(gm.gameObject);
            }
            else
            {
                CarSpawn.instance.owncar.GetComponent<Controller>().MaxSpeed = 1;
            }
            if (position == totalCars)
            {
                CoResult();
            }

        }
    }


    private void EndcarMovement(GameObject gm)
    {
        gm.GetComponent<AICarController>().enabled = false;
    }

    private void CoResult()
    {
        GameManager.Instance.ShowLevelComplete();
    }

    private bool gameEnded = false;

    // Optional UI text
    public Text resultText;

    void Update()
    {
        //if (gameEnded) return;

        //float playerDist = Vector3.Distance(player.position, finishLine.position);
        //float aiDist = Vector3.Distance(ai.position, finishLine.position);

        //// You can tweak this threshold as needed
        //float winThreshold = 1.0f;

        //if (playerDist < winThreshold)
        //{
        //    gameEnded = true;
        //    Win();
        //}
        //else if (aiDist < winThreshold)
        //{
        //    gameEnded = true;
        //    Lose();
        //}
    }

    void Win()
    {
        GameManager.Instance.ShowLevelComplete();
    }

    void Lose()
    {
        gameOver.SetActive(true);
    }

    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}