using System.Collections;
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
            owncar = car;

            // 🔹 DELAY RIGIDBODY ACTIVATION (CRITICAL)
            Rigidbody rb = car.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                StartCoroutine(EnablePhysicsNextFrame(rb));
            }

            GameManager.Instance.AssignPlayer(car); //  Send the car to GameManager directly

           

        }

    }
    // 🔹 Coroutine to safely enable physics
    private IEnumerator EnablePhysicsNextFrame(Rigidbody rb)
    {
        // Wait for one physics step
        yield return new WaitForFixedUpdate();

        if (rb != null)
            rb.isKinematic = false;
    }



}