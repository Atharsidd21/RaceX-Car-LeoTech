using Unity.Cinemachine;
using UnityEngine;

public class CarSpawn : MonoBehaviour
{
    public static CarSpawn instance;
    public GameObject owncar;
    public GameObject[] vehiclePrefabs; // Assign car prefabs in Inspector
    public Transform spawnPoint;        // Assign spawn location



    private void Awake()
    {
        instance = this;
    }


    void Start()
    {
        int selectedIndex = PlayerPrefs.GetInt("Pointer", 0);

        // Safety check: make sure index is valid
        if (selectedIndex >= 0 && selectedIndex < vehiclePrefabs.Length)
        {
            GameObject car = Instantiate(vehiclePrefabs[selectedIndex], spawnPoint.position, spawnPoint.rotation);
            GameManager.Instance.AssignPlayer(car); //  Send the car to GameManager directly



        }

    }



}
