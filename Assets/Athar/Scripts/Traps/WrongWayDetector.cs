using UnityEngine;
using System;

public class WrongWayDetector : MonoBehaviour
{
    public static event Action<bool> OnWrongWayChanged;

    [Header("Detection")]
    [SerializeField] private float checkInterval = 0.2f;
    [SerializeField] private float minSpeedToCheck = 5f;

    [Header("Grace Settings")]
    [SerializeField] private float directionChangeGraceTime = 0.6f;

    private float lastDirectionUpdateTime;

    private Rigidbody rb;
    private Vector3 lastValidForward;
    private bool isWrongWay = false;
    private TrackSegment currentSegment;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        lastValidForward = transform.forward;
    }

    private void Start()
    {
        InvokeRepeating(nameof(CheckDirection), 0f, checkInterval);
    }

    private void CheckDirection()
    {
        if (currentSegment == null)
            return;

        if (!currentSegment.allowWrongWayDetection)
            return;

        float angle = Vector3.Angle(transform.forward, currentSegment.Forward);

        bool wrongNow = angle > 120f;

        if (wrongNow != isWrongWay)
        {
            isWrongWay = wrongNow;
            OnWrongWayChanged?.Invoke(isWrongWay);
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        TrackSegment segment = other.GetComponent<TrackSegment>();
        if (segment == null)
            return;

        currentSegment = segment;

        // ?? HARD RESET when changing segment
        isWrongWay = false;
        OnWrongWayChanged?.Invoke(false);
    }

}
