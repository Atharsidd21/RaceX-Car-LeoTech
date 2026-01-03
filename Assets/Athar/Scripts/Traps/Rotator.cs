using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float rotationSpeed = 20f; // Degrees per second

    void Update()
    {
        // Rotate around Y-axis
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }


}


