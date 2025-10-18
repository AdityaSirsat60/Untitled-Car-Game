using UnityEngine;

public class ZombieRagdoll : MonoBehaviour, IPooledObject
{
    [Header("Ragdoll Setup")]
    public Rigidbody[] ragdollBodies;
    public Collider[] ragdollColliders;
    public Animator animator;

    [Header("Hit Settings")]
    public float hitForce = 10f;

    [Header("Pooling")]
    public float ragdollLifetime = 5f;

    [Header("Zombie Sounds")]
    public AudioSource hitAudio;              // AudioSource for hits
    public AudioClip[] boneBreakVariants;     // Bone-breaking sounds
    public AudioClip[] splashVariants;        // Blood splash sounds
    private float lastHitTime;
    private float hitCooldown = 0.1f;         // Minimum delay between hits

    [Header("Idle Sound")]
    public AudioSource idleAudio;             // AudioSource for idle looping
    public AudioClip idleClip;                // Idle clip
    public float idleVolume = 1f;             // Idle volume
    public float idlePitchMin = 0.9f;         // Idle pitch min
    public float idlePitchMax = 1.1f;         // Idle pitch max

    private bool isRagdoll = false;
    private Coroutine ragdollCoroutine;

    /// <summary>Reset zombie when pulled from pool.</summary>
    public void Initialize()
    {
        isRagdoll = false;
        if (animator != null) animator.enabled = true;
        SetRagdoll(false);

        // Start idle sound if assigned
        if (idleAudio != null && idleClip != null)
        {
            idleAudio.clip = idleClip;
            idleAudio.volume = idleVolume;
            idleAudio.pitch = Random.Range(idlePitchMin, idlePitchMax);
            if (!idleAudio.isPlaying)
                idleAudio.Play();
        }
    }

    /// <summary>Trigger ragdoll, apply force, spawn blood effect, auto-return to pool.</summary>
    public void Hit(Vector3 forcePoint, Vector3 forceDir, GameObject bloodEffectPrefab = null)
    {
        if (isRagdoll) return;
        isRagdoll = true;

        if (animator != null) animator.enabled = false;

        SetRagdoll(true);

        // Stop idle sound when zombie is ragdolled
        if (idleAudio != null && idleAudio.isPlaying)
            idleAudio.Stop();

        // Apply force to all ragdoll bodies
        for (int i = 0; i < ragdollBodies.Length; i++)
            ragdollBodies[i].AddForceAtPosition(forceDir * hitForce, forcePoint, ForceMode.Impulse);

        // Play bone-breaking sound
        if (hitAudio != null && boneBreakVariants.Length > 0 && Time.time - lastHitTime > hitCooldown)
        {
            AudioClip chosenBoneClip = boneBreakVariants[Random.Range(0, boneBreakVariants.Length)];
            hitAudio.pitch = Random.Range(0.85f, 1.15f); // slight random pitch
            hitAudio.PlayOneShot(chosenBoneClip, 1f);
        }

        // Play splash sound
        if (hitAudio != null && splashVariants.Length > 0 && Time.time - lastHitTime > hitCooldown)
        {
            AudioClip chosenSplashClip = splashVariants[Random.Range(0, splashVariants.Length)];
            hitAudio.pitch = Random.Range(0.85f, 1.15f);
            hitAudio.PlayOneShot(chosenSplashClip, 1f);
        }

        lastHitTime = Time.time;

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
        Initialize(); // reset ragdoll state and idle sound
    }
}
