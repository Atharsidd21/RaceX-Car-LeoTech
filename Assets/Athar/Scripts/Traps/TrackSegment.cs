using UnityEngine;

public class TrackSegment : MonoBehaviour
{
    [Header("Segment Settings")]
    public bool allowWrongWayDetection = true;

    public Vector3 Forward => transform.forward;
}
