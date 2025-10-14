using UnityEngine;
using System.Collections.Generic;

public class EndlessRoad : MonoBehaviour
{
    public GameObject roadPrefab;
    public int roadCount = 7;           // total segments (odd number recommended)
    public float roadLength = 50f;
    public float roadWidth = 10f;
    public Transform player;
    public float recycleBuffer = 20f;   // how early to recycle segments

    private LinkedList<GameObject> roads = new LinkedList<GameObject>();
    private float lastPlayerZ;

    void Start()
    {
        lastPlayerZ = player.position.z;

        int halfCount = roadCount / 2;

        // Spawn roads **behind the car**
        for (int i = halfCount; i > 0; i--)
        {
            float zPos = player.position.z - i * roadLength;
            SpawnRoadAtFront(zPos);  // Add behind at start of list
        }

        // Spawn the road **under the car** (center segment)
        SpawnRoadAtEnd(player.position.z);

        // Spawn roads **in front of the car**
        for (int i = 1; i <= halfCount; i++)
        {
            float zPos = player.position.z + i * roadLength;
            SpawnRoadAtEnd(zPos);
        }
    }

    void Update()
    {
        float zMovement = player.position.z - lastPlayerZ;
        lastPlayerZ = player.position.z;

        if (Mathf.Abs(zMovement) < 0.01f) return; // stationary

        if (zMovement > 0) // moving +Z
        {
            GameObject lastRoad = roads.Last.Value;
            if (player.position.z + recycleBuffer > lastRoad.transform.position.z + roadLength / 2f)
                MoveRoadForward();
        }
        else // moving -Z
        {
            GameObject firstRoad = roads.First.Value;
            if (player.position.z - recycleBuffer < firstRoad.transform.position.z - roadLength / 2f)
                MoveRoadBackward();
        }
    }

    // Spawn road at the **end** of the LinkedList (front)
    void SpawnRoadAtEnd(float zPos)
    {
        Vector3 pos = new Vector3(roadWidth * 0.5f, 0f, zPos - roadLength * 0.5f);
        GameObject obj = Instantiate(roadPrefab, pos, Quaternion.identity);
        roads.AddLast(obj);
    }

    // Spawn road at the **front** of the LinkedList (behind)
    void SpawnRoadAtFront(float zPos)
    {
        Vector3 pos = new Vector3(roadWidth * 0.5f, 0f, zPos - roadLength * 0.5f);
        GameObject obj = Instantiate(roadPrefab, pos, Quaternion.identity);
        roads.AddFirst(obj);
    }

    void MoveRoadForward()
    {
        GameObject obj = roads.First.Value;
        roads.RemoveFirst();

        float newZ = roads.Last.Value.transform.position.z + roadLength;
        obj.transform.position = new Vector3(roadWidth * 0.5f, 0f, newZ);
        roads.AddLast(obj);
    }

    void MoveRoadBackward()
    {
        GameObject obj = roads.Last.Value;
        roads.RemoveLast();

        float newZ = roads.First.Value.transform.position.z - roadLength;
        obj.transform.position = new Vector3(roadWidth * 0.5f, 0f, newZ);
        roads.AddFirst(obj);
    }
}
