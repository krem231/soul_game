using UnityEngine;

public class EnemyAttack1 : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRange = 3f;          // phạm vi phát hiện player
    public float attackCooldown = 2f;      // thời gian chờ giữa các cú nhảy
    public float jumpForce = 6f;           // lực nhảy về phía player
    public int damage = 10;                // damage khi chạm player

    private Rigidbody2D rb;
    private Transform player;
    private float nextAttackTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Tìm Player theo tag
        GameObject obj = GameObject.FindGameObjectWithTag("Player");
        if (obj != null)
        {
            player = obj.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Nếu player trong phạm vi và đã hết cooldown thì nhảy
        if (distanceToPlayer <= attackRange && Time.time >= nextAttackTime)
        {
            JumpAttack();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    void JumpAttack()
    {
        if (player == null) return;

        // Hướng từ slime → player
        Vector2 direction = (player.position - transform.position).normalized;

        // Reset vận tốc trước khi AddForce để nhảy ổn định
        rb.linearVelocity = Vector2.zero;

        // Nhảy bật về phía player
        rb.AddForce(direction * jumpForce, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Khi chạm Player → gây damage
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.TakeDamage(damage);
            }
        }
    }

    // Vẽ phạm vi tấn công trong Scene
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
