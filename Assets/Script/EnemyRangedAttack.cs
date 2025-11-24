using UnityEngine;
using System.Collections.Generic;

public class EnemyRangedAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRange = 10f;
    public float attackCooldown = 2f;
    public GameObject projectilePrefab;
    public float projectileSpeed = 8f;
    public int damage = 10;
    public Transform firePoint;

    [Header("Alert Settings")]
    public float alertRange = 15f;
    public float chaseRange = 25f;
    public LayerMask enemyLayer;

    [Header("Retreat Settings")]
    public bool enableRetreat = true;
    public float retreatDistance = 4f;
    public float retreatSpeed = 3f;

    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;
    private float nextAttackTime = 0f;
    private bool hasDetectedPlayer = false;

    // Danh sách các quái Ranged (để chia sẻ cảnh báo giữa chúng)
    private static readonly List<EnemyRangedAttack> allRangedEnemies = new List<EnemyRangedAttack>();

    void OnEnable() => allRangedEnemies.Add(this);
    void OnDisable() => allRangedEnemies.Remove(this);

    void Start()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("Player");
        if (obj != null)
            player = obj.transform;

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        if (firePoint == null) firePoint = transform;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // ✅ Phát hiện player
        if (distance <= chaseRange)
        {
            if (!hasDetectedPlayer)
            {
                hasDetectedPlayer = true;
                AlertNearbyEnemies(); // 🔹 Gọi báo động khi phát hiện player
            }
        }

        // ✅ Nếu có bật tính năng lùi lại và player quá gần
        if (enableRetreat && hasDetectedPlayer && distance < retreatDistance)
        {
            RetreatFromPlayer();
        }

        // ✅ Tấn công khi trong tầm
        else if (hasDetectedPlayer && distance <= attackRange && Time.time >= nextAttackTime)
        {
            AttackPlayer();
            nextAttackTime = Time.time + attackCooldown;
        }

        // ✅ Xoay mặt
        if (rb != null)
        {
            Vector2 velocity = rb.linearVelocity;
            if (Mathf.Abs(velocity.x) > 0.1f)
            {
                if (velocity.x > 0)
                    transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                else
                    transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
    }

    void RetreatFromPlayer()
    {
        if (rb == null || player == null) return;

        Vector2 directionAway = (transform.position - player.position).normalized;
        rb.linearVelocity = directionAway * retreatSpeed;

        if (animator != null)
            animator.SetBool("IsRetreating", true);
    }

    public void AttackPlayer()
    {
        if (projectilePrefab == null || player == null) return;

        Vector2 direction = (player.position - firePoint.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.Euler(0, 0, angle));
        Rigidbody2D rbProj = proj.GetComponent<Rigidbody2D>();
        if (rbProj != null)
            rbProj.linearVelocity = direction * projectileSpeed;

        Collider2D enemyCol = GetComponent<Collider2D>();
        Collider2D projCol = proj.GetComponent<Collider2D>();
        if (enemyCol != null && projCol != null)
            Physics2D.IgnoreCollision(projCol, enemyCol);

        Projectile projectile = proj.GetComponent<Projectile>();
        if (projectile != null)
            projectile.damage = damage;

        if (animator != null)
        {
            animator.SetTrigger("Attack");
            animator.SetBool("IsRetreating", false);
        }
    }

    // 🔹 Báo động khi phát hiện player
    void AlertNearbyEnemies()
    {
        // 1️⃣ Báo cho các quái Ranged khác
        foreach (var enemy in allRangedEnemies)
        {
            if (enemy == null || enemy == this) continue;
            float dist = Vector2.Distance(transform.position, enemy.transform.position);
            if (dist <= alertRange)
                enemy.OnAlerted(player);
        }

        // 2️⃣ Báo cho các quái bay có PathfindingEnemy trong vùng
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, alertRange, enemyLayer);
        foreach (var hit in hits)
        {
            PathfindingEnemy flying = hit.GetComponent<PathfindingEnemy>();
            if (flying != null)
            {
                flying.SendMessage("OnAlerted", player, SendMessageOptions.DontRequireReceiver);
            }
        }

        Debug.Log($"[{name}] alerted nearby enemies!");
    }

    public void OnAlerted(Transform detectedPlayer)
    {
        if (hasDetectedPlayer) return;
        hasDetectedPlayer = true;
        player = detectedPlayer;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red; Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(transform.position, alertRange);
        Gizmos.color = Color.cyan; Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.green; Gizmos.DrawWireSphere(transform.position, retreatDistance);
    }
}
