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
    public float brakeTorque = 3000f;        // Regular braking
    public float handBrakeTorque = 5000f;    // Stronger for handbrake

    private float motorInput;
    private float steeringInput;
    private bool isBraking;
    private bool isHandbrake;

    void Start()
    {
        SetupWheelFriction(frontLeftWheel);
        SetupWheelFriction(frontRightWheel);
        SetupWheelFriction(rearLeftWheel);
        SetupWheelFriction(rearRightWheel);
    }

    void Update()
    {
        motorInput = Input.GetAxis("Vertical");        // W/S
        steeringInput = Input.GetAxis("Horizontal");   // A/D

        // Check for braking
        isBraking = Input.GetKey(KeyCode.Space);       // Normal brake: Space
        isHandbrake = Input.GetKey(KeyCode.LeftControl); // Handbrake: Left Ctrl
    }

    void SetupWheelFriction(WheelCollider wheel)
    {
        WheelFrictionCurve forward = wheel.forwardFriction;
        forward.extremumSlip = 0.4f;
        forward.extremumValue = 1.5f;
        forward.asymptoteSlip = 0.8f;
        forward.asymptoteValue = 1f;
        forward.stiffness = 1.0f; // <- Increase this for more grip
        wheel.forwardFriction = forward;

        WheelFrictionCurve sideways = wheel.sidewaysFriction;
        sideways.extremumSlip = 0.2f;
        sideways.extremumValue = 1.5f;
        sideways.asymptoteSlip = 0.5f;
        sideways.asymptoteValue = 1f;
        sideways.stiffness = 1.0f; // <- More lateral grip
        wheel.sidewaysFriction = sideways;
    }
    void FixedUpdate()
    {
        // Handle steering
        float steerAngle = steeringInput * maxSteeringAngle;
        frontLeftWheel.steerAngle = steerAngle;
        frontRightWheel.steerAngle = steerAngle;

        // Handle motor torque
        if (!isBraking && !isHandbrake)
        {
            rearLeftWheel.motorTorque = motorInput * maxMotorTorque;
            rearRightWheel.motorTorque = motorInput * maxMotorTorque;

            rearLeftWheel.brakeTorque = 0f;
            rearRightWheel.brakeTorque = 0f;
            frontLeftWheel.brakeTorque = 0f;
            frontRightWheel.brakeTorque = 0f;
        }

        // Normal brake
        if (isBraking)
        {
            rearLeftWheel.brakeTorque = brakeTorque;
            rearRightWheel.brakeTorque = brakeTorque;
            frontLeftWheel.brakeTorque = brakeTorque;
            frontRightWheel.brakeTorque = brakeTorque;

            rearLeftWheel.motorTorque = 0f;
            rearRightWheel.motorTorque = 0f;
        }

        // Handbrake (rear wheels only)
        if (isHandbrake)
        {
            rearLeftWheel.brakeTorque = handBrakeTorque;
            rearRightWheel.brakeTorque = handBrakeTorque;

            rearLeftWheel.motorTorque = 0f;
            rearRightWheel.motorTorque = 0f;
        }

        // Update visuals
        UpdateWheelVisual(frontLeftWheel, frontLeftTransform);
        UpdateWheelVisual(frontRightWheel, frontRightTransform);
        UpdateWheelVisual(rearLeftWheel, rearLeftTransform);
        UpdateWheelVisual(rearRightWheel, rearRightTransform);
    }

    void UpdateWheelVisual(WheelCollider col, Transform trans)
    {
        Vector3 pos;
        Quaternion rot;
        col.GetWorldPose(out pos, out rot);
        trans.position = pos;
        trans.rotation = rot;
    }
}
