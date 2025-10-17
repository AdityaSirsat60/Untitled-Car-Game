using UnityEngine;

public class MainMenuCameraController : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;             // Focus object in the scene
    public Vector3 offset = new Vector3(0, 2f, -5f);
    public float distance = 5f;

    [Header("Orbit Settings")]
    public bool autoRotate = true;
    public float rotationSpeed = 10f;
    public float manualSpeed = 60f;
    public float smoothTime = 0.2f;

    [Header("Zoom Settings")]
    public float zoomSpeed = 2f;
    public float minDistance = 3f;
    public float maxDistance = 10f;

    private float yaw;
    private float pitch;
    private Vector3 currentVelocity;

    void Start()
    {
        
        if (target == null)
        {
            Debug.LogWarning("MainMenuCameraController: No target assigned.");
            enabled = false;
            return;
        }

        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Manual rotation (optional mouse drag)
        if (Input.GetMouseButton(0))
        {
            yaw += Input.GetAxis("Mouse X") * manualSpeed * Time.deltaTime;
            pitch -= Input.GetAxis("Mouse Y") * manualSpeed * Time.deltaTime;
        }
        else if (autoRotate)
        {
            yaw += rotationSpeed * Time.deltaTime;
        }

        pitch = Mathf.Clamp(pitch, -20f, 80f);

        // Zoom
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        distance = Mathf.Clamp(distance - scroll * zoomSpeed, minDistance, maxDistance);

        // Calculate target position
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 desiredPosition = target.position + rotation * new Vector3(0, 0, -distance) + offset;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, smoothTime);

        transform.LookAt(target.position);
    }
}
