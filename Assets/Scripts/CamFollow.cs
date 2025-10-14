using UnityEngine;

public class CamFollow : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;
    public Rigidbody carRigidbody;

    [Header("Follow Settings")]
    public Vector3 offset = new Vector3(0, 3, -8);
    public float followSmoothTime = 0.15f;

    [Header("Zoom Settings")]
    public float zoomInDistance = -6f;
    public float zoomOutDistance = -14f;
    public float maxSpeed = 50f; // speed at which full zoom out applies

    [Header("Zoom Curve")]
    public AnimationCurve zoomCurve = AnimationCurve.Linear(0, 0, 1, 1); 
    // X-axis = normalized speed (0-1), Y-axis = zoom interpolation (0-1)

    [Header("Mouse Look Settings")]
    public float mouseSensitivity = 3f;
    public float verticalRotationLimit = 45f;

    [Header("Camera Clamp")]
    public float minHeightAboveCar = 1.5f;
    public float maxHeightAboveCar = 10f;

    [Header("Collision Settings")]
    public LayerMask obstacleLayers;
    public float cameraRadius = 0.3f;

    private float yaw;
    private float pitch;
    private Vector3 currentVelocity;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Default zoom curve if not set
        if (zoomCurve.length == 0)
        {
            zoomCurve = AnimationCurve.Linear(0, 0, 1, 1);
        }
    }

    void LateUpdate()
    {
        if (!target || !carRigidbody) return;

        // --- Mouse Look ---
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -verticalRotationLimit, verticalRotationLimit);

        // --- Dynamic Zoom Based on Speed Curve ---
        float speed = carRigidbody.velocity.magnitude;
        float normalizedSpeed = Mathf.Clamp01(speed / maxSpeed); // 0 to 1
        float zoomLerp = zoomCurve.Evaluate(normalizedSpeed);    // Apply curve
        float dynamicZ = Mathf.Lerp(zoomInDistance, zoomOutDistance, zoomLerp);

        // Base offset
        Vector3 baseOffset = new Vector3(0, offset.y, dynamicZ);
        Vector3 rotatedOffset = Quaternion.Euler(0f, yaw, 0f) * baseOffset;
        rotatedOffset.y += Mathf.Sin(pitch * Mathf.Deg2Rad) * Mathf.Abs(dynamicZ);

        Vector3 desiredPosition = target.position + rotatedOffset;

        // --- Camera Collision Check ---
        Vector3 direction = desiredPosition - target.position;
        if (Physics.SphereCast(target.position, cameraRadius, direction.normalized, out RaycastHit hit, direction.magnitude, obstacleLayers))
        {
            desiredPosition = hit.point - direction.normalized * cameraRadius;
        }

        // Clamp height above car
        desiredPosition.y = Mathf.Clamp(desiredPosition.y, target.position.y + minHeightAboveCar, target.position.y + maxHeightAboveCar);

        // --- Smooth camera movement ---
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, followSmoothTime);

        // Look at target slightly above car
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}
