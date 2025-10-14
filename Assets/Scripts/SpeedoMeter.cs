using UnityEngine;
using TMPro;

public class Speedometer : MonoBehaviour
{
    public Rigidbody carRigidbody;      // Reference to your car's Rigidbody
    public TextMeshProUGUI speedText;   // TMP UI Text component
    public bool useKPH = true;          // Toggle for units (true = km/h, false = mph)

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (carRigidbody == null || speedText == null || mainCamera == null)
            return;

        // --- Update Speed ---
        float speed = carRigidbody.velocity.magnitude;
        float displaySpeed = useKPH ? speed * 3.6f : speed * 2.237f;
        speedText.text = Mathf.RoundToInt(displaySpeed) + (useKPH ? " km/h" : " mph");

        // --- Make text face the camera only on Y-axis ---
        Vector3 lookDir = mainCamera.transform.position - transform.position;
        lookDir.y = 0; // ignore vertical
        if (lookDir.sqrMagnitude > 0.001f)
        {
            // Flip the text so front faces camera
            transform.rotation = Quaternion.LookRotation(-lookDir);
        }
    }
}
