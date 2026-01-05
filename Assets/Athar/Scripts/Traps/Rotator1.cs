using UnityEngine;

public class Rotator1 : MonoBehaviour
{
    public float rotationSpeed = 20f; // Degrees per second

    //void Update()
    //{
    //    // Rotate around Y-axis
    //    transform.Rotate(0f, rotationSpeed * Time.deltaTime,0f);
    //}
    public float rotationRange = 50f;       // Max degrees to rotate left and right
    
    private Quaternion startRotation;

    void Start()
    {
        startRotation = transform.rotation;
    }

    void Update()
    {
        float angle = Mathf.Sin(Time.time * rotationSpeed) * rotationRange;
        transform.rotation = startRotation * Quaternion.Euler(0f, 0f, angle);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
           // GameManager.Instance.DamageCar(); // Call your car damage
        }
    }
}


