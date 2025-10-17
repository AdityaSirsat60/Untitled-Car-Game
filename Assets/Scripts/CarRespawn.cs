using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class CarRespawn : MonoBehaviour
{
    [Header("References")]
    public EndlessRoad roadManager;
    public Transform car;
    public float fallThreshold = -5f;
    public float respawnHeight = 2f;
    public float resetVelocityDelay = 0.05f;

    void Update()
    {
        if (car.position.y < fallThreshold)
            RespawnCar();
    }

    void RespawnCar()
    {
        if (roadManager == null)
        {
            Debug.LogWarning("CarRespawn: Missing EndlessRoad reference.");
            return;
        }

        LinkedList<GameObject> roads = GetPrivateField<LinkedList<GameObject>>(roadManager, "roads");
        if (roads == null || roads.Count == 0)
        {
            Debug.LogWarning("CarRespawn: No road segments found.");
            return;
        }

        GameObject closestRoad = roads
            .OrderBy(r => Mathf.Abs(r.transform.position.z - car.position.z))
            .FirstOrDefault();

        if (closestRoad == null)
            return;

        // Account for pivot at corner instead of center
        float roadWidth = roadManager.roadWidth;
        float roadLength = roadManager.roadLength;
        Vector3 roadCenter = closestRoad.transform.position + new Vector3(roadWidth * -1, 0f, roadLength * -1f);

        // Set car position above road center
        Vector3 newPos = roadCenter + Vector3.up * respawnHeight;
        car.position = newPos;
        car.rotation = Quaternion.Euler(0,-180,0);

        Rigidbody rb = car.GetComponent<Rigidbody>();
        if (rb != null)
            StartCoroutine(ResetVelocity(rb));
    }

    System.Collections.IEnumerator ResetVelocity(Rigidbody rb)
    {
        yield return new WaitForSeconds(resetVelocityDelay);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private T GetPrivateField<T>(object obj, string fieldName)
    {
        var field = obj.GetType().GetField(fieldName,
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return field != null ? (T)field.GetValue(obj) : default;
    }
}
