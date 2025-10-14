using UnityEngine;

public class CenterOfMassDebug : MonoBehaviour
{
    public Rigidbody carRigidbody;
    public float sphereSize = 0.2f; // Size of the visual sphere

    void OnDrawGizmos()
    {
        if (carRigidbody == null) return;

        Gizmos.color = Color.red;
        // Transform the COM from local to world space
        Vector3 comWorld = carRigidbody.worldCenterOfMass;
        Gizmos.DrawSphere(comWorld, sphereSize);
    }
}
