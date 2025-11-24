using UnityEngine;
using System.Collections;

[RequireComponent(typeof(EnemyAnimation))]
public class NormalAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRange = 1.3f;
    public int attackDamage = 10;
    public float attackCooldown = 1.5f;

    [Header("Layer Settings")]
    public LayerMask playerLayer;

    private float nextAttackTime = 0f;
    private EnemyAnimation anim;
    private bool isAttacking;

    void Start()
    {
        anim = GetComponent<EnemyAnimation>();
    }

    // TryAttack() mặc định dùng attackRange nội bộ
    public void TryAttack()
    {
        TryAttack(attackRange);
    }

    // Cho phép BaseEnemy truyền vào range (để đồng bộ với attackRange của AI)
    public void TryAttack(float range)
    {
        if (Time.time < nextAttackTime || isAttacking) return;

        // Nếu player không trong khoảng range thì bỏ
        Collider2D hitCheck = Physics2D.OverlapCircle(transform.position, range, playerLayer);
        if (hitCheck == null) return;

        nextAttackTime = Time.time + attackCooldown;
        StartCoroutine(PerformAttackAfterDelay(0.25f, range));
    }

    private IEnumerator PerformAttackAfterDelay(float delay, float range)
    {
        isAttacking = true;
        anim?.SetSlashing(true);

        yield return new WaitForSeconds(delay);

        Collider2D hit = Physics2D.OverlapCircle(transform.position, range, playerLayer);
        if (hit != null)
        {
            PlayerController player = hit.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(attackDamage);
                Debug.Log($"[{name}] Normal attack hit player for {attackDamage} damage!");
            }
        }

        yield return new WaitForSeconds(0.4f);
        anim?.SetIdle();
        isAttacking = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
