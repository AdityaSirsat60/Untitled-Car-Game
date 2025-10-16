using UnityEngine;

public class ZombieRagdoll : MonoBehaviour,IPooledObject
{
    [Header("Ragdoll Setup")]
    public Rigidbody[] ragdollBodies;
    public Collider[] ragdollColliders;
    public Animator animator;

    [Header("Hit Settings")]
    public float hitForce = 10f;

    [Header("Pooling")]
    public float ragdollLifetime = 5f; // auto-return delay

    private bool isRagdoll = false;
    private Coroutine ragdollCoroutine;

    /// <summary>Reset zombie when pulled from pool.</summary>
    public void Initialize()
    {
        isRagdoll = false;
        if (animator != null) animator.enabled = true;
        SetRagdoll(false);
    }

    /// <summary>Trigger ragdoll, apply force, spawn blood effect, auto-return to pool.</summary>
    public void Hit(Vector3 forcePoint, Vector3 forceDir, GameObject bloodEffectPrefab = null)
    {
        if (isRagdoll) return;
        isRagdoll = true;

        if (animator != null) animator.enabled = false;

        SetRagdoll(true);

        for (int i = 0; i < ragdollBodies.Length; i++)
            ragdollBodies[i].AddForceAtPosition(forceDir * hitForce, forcePoint, ForceMode.Impulse);

        // Spawn pooled blood effect
        if (bloodEffectPrefab != null)
        {
            GameObject fx = ObjectPool.Instance.SpawnFromPool(bloodEffectPrefab, transform.position, Quaternion.identity);
        }

        // Return to pool after ragdollLifetime
        if (ragdollCoroutine != null) StopCoroutine(ragdollCoroutine);
        ragdollCoroutine = StartCoroutine(ReturnToPoolAfterDelay(ragdollLifetime));
    }

    private void SetRagdoll(bool enable)
    {
        for (int i = 0; i < ragdollBodies.Length; i++)
            ragdollBodies[i].isKinematic = !enable;

        for (int i = 0; i < ragdollColliders.Length; i++)
            ragdollColliders[i].enabled = enable;
    }

    private System.Collections.IEnumerator ReturnToPoolAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ObjectPool.Instance.ReturnToPool(gameObject);
    }

    public void OnObjectSpawn()
    {
        Initialize(); // reset ragdoll state when spawned from pool
    }
}
