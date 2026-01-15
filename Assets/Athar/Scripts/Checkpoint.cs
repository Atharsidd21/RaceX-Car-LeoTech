using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int index;

    private void OnTriggerEnter(Collider other)
    {
        GameObject car = other.transform.root.gameObject;
        End end = FindObjectOfType<End>();

        if (end != null) ;
           // end.OnCheckpointPassed(car, index);
    }
}
