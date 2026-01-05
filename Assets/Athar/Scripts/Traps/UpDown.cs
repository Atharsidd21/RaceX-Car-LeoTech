using UnityEngine;

public class UpDown : MonoBehaviour
{
    public float moveSpeed = 2f;     // Speed of up/down movement
    public float moveRange = 3f;     // Total range of movement
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        float y = Mathf.PingPong(Time.time * moveSpeed, moveRange) - moveRange / 2f;
        transform.position = startPosition + new Vector3(0f, y, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
          //  GameManager.Instance.DamageCar(); // Damage the car when touched
        }
    }
}
