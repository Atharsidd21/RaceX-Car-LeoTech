using UnityEngine;

public class LeftRight : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float moveRange = 3f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        float x = Mathf.PingPong(Time.time * moveSpeed, moveRange) - moveRange / 2f;
        transform.position = new Vector3(startPosition.x + x, startPosition.y, startPosition.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Debug.Log("Player hit by moving obstacle");
           // GameManager.Instance?.DamageCar();
        }
    }
}
