using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EnemyAnimation))]
[RequireComponent(typeof(EnemyRangedAttack))]
[RequireComponent(typeof(PathfindingEnemy))]
public class HeavyEnemy : MonoBehaviour
{
    [Header("Target & Ranges")]
    public Transform player;
    public float detectionRange = 18f;
    public float attackRange = 10f;
    public float dashRange = 2.5f;

    [Header("Movement")]
    public float moveSpeed = 2.2f;

    [Header("Decision")]
    public float decisionInterval = 0.2f;
    private float decisionTimer;

    private bool isAttacking = false;
    private bool isDashing = false;
    private bool canShoot = true;

    private EnemyAnimation anim;
    private EnemyRangedAttack ranged;
    private PathfindingEnemy pathAI;
    private Rigidbody2D rb;
    private EnemyDash dash;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<EnemyAnimation>();
        ranged = GetComponent<EnemyRangedAttack>();
        pathAI = GetComponent<PathfindingEnemy>();
        dash = GetComponent<EnemyDash>();

        anim.enemyType = EnemyAnimation.EnemyType.Heavy;

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        pathAI.player = player;
        pathAI.enabled = false;

        anim.SetIdle();
        decisionTimer = decisionInterval;
    }

    void Update()
    {
        if (player == null) return;

        decisionTimer -= Time.deltaTime;

        if (decisionTimer <= 0f && !isAttacking && !isDashing)
        {
            decisionTimer = decisionInterval;
            MakeDecision();
        }
    }

    private void MakeDecision()
    {
        float dist = Vector2.Distance(transform.position, player.position);

        if (dist > detectionRange)
        {
            StopMoving();
            anim.SetIdle();
            return;
        }

        if (isAttacking || isDashing)
        {
            StopMoving();
            return;
        }

        // DASH
        if (dist <= dashRange && dash != null && dash.CanDash())
        {
            StartCoroutine(DoDash());
            return;
        }

        // SHOOT
        if (dist <= attackRange)
        {
            StopMoving();
            StartCoroutine(HandleAttack());
            return;
        }

        // CHASE (same as BaseEnemy)
        Vector2 dir = (player.position - transform.position).normalized;
        rb.linearVelocity = dir * moveSpeed;

        anim.SetWalking();
    }

    private IEnumerator HandleAttack()
    {
        isAttacking = true;
        anim.SetShooting(true);

        if (canShoot)
        {
            canShoot = false;
            ranged.AttackPlayer();
        }

        yield return new WaitForSeconds(1.2f);

        anim.SetIdle();
        isAttacking = false;

        yield return new WaitForSeconds(1.0f);
        canShoot = true;
    }

    private IEnumerator DoDash()
    {
        isDashing = true;

        anim.SetDashing(true);
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(0.15f);

        dash?.DoDash(player.position);

        yield return new WaitForSeconds(dash != null ? dash.dashDuration : 0.5f);

        StopMoving();
        anim.SetIdle();
        isDashing = false;
    }

    private void StopMoving()
    {
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        pathAI.enabled = false;
    }

    public void FireProjectile()
    {
        ranged.AttackPlayer();
    }
}
