using UnityEngine;

public class AccelerationTimer : MonoBehaviour
{
    public Rigidbody carRigidbody; // Reference to your car's Rigidbody

    private bool isTesting = false;
    private bool hasFinished = false;
    private float startTime;
    private float zeroToHundredTarget = 27.78f; // 100 km/h in m/s

    void Update()
    {
        if (!carRigidbody) return;

        // Detect when player first starts accelerating (press W or UpArrow)
        if (Input.GetKeyDown(KeyCode.W) && !isTesting)
        {
            // Start the test only if car is nearly stationary
            if (carRigidbody.velocity.magnitude < 1f)
            {
                isTesting = true;
                hasFinished = false;
                startTime = Time.time;
                Debug.Log("🚦 0–100 km/h test started!");
            }
        }

        if (isTesting && !hasFinished)
        {
            float speed = carRigidbody.velocity.magnitude;

            // Check if we've reached 100 km/h
            if (speed >= zeroToHundredTarget)
            {
                float elapsed = Time.time - startTime;
                Debug.Log($"🏁 Reached 100 km/h in {elapsed:F2} seconds!");
                hasFinished = true;
                isTesting = false;
            }
        }

        // Optional reset (press R)
        if (Input.GetKeyDown(KeyCode.R))
        {
            isTesting = false;
            hasFinished = false;
            Debug.Log("🔁 0–100 km/h test reset.");
        }
    }
}
