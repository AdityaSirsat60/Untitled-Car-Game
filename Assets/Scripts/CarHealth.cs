using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CarHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("UI")]
    public Slider healthSlider;        // Assign your Slider

    [Header("Explosion Effect & Sound")]
    public GameObject explosionPrefab; // Explosion particle prefab
    public AudioClip explosionClip;    // Explosion sound clip
    public float explosionVolume = 1f; // Volume for explosion sound

    void Start()
    {
        currentHealth = maxHealth;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();

        if (currentHealth <= 0)
            Die();
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
    }

    private void Die()
    {
        // Spawn explosion particle effect
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        // Play explosion sound via AudioManager
        if (AudioManager.Instance != null && explosionClip != null)
        {
            AudioManager.Instance.PlaySound(explosionClip, transform.position, explosionVolume);
        }

        // Disable the car
        this.gameObject.SetActive(false);

        // Reload the scene after a delay
        Invoke(nameof(Delay), 5f);
    }

    void Delay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
