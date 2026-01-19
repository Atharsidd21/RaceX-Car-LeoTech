using UnityEngine;
using System;

public class WrongWayDetector : MonoBehaviour
{
    public static event Action<bool> OnWrongWayChanged;

    [Header("Detection")]
    [SerializeField] private float checkInterval = 0.2f;
    [SerializeField] private float minSpeedToCheck = 5f;

    private Rigidbody rb;
    private Vector3 lastValidForward;
    private bool isWrongWay = false;

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
        if (rb.linearVelocity.magnitude < minSpeedToCheck)
            return;

        Vector3 moveDir = rb.linearVelocity.normalized;
        float dot = Vector3.Dot(moveDir, lastValidForward);

        Debug.Log($"DOT = {dot}");

        bool wrongNow = dot < 0.2f; // ? FIX

        if (wrongNow != isWrongWay)
        {
            isWrongWay = wrongNow;
            Debug.Log("?? WRONG WAY STATE = " + isWrongWay);
            OnWrongWayChanged?.Invoke(isWrongWay);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("TRIGGER ENTERED BY: " + other.name);

        if (!other.CompareTag("TrackDirection"))
            return;

        // Update correct forward direction
        lastValidForward = other.transform.forward;
    }
}
