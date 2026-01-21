using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int checkpointIndex;

    private void OnTriggerEnter(Collider other)
    {
        var car = other.transform.root.GetComponent<RaceProgressTracker>();
        if (car == null || car.finished) return;

       // car.PassCheckpoint(checkpointIndex, transform);
    }
}
