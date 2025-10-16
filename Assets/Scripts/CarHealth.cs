using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CarHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    public Slider healthSlider;       // Assign your Slider
    public GameObject explosionPrefab; // Assign your explosion particle prefab

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
        // Spawn explosion effect
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        // Destroy the car
        // Destroy(gameObject);
        this.gameObject.SetActive(false);
        Invoke(nameof(Delay), 2.5f);
    }

    void Delay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
