using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EnemyAnimation))]
[RequireComponent(typeof(NormalAttack))]
[RequireComponent(typeof(EnemyDefense))]
public class BaseEnemy : MonoBehaviour
{
    [Header("General Settings")]
    public Transform player;
    public float detectionRange = 8f;
    public float attackRange = 1.3f;
    public float moveSpeed = 2f;
    public float decisionInterval = 0.2f;

    private float decisionTimer;
    private bool isAttacking = false;

    protected Rigidbody2D rb;
    protected EnemyAnimation anim;
    private NormalAttack normalAttack;
    private EnemyDefense defense;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<EnemyAnimation>();
        normalAttack = GetComponent<NormalAttack>();
        defense = GetComponent<EnemyDefense>();

        anim.enemyType = EnemyAnimation.EnemyType.Base;

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        anim?.SetIdle();
    }

    protected virtual void Update()
    {
        if (player == null) return;

        decisionTimer -= Time.deltaTime;
        if (decisionTimer <= 0f && !isAttacking)
        {
            decisionTimer = decisionInterval;
            MakeDecision();
        }

        // Nếu đang block, giữ mặt hướng về player
        if (defense.IsBlocking())
        {
            rb.linearVelocity = Vector2.zero;
            
        }
    }

    protected virtual void MakeDecision()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        // 🔹 Ngoài vùng phát hiện → Idle
        if (distance > detectionRange)
        {
            rb.linearVelocity = Vector2.zero;
            anim.SetIdle();
            defense.StopBlock(); // đảm bảo không giữ khiên mãi
            return;
        }

        // 🔹 Nếu player trong tầm block → gọi logic block
        if (!defense.IsBlocking() && distance <= defense.blockRange && Time.time >= defense.NextBlockTime)
        {
            StartCoroutine(HandleBlock());
            return;
        }

        // 🔹 Nếu đang block thì không tấn công, không di chuyển
        if (defense.IsBlocking())
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // 🔹 Trong tầm tấn công → tấn công
        if (distance <= attackRange)
        {
            rb.linearVelocity = Vector2.zero;
            StartCoroutine(HandleAttack());
            return;
        }

        // 🔹 Di chuyển đến player
        Vector2 dir = (player.position - transform.position).normalized;
        rb.linearVelocity = dir * moveSpeed;
        anim.SetWalking();
        
    }

    private IEnumerator HandleBlock()
    {
        defense.StartBlock();
        rb.linearVelocity = Vector2.zero;
        
        float blockTimer = defense.blockDuration;
        while (blockTimer > 0f)
        {
            float distance = Vector2.Distance(transform.position, player.position);
            if (distance > defense.blockRange + 0.5f) break;

            blockTimer -= Time.deltaTime;
            yield return null;
        }

        defense.StopBlock();
        defense.NextBlockTime = Time.time + defense.blockCooldown;
    }

    private IEnumerator HandleAttack()
    {
        isAttacking = true;
        rb.linearVelocity = Vector2.zero;
        
        anim.SetSlashing(true);

        yield return new WaitForSeconds(0.1f);

        normalAttack.TryAttack(attackRange);

        yield return new WaitForSeconds(0.6f);
        anim.SetIdle();
        isAttacking = false;
    }


}
