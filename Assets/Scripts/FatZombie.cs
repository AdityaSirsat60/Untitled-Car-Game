using UnityEngine;

public class FatZombie : MonoBehaviour, IPooledObject
{
    [Header("Settings")]
    public float carDamage = 50f;          // Damage dealt to car
    public float disappearDelay = 0.5f;    // Time before returning to pool
    public GameObject hitEffect;           // Optional particle effect

    private bool hit = false;
    private Collider col;
    private MeshRenderer[] renderers;

    void Awake()
    {
        // Ensure collider exists
        col = GetComponent<Collider>();
        if (col == null)
            col = gameObject.AddComponent<BoxCollider>();
        col.isTrigger = false;

        // Ensure Rigidbody exists
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;

        renderers = GetComponentsInChildren<MeshRenderer>();
    }

    /// <summary>Reset FatZombie when pulled from pool</summary>
    public void OnObjectSpawn()
    {
        hit = false;

        if (col != null) col.enabled = true;
        if (renderers != null)
        {
            foreach (var r in renderers)
                r.enabled = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hit) return;
        if (!collision.gameObject.CompareTag("Player") && !collision.gameObject.CompareTag("Car")) return;

        hit = true;

        // Deal damage to car
        CarHealth carHealth = collision.gameObject.GetComponent<CarHealth>();
        if (carHealth != null)
            carHealth.TakeDamage(carDamage);

        // Spawn hit effect via pool
        if (hitEffect != null)
            ObjectPool.Instance.SpawnFromPool(hitEffect, transform.position, Quaternion.identity);

        // Disable collider and renderer immediately
        // if (col != null) col.enabled = false;
        // if (renderers != null)
        // {
        //     foreach (var r in renderers)
        //         r.enabled = false;
        // }

        // Return to pool after delay
        Invoke(nameof(ReturnToPool), disappearDelay);
    }

    private void ReturnToPool()
    {
        ObjectPool.Instance.ReturnToPool(gameObject);
    }
}
