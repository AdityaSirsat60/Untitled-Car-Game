using UnityEngine;

public class CarCollision : MonoBehaviour
{
    public float hitForce = 5f;
    private CarHealth carHealth;
    public GameObject bloodPrefab;

    void Start()
    {
        carHealth = FindObjectOfType<CarHealth>();
    }

    private void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Zombie"))
        {
            carHealth.TakeDamage(100);
        ZombieRagdoll zr = other.GetComponent<ZombieRagdoll>();
        if (zr != null)
        {
            // Calculate force direction away from car
            Vector3 dir = (other.transform.position - transform.position).normalized;
                dir.y = 1f; // upward push
            zr.Hit(other.transform.position, dir,bloodPrefab);
            
        }
    }
}

}
