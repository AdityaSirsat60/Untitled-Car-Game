using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;

    private Dictionary<GameObject, Queue<GameObject>> poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>Spawn a pooled object. If none exist, it will instantiate.</summary>
    public GameObject SpawnFromPool(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (!poolDictionary.ContainsKey(prefab))
            poolDictionary[prefab] = new Queue<GameObject>();

        GameObject obj;
        if (poolDictionary[prefab].Count > 0)
        {
            obj = poolDictionary[prefab].Dequeue();
            obj.SetActive(true);
        }
        else
        {
            obj = Instantiate(prefab, position, rotation, parent);
        }

        obj.transform.position = position;
        obj.transform.rotation = rotation;
        if (parent != null) obj.transform.parent = parent;

        // If the object has a pooled initializer (like ZombieRagdoll.Initialize)
        var init = obj.GetComponent<IPooledObject>();
        if (init != null) init.OnObjectSpawn();

        return obj;
    }

    /// <summary>Return object to the pool.</summary>
    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        GameObject prefab = obj; // assuming prefab reference is same; optionally store prefab reference in pooled objects

        if (!poolDictionary.ContainsKey(prefab))
            poolDictionary[prefab] = new Queue<GameObject>();

        poolDictionary[prefab].Enqueue(obj);
    }
}

/// <summary>Optional interface for pooled objects to initialize themselves.</summary>
public interface IPooledObject
{
    void OnObjectSpawn();
}
