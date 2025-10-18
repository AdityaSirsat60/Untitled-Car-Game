using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Wheel Colliders")]
    public WheelCollider frontLeftWheel;
    public WheelCollider frontRightWheel;
    public WheelCollider rearLeftWheel;
    public WheelCollider rearRightWheel;

    [Header("Wheel Meshes")]
    public Transform frontLeftTransform;
    public Transform frontRightTransform;
    public Transform rearLeftTransform;
    public Transform rearRightTransform;

    [Header("Car Settings")]
    public float maxMotorTorque = 1500f;
    public float maxSteeringAngle = 30f;
    public float brakeTorque = 3000f;
    public float handBrakeTorque = 5000f;

    [Header("Car Sounds")]
    public AudioSource engineAudio;
    public AudioSource brakeAudio;
    public float minEnginePitch = 0.8f;
    public float maxEnginePitch = 2.0f;
    public float enginePitchMultiplier = 1.2f;

    [Header("Brake Settings")]
public float brakeSoundMinSpeed = 0.5f; // Minimum Rigidbody speed to play brake sound


    private float motorInput;
    private float steeringInput;
    private bool isBraking;
    private bool isHandbrake;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        SetupWheelFriction(frontLeftWheel);
        SetupWheelFriction(frontRightWheel);
        SetupWheelFriction(rearLeftWheel);
        SetupWheelFriction(rearRightWheel);

        if (engineAudio != null)
        {
            engineAudio.loop = true;
            engineAudio.Play();
        }
    }

    void Update()
    {
        motorInput = Input.GetAxis("Vertical");
        steeringInput = Input.GetAxis("Horizontal");

        isBraking = Input.GetKey(KeyCode.Space);
        isHandbrake = Input.GetKey(KeyCode.LeftControl);

        // Play brake sound only if the car is moving
        bool isMoving = rb.velocity.magnitude > brakeSoundMinSpeed; // adjust threshold if needed

        if ((isBraking || isHandbrake) && isMoving && brakeAudio != null && !brakeAudio.isPlaying)
        {
            brakeAudio.Play();
        }
        else if ((!isBraking && !isHandbrake) || !isMoving)
        {
            if (brakeAudio != null && brakeAudio.isPlaying)
                brakeAudio.Stop();
        }
    }

    void FixedUpdate()
    {
        float steerAngle = steeringInput * maxSteeringAngle;
        frontLeftWheel.steerAngle = steerAngle;
        frontRightWheel.steerAngle = steerAngle;

        if (!isBraking && !isHandbrake)
        {
            rearLeftWheel.motorTorque = motorInput * maxMotorTorque;
            rearRightWheel.motorTorque = motorInput * maxMotorTorque;

            rearLeftWheel.brakeTorque = 0f;
            rearRightWheel.brakeTorque = 0f;
            frontLeftWheel.brakeTorque = 0f;
            frontRightWheel.brakeTorque = 0f;
        }

        if (isBraking)
        {
            rearLeftWheel.brakeTorque = brakeTorque;
            rearRightWheel.brakeTorque = brakeTorque;
            frontLeftWheel.brakeTorque = brakeTorque;
            frontRightWheel.brakeTorque = brakeTorque;

            rearLeftWheel.motorTorque = 0f;
            rearRightWheel.motorTorque = 0f;
        }

        if (isHandbrake)
        {
            rearLeftWheel.brakeTorque = handBrakeTorque;
            rearRightWheel.brakeTorque = handBrakeTorque;

            rearLeftWheel.motorTorque = 0f;
            rearRightWheel.motorTorque = 0f;
        }

        UpdateWheelVisual(frontLeftWheel, frontLeftTransform);
        UpdateWheelVisual(frontRightWheel, frontRightTransform);
        UpdateWheelVisual(rearLeftWheel, rearLeftTransform);
        UpdateWheelVisual(rearRightWheel, rearRightTransform);

        UpdateEngineSound();
    }

    void SetupWheelFriction(WheelCollider wheel)
    {
        WheelFrictionCurve forward = wheel.forwardFriction;
        forward.extremumSlip = 0.4f;
        forward.extremumValue = 1.5f;
        forward.asymptoteSlip = 0.8f;
        forward.asymptoteValue = 1f;
        forward.stiffness = 1.0f;
        wheel.forwardFriction = forward;

        WheelFrictionCurve sideways = wheel.sidewaysFriction;
        sideways.extremumSlip = 0.2f;
        sideways.extremumValue = 1.5f;
        sideways.asymptoteSlip = 0.5f;
        sideways.asymptoteValue = 1f;
        sideways.stiffness = 1.0f;
        wheel.sidewaysFriction = sideways;
    }

    void UpdateWheelVisual(WheelCollider col, Transform trans)
    {
        Vector3 pos;
        Quaternion rot;
        col.GetWorldPose(out pos, out rot);
        trans.position = pos;
        trans.rotation = rot;
    }

    void UpdateEngineSound()
    {
        if (engineAudio == null) return;

        float avgRpm = (Mathf.Abs(rearLeftWheel.rpm) + Mathf.Abs(rearRightWheel.rpm)) * 0.5f;
        float pitch = Mathf.Lerp(minEnginePitch, maxEnginePitch, avgRpm / 3000f);
        engineAudio.pitch = Mathf.Clamp(pitch * enginePitchMultiplier, minEnginePitch, maxEnginePitch);
        engineAudio.volume = Mathf.Lerp(0.3f, 1.0f, avgRpm / 3000f);
    }
}
