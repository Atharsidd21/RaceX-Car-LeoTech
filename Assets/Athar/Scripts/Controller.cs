using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
   public AudioSource engineAudio;

    [Header("Wheels")]
    [SerializeField] GameObject Fl_Wheel;
    [SerializeField] GameObject Bl_Wheel;
    [SerializeField] GameObject Fr_Wheel;
    [SerializeField] GameObject Br_Wheel;

    [Header("Wheel Colliders")]
    [SerializeField] WheelCollider Fl_WheelCollider;
    [SerializeField] WheelCollider Bl_WheelCollider;
    [SerializeField] WheelCollider Fr_WheelCollider;
    [SerializeField] WheelCollider Br_WheelCollider;

    [Header("Movement Settings")]
    [SerializeField] float MaxMotorTorque;
    [SerializeField] float MaxSteeringAngle;
    [SerializeField] public float MaxSpeed;
    [SerializeField] float BrakingPower;
    [SerializeField] Transform Com;
    public float CarSpeedConverted;

    [Header("Boost Handling Adjustment")]
    [SerializeField] float boostSteeringMultiplier = 0.5f; // 50% steering during boost

    private float originalMaxSteeringAngle;


    float motorTorque;
    float VertcalPos = 0;
    float HorizontalPos = 0;
    Rigidbody Car_Rb;

    [Header("Smoke Effects")]
    public ParticleSystem driftSmoke;
    public ParticleSystem boostSmoke;

    [Header("Drift Sound")]
    public AudioClip driftClip;
    public AudioSource driftAudio;

    [Header("Car Info")]
    public int CarPrice;
    public string CarName;

    private bool isBoosting = false;
    private float originalMotorTorque;
    private float boostEndTime;
    private bool hasGameEnded = false;

    public Sprite Acceleration_Highlight;
    public Sprite Break_Highlight;
    public Sprite Left_Highlight;
    public Sprite Right_Highlight;

    public Sprite Acceleration;
    public Sprite Break;
    public Sprite Left;
    public Sprite Right;

    public GameObject Acceleration_parent;
    public GameObject Break_parent;
    public GameObject Left_parent;
    public GameObject Right_parent;

    //New variables for damage speed penalty
    //Stop Method 
    [Header("Damage Stop Penalty")]
    [SerializeField] float stopDuration = 1.5f;

    private bool isStopped = false;

    //Slow down method 
    [Header("Damage Speed Slow")]
    [SerializeField] float damagedMaxSpeed = 20f; // temporary speed after hit
    [SerializeField] float slowDuration = 2f;

    private bool isSpeedLimited = false;
    private float originalMaxSpeed;
    




    private void Awake()
    {
        Car_Rb = GetComponent<Rigidbody>();
        //engineAudio = GetComponent<AudioSource>();
        if (Car_Rb != null)
            Car_Rb.centerOfMass = Com.localPosition;

        //driftAudio = gameObject.AddComponent<AudioSource>();
        driftAudio.playOnAwake = false;
        driftAudio.loop = true;
        driftAudio.volume = 1.6f;
        driftAudio.spatialBlend = 1f;
        if (driftClip != null)
            driftAudio.clip = driftClip;
    }

    private void Update()
    {
        if (hasGameEnded) return;

        CalculateMovement();
        Steering();
        ApplyTransformToWheels();
        HandleEngineAudio();
        HandleDriftSound();
    }

    public void MoveInput(float input)
    {
        if (input == 1)
        {
            Acceleration_parent.transform.GetComponent<Image>().sprite = Acceleration_Highlight;
        }
        else if (input == -1)
        {
            Break_parent.transform.GetComponent<Image>().sprite = Break_Highlight;
        }
        else if (input == 0)
        {
            Acceleration_parent.transform.GetComponent<Image>().sprite = Acceleration;
            Break_parent.transform.GetComponent<Image>().sprite = Break;
        }

        VertcalPos = input;
    }

    public void SteeringInput(float input)
    {
        if (input == 1)
        {
            Right_parent.transform.GetComponent<Image>().sprite = Right_Highlight;
        }
        else if (input == -1)
        {
            Left_parent.transform.GetComponent<Image>().sprite = Left_Highlight;
        }
        else if (input == 0)
        {
            Right_parent.transform.GetComponent<Image>().sprite = Right;
            Left_parent.transform.GetComponent<Image>().sprite = Left;
        }


        Debug.Log("The staering input is ----------------- ");
        HorizontalPos = input;
    }

    void CalculateMovement()
    {
        float CarSpeed = Car_Rb.linearVelocity.magnitude;
        CarSpeedConverted = Mathf.Round(CarSpeed * 3.6f);

        if (CarSpeedConverted < MaxSpeed)
        {
            motorTorque = MaxMotorTorque * VertcalPos;
        }
        else
        {
            motorTorque = 0;
        }

        ApplyMotorTorque();
    }

    void ApplyMotorTorque()
    {
        Fl_WheelCollider.motorTorque = motorTorque;
        Fr_WheelCollider.motorTorque = motorTorque;
        Bl_WheelCollider.motorTorque = motorTorque;
        Br_WheelCollider.motorTorque = motorTorque;
    }

    public void Steering()
    {

        float angle = MaxSteeringAngle * HorizontalPos;
        Debug.Log("Here the angle is " + angle);
        Fr_WheelCollider.steerAngle = angle;
        Fl_WheelCollider.steerAngle = angle;
    }

    public void ApplyTransformToWheels()
    {
        UpdateWheel(Fl_WheelCollider, Fl_Wheel);
        UpdateWheel(Fr_WheelCollider, Fr_Wheel);
        UpdateWheel(Bl_WheelCollider, Bl_Wheel);
        UpdateWheel(Br_WheelCollider, Br_Wheel);
    }

    void UpdateWheel(WheelCollider collider, GameObject wheel)
    {
        collider.GetWorldPose(out Vector3 pos, out Quaternion rot);
        wheel.transform.position = pos;
        wheel.transform.rotation = rot;
    }

    void HandleEngineAudio()
    {
        if (!engineAudio.isPlaying)
            engineAudio.Play();

        float speed01 = Mathf.Clamp01(CarSpeedConverted / MaxSpeed);
        float targetVolume = Mathf.Lerp(0.2f, 1.2f, speed01);
        engineAudio.volume = Mathf.Lerp(engineAudio.volume, targetVolume, Time.deltaTime * 5f);
        engineAudio.pitch = Mathf.Clamp(1f + (CarSpeedConverted / 100f), 1f, 2f);
    }

    void HandleDriftSound()
    {
        if (driftAudio == null || driftClip == null) return;

        bool isTurning = Mathf.Abs(HorizontalPos) > 0.2f;
        bool isMoving = CarSpeedConverted > 20f;

        if (isTurning && isMoving)
        {
            if (!driftAudio.isPlaying) driftAudio.Play();
            if (!driftSmoke.isPlaying) driftSmoke.Play();
        }
        else
        {
            if (driftAudio.isPlaying) driftAudio.Stop();
            if (driftSmoke.isPlaying) driftSmoke.Stop();
        }
    }

    public void ActivateSpeedBoost(float multiplier, float duration)
    {
        Debug.Log("BOOST TRIGGERED");

        if (isBoosting) return;

        // Cache original values
        originalMotorTorque = MaxMotorTorque;
        originalMaxSteeringAngle = MaxSteeringAngle;

        // Apply boost
        MaxMotorTorque *= multiplier;
        MaxSteeringAngle *= boostSteeringMultiplier;

        isBoosting = true;
        boostEndTime = Time.time + duration;

        // Visual effect
        if (boostSmoke != null && !boostSmoke.isPlaying)
            boostSmoke.Play();
    }


    private void LateUpdate()
    {
        if (!isBoosting) return;

        if (Time.time >= boostEndTime)
        {
            // Restore original handling
            MaxMotorTorque = originalMotorTorque;
            MaxSteeringAngle = originalMaxSteeringAngle;

            isBoosting = false;

            // Stop boost effect
            if (boostSmoke != null && boostSmoke.isPlaying)
                boostSmoke.Stop();
        }
    }



    public void OnGameOver()
    {
      hasGameEnded = true;

        if (engineAudio != null) engineAudio.Stop();
        if (driftAudio != null) driftAudio.Stop();
        if (driftSmoke != null) driftSmoke.Stop();
        if (boostSmoke != null) boostSmoke.Stop();
    }

    public void OnPause()
    {
        engineAudio?.Pause();
        driftAudio?.Pause();
    }

    public void OnResume()
    {
        hasGameEnded = false;
        engineAudio?.UnPause();
        driftAudio?.UnPause();
    }
    //Stop Method Implementation
    public void ApplyStop()
    {
        if (isStopped) return;
        StartCoroutine(StopCoroutine());
    }
    private IEnumerator StopCoroutine()
    {
        isStopped = true;

        float cachedSpeed = MaxSpeed;
        float cachedTorque = MaxMotorTorque;

        MaxSpeed = 0f;
        MaxMotorTorque = 0f;

        Car_Rb.linearVelocity = Vector3.zero;
        Car_Rb.angularVelocity = Vector3.zero;

        yield return new WaitForSeconds(stopDuration);

        MaxSpeed = cachedSpeed;
        MaxMotorTorque = cachedTorque;

        isStopped = false;
    }
   // Slow Down Method Implementation
    public void ApplySlowDown()
    {
        if (isSpeedLimited) return;
        StartCoroutine(SpeedLimitCoroutine());
    }
    private IEnumerator SpeedLimitCoroutine()
    {
        isSpeedLimited = true;

        originalMaxSpeed = MaxSpeed;
        MaxSpeed = damagedMaxSpeed;

        yield return new WaitForSeconds(slowDuration);

        MaxSpeed = originalMaxSpeed;
        isSpeedLimited = false;
    }




}