using UnityEngine;
using UnityEngine.UI;

public class enemy_health : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Health Bar")]
    public HealthBar healthBar;                 // slider object
    public RectTransform healthBarCanvas;       // canvas world space
    public Vector3 offset = new Vector3(0, 1.5f, 0);

    void Start()
    {
        currentHealth = maxHealth;
        if (healthBar != null)
            healthBar.SetMaxHealth(maxHealth);
    }

    void LateUpdate()
    {
        // 🎯 canvas luôn nằm trên enemy KHÔNG dùng camera
        if (healthBarCanvas != null)
        {
            healthBarCanvas.position = transform.position + offset;

            // FIX ⛔ NaN: đảm bảo không có phép chia/matrix lỗi
            if (float.IsNaN(healthBarCanvas.position.x))
            {
                healthBarCanvas.position = transform.position + offset;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        Debug.Log("Enemy died");
    }
}
