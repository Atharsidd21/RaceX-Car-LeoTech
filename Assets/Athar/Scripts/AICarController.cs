using System.Collections;
using UnityEngine;

//[RequireComponent(typeof(Rigidbody))]
public class AICarController : MonoBehaviour
{
    /*/waypoints
    public Transform[] waypoints;
    public float turnSpeed = 6f;
    public float waypointThreshold = 2f;
    private int currentWaypointIndex = 0;

    //difficulty settings
    public enum AIDifficulty { Easy, Medium, Hard }
    public AIDifficulty difficulty = AIDifficulty.Hard;

    //speed and movement
    [Header("Speed")]
    public float minSpeed = 10f;     // ONLY for recovery
    public float maxSpeed = 18f;     // NORMAL race speed

    [Header("Acceleration")]
    public float acceleration = 10f;

    [Header("Catch-Up (Rubber Band)")]
    public float catchUpDistance = 15f;
    public float strongCatchMultiplier = 1.25f;
    public float mildCatchMultiplier = 1.1f;

    [Header("Finish Line Aggression")]
    public float finishPushMultiplier = 1.4f;
    public float finishDistance = 30f;

     //steering 
    [Header("Steering")]
    public float steeringError = 0f; // Hard AI = perfect

    //recovery
    [Header("Recovery")]
    public float recoveryTime = 1.5f;
    private bool isRecovering = false;

    
    private Rigidbody rb;
    private Transform player;
    private float currentSpeed;
    private float targetSpeed;

 
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.5f, 0);

        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        ApplyDifficulty();
        currentSpeed = maxSpeed; // IMPORTANT: start at race speed
    }

    void FixedUpdate()
    {
        if (waypoints.Length == 0) return;

        HandleMovement();
        SwitchWaypoint();
    }

    // CORE MOVEMENT
   
    void HandleMovement()
    {
        Transform targetWaypoint = waypoints[currentWaypointIndex];

        // Direction
        Vector3 direction = (targetWaypoint.position - transform.position).normalized;
        Vector3 flatDirection = new Vector3(direction.x, 0, direction.z);

        // Steering
        if (flatDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(flatDirection);
            targetRotation *= Quaternion.Euler(0, Random.Range(-steeringError, steeringError), 0);

            rb.MoveRotation(
                Quaternion.Slerp(rb.rotation, targetRotation, Time.fixedDeltaTime * turnSpeed)
            );
        }

        //Speed Control
        //Always race at max speed
        targetSpeed = maxSpeed;

        if (player != null && !isRecovering)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            // STRONG catch-up ONLY when far behind
            if (distanceToPlayer > catchUpDistance)
            {
                targetSpeed = maxSpeed * strongCatchMultiplier;
            }
            // Mild pressure when slightly behind
            else if (distanceToPlayer > 8f)
            {
                targetSpeed = maxSpeed * mildCatchMultiplier;
            }
        }

        // finish line push
        float distanceToFinish = Vector3.Distance(
            transform.position,
            waypoints[waypoints.Length - 1].position
        );

        if (distanceToFinish < finishDistance)
        {
            targetSpeed *= finishPushMultiplier;
        }

        // acceleration
        currentSpeed = Mathf.MoveTowards(
            currentSpeed,
            targetSpeed,
            acceleration * Time.fixedDeltaTime
        );

        // move
        rb.MovePosition(rb.position + transform.forward * currentSpeed * Time.fixedDeltaTime);
    }

    // Waypoint switching
    void SwitchWaypoint()
    {
        float distance = Vector3.Distance(
            transform.position,
            waypoints[currentWaypointIndex].position
        );

        if (distance < waypointThreshold)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }

    //Setting difficulty parameters
    void ApplyDifficulty()
    {
        switch (difficulty)
        {
            case AIDifficulty.Easy:
                maxSpeed = 15f;
                steeringError = 3f;
                break;

            case AIDifficulty.Medium:
                maxSpeed = 17f;
                steeringError = 1.5f;
                break;

            case AIDifficulty.Hard:
                maxSpeed = 19f;
                steeringError = 0f;
                break;
        }
    }

   
    // Recovery from collision 
  
    private void OnCollisionEnter(Collision collision)
    {
        if (!isRecovering && collision.gameObject.CompareTag("Obstacle"))
        {
            StartCoroutine(RecoveryRoutine());
        }
    }

    IEnumerator RecoveryRoutine()
    {
        isRecovering = true;

        // Temporary slowdown
        currentSpeed = minSpeed;

        yield return new WaitForSeconds(recoveryTime);

        // return to MAX speed
        currentSpeed = maxSpeed;
        isRecovering = false;
    }*/
}
