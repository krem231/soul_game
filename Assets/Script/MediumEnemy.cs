using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EnemyAnimation))]
[RequireComponent(typeof(EnemyRangedAttack))]
public class MediumEnemy : MonoBehaviour
{
    [Header("Target & Detection")]
    public Transform player;
    public float detectionRange = 10f;
    public float closeAttackRange = 1.5f;
    public float blockRange = 3f;

    [Header("Movement Settings")]
    public float moveSpeed = 2f;

    [Header("Attack Settings")]
    public int meleeDamage = 10;
    public float meleeCooldown = 1.5f;
    public float meleeAnimDuration = 0.6f;

    [Header("References")]
    public Transform firePoint;
    private Rigidbody2D rb;
    private EnemyAnimation anim;
    private EnemyRangedAttack rangedAttack;
    private EnemyDefense defense;
    private bool canMelee = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<EnemyAnimation>();
        rangedAttack = GetComponent<EnemyRangedAttack>();
        defense = GetComponent<EnemyDefense>();

        anim.enemyType = EnemyAnimation.EnemyType.Medium;

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        if (rangedAttack != null && rangedAttack.firePoint == null)
            rangedAttack.firePoint = firePoint;

        anim.SetIdle();
    }

    void Update()
    {
        

        float distance = Vector2.Distance(transform.position, player.position);

        // Không còn anim.FaceTarget() – EnemyAnimation tự xử lý hướng

        // Cận chiến
        if (distance <= closeAttackRange)
        {
            rb.linearVelocity = Vector2.zero;
            anim.SetSlashing();

            if (canMelee)
                StartCoroutine(MeleeAttack());
            return;
        }

        // Bật khiên
        if (defense != null && distance <= blockRange)
        {
            rb.linearVelocity = Vector2.zero;
            anim.SetBlocking();
            if (!defense.IsBlocking())
                defense.StartBlock();
            return;
        }

        // Bắn tầm xa
        if (rangedAttack != null && distance <= rangedAttack.attackRange)
        {
            rb.linearVelocity = Vector2.zero;
            anim.SetShooting();
            return;
        }

        // Di chuyển tới player
        if (distance <= detectionRange)
        {
            Vector2 dir = (player.position - transform.position).normalized;
            rb.linearVelocity = dir * moveSpeed;
            anim.SetWalking();
            return;
        }

        // Đứng yên
        rb.linearVelocity = Vector2.zero;
        anim.SetIdle();
    }

    private IEnumerator MeleeAttack()
    {
        canMelee = false;
        yield return new WaitForSeconds(0.2f);

        Collider2D hit = Physics2D.OverlapCircle(transform.position, closeAttackRange, LayerMask.GetMask("Player"));
        if (hit != null)
        {
            PlayerController pc = hit.GetComponent<PlayerController>();
            if (pc != null)
                pc.TakeDamage(meleeDamage);
        }

        yield return new WaitForSeconds(meleeAnimDuration);
        yield return new WaitForSeconds(meleeCooldown);
        canMelee = true;
    }

    public void FireProjectile()
    {
        if (rangedAttack != null)
            rangedAttack.SendMessage("AttackPlayer", SendMessageOptions.DontRequireReceiver);
    }
}
