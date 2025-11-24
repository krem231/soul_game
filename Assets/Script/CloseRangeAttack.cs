using UnityEngine;
using System.Collections;
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(EnemyAnimation))]
public class CloseRangeAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public int damage = 10;
    public float attackCooldown = 1.0f;
    public float slashDuration = 0.5f;

    [Header("Target")]
    public string playerTag = "Player";

    private EnemyAnimation anim;
    private bool canAttack = true;
    private HealthBar playerHealthBar;
    private int currentHealth;

    void Start()
    {
        anim = GetComponent<EnemyAnimation>();

        CircleCollider2D col = GetComponent<CircleCollider2D>();
        col.isTrigger = true;

        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null)
        {
            playerHealthBar = player.GetComponentInChildren<HealthBar>();
            if (playerHealthBar != null)
                currentHealth = (int)playerHealthBar.slider.value;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!canAttack) return;
        if (!collision.CompareTag(playerTag)) return;

        StartCoroutine(PerformAttack());
    }

    private IEnumerator PerformAttack()
    {
        canAttack = false;

        // Bật animation Slash (Blend = 0.75)
        anim?.SetSlashing();

        yield return new WaitForSeconds(0.2f); // khớp khung tấn công

        // Gây sát thương
        if (playerHealthBar != null)
        {
            currentHealth -= damage;
            currentHealth = Mathf.Max(currentHealth, 0);
            playerHealthBar.SetHealth(currentHealth);
        }

        yield return new WaitForSeconds(slashDuration);

        // Quay lại idle
        anim?.SetIdle();

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}
