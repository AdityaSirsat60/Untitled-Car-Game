using UnityEngine;
using System.Collections.Generic;

public class EndlessRoad : MonoBehaviour
{
    [Header("Road Settings")]
    public GameObject roadPrefab;
    public int roadCount = 7;
    public float roadLength = 50f;
    public float roadWidth = 10f;

    [Header("Player Settings")]
    public Transform player;
    public float recycleBuffer = 20f;

    [Header("Zombie Settings")]
    public GameObject zombiePrefab;
    [Range(0f, 1f)] public float zombieSpawnChance = 0.5f;
    public float spawnAreaWidth = 6f;
    public float spawnAreaDepth = 30f;
    public Vector3 spawnOffset = Vector3.zero;
    public bool showSpawnAreaGizmos = true;

    private LinkedList<GameObject> roads = new LinkedList<GameObject>();
    private float lastPlayerZ;

    void Start()
    {
        lastPlayerZ = player.position.z;
        int halfCount = roadCount / 2;

        for (int i = halfCount; i > 0; i--)
            SpawnRoadAtEnd(player.position.z - i * roadLength);

        SpawnRoadAtEnd(player.position.z);

        for (int i = 1; i <= halfCount; i++)
            SpawnRoadAtEnd(player.position.z + i * roadLength);
    }

    void Update()
    {
        float zMovement = player.position.z - lastPlayerZ;
        lastPlayerZ = player.position.z;
        if (Mathf.Abs(zMovement) < 0.01f) return;

        if (zMovement > 0)
        {
            GameObject lastRoad = roads.Last.Value;
            if (player.position.z + recycleBuffer > lastRoad.transform.position.z + roadLength / 2f)
                MoveRoadForward();
        }
        else
        {
            GameObject firstRoad = roads.First.Value;
            if (player.position.z - recycleBuffer < firstRoad.transform.position.z - roadLength / 2f)
                MoveRoadBackward();
        }
    }

    void SpawnRoadAtEnd(float zPos)
    {
        Vector3 pos = new Vector3(roadWidth * 0.5f, 0f, zPos - roadLength * 0.5f);
        GameObject obj = ObjectPool.Instance.SpawnFromPool(roadPrefab, pos, Quaternion.identity);
        roads.AddLast(obj);
        TrySpawnZombie(obj);
    }

    void SpawnRoadAtFront(float zPos)
    {
        Vector3 pos = new Vector3(roadWidth * 0.5f, 0f, zPos - roadLength * 0.5f);
        GameObject obj = ObjectPool.Instance.SpawnFromPool(roadPrefab, pos, Quaternion.identity);
        roads.AddFirst(obj);
        TrySpawnZombie(obj);
    }

    void MoveRoadForward()
    {
        GameObject obj = roads.First.Value;
        roads.RemoveFirst();

        float newZ = roads.Last.Value.transform.position.z + roadLength;
        obj.transform.position = new Vector3(roadWidth * 0.5f, 0f, newZ);
        roads.AddLast(obj);

        ClearOldZombies(obj);
        TrySpawnZombie(obj);
    }

    void MoveRoadBackward()
    {
        GameObject obj = roads.Last.Value;
        roads.RemoveLast();

        float newZ = roads.First.Value.transform.position.z - roadLength;
        obj.transform.position = new Vector3(roadWidth * 0.5f, 0f, newZ);
        roads.AddFirst(obj);

        ClearOldZombies(obj);
        TrySpawnZombie(obj);
    }

    void TrySpawnZombie(GameObject road)
    {
        if (zombiePrefab == null) return;
        if (Random.value > zombieSpawnChance) return;

        float xOffset = Random.Range(-spawnAreaWidth * 0.5f, spawnAreaWidth * 0.5f);
        float zOffset = Random.Range(-spawnAreaDepth * 0.5f, spawnAreaDepth * 0.5f);

        Vector3 spawnPos = road.transform.position + spawnOffset + new Vector3(xOffset, 0f, zOffset);

        GameObject zombie = ObjectPool.Instance.SpawnFromPool(zombiePrefab, spawnPos, Quaternion.identity, road.transform);
        ZombieRagdoll zr = zombie.GetComponent<ZombieRagdoll>();
        if (zr != null) zr.Initialize();
    }

    void ClearOldZombies(GameObject road)
    {
        foreach (Transform child in road.transform)
        {
            if (child.CompareTag("Zombie"))
                ObjectPool.Instance.ReturnToPool(child.gameObject);
        }
    }

    void OnDrawGizmos()
    {
        if (!showSpawnAreaGizmos) return;

        Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.25f);

        if (roads != null)
        {
            foreach (GameObject road in roads)
            {
                if (road == null) continue;
                Vector3 boxCenter = road.transform.position + spawnOffset;
                Gizmos.DrawWireCube(boxCenter, new Vector3(spawnAreaWidth, 0.1f, spawnAreaDepth));
            }
        }
        else if (Application.isEditor && !Application.isPlaying && roadPrefab != null)
        {
            Vector3 boxCenter = transform.position + spawnOffset;
            Gizmos.DrawWireCube(boxCenter, new Vector3(spawnAreaWidth, 0.1f, spawnAreaDepth));
        }
    }
}
