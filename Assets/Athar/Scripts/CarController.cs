using UnityEngine;
using UnityEngine.SceneManagement;

public class CarController : MonoBehaviour
{
    [Header("Car Info")]
    public string CarName;
    public int CarPrice;

    //  [Header("Rotation Settings")]
    // [SerializeField] private float rotationSpeed = 0f;

    private void Update()
    {
        // Only rotate the car in the SelectCar scene
        if (SceneManager.GetActiveScene().name == "SelectCar")
        {
            // transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        }
    }
}