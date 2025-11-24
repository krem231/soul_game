using UnityEngine;
using Pathfinding;

public class FlyingEnemyMovement : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform player;

    [Header("Movement Settings")]
    public float speed = 120f;
    public float nextWaypointDistance = 3f;
    public float chaseRange = 35f;
    public LayerMask obstacleMask;

    [Header("Retreat Settings")]
    public float retreatRange = 5f;
    [Range(0f, 1f)]
    public float retreatSpeedMultiplier = 0.5f;

    [Header("Patrol Settings")]
    public float patrolRadius = 15f;
    public float patrolWaitTime = 2f;
    public float patrolDuration = 15f;

    [Header("Alert Settings")]
    public float alertRange = 20f;              // Bán kính báo động
    public LayerMask enemyLayer;                // Layer của quái

    private Path path;
    private int currentWaypoint = 0;

    private bool isChasing = false;
    private bool isPatrolling = false;
    private bool isReturning = false;
    private bool isRetreating = false;
    private bool hasAlertedOthers = false;

    private Vector2 startPosition;
    private Vector2 lastKnownPosition;
    private Vector2 currentPatrolTarget;
    private float totalPatrolTime = 0f;

    private Seeker seeker;
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 lastVelocity;

    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        startPosition = rb.position;
        animator = GetComponent<Animator>();
        InvokeRepeating("UpdatePath", 0f, 0.5f);
    }

    void UpdatePath()
    {
        if (player == null) return;
        if (!seeker.IsDone()) return;

        if (isRetreating) return;

        if (isChasing)
        {
            // Khi đuổi → bay tự do, không cần Pathfinding
            path = null;
        }
        else if (isReturning)
        {
            seeker.StartPath(rb.position, startPosition, OnPathComplete);
        }
        else if (isPatrolling)
        {
            seeker.StartPath(rb.position, currentPatrolTarget, OnPathComplete);
        }
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    void FixedUpdate()
    {
        if (player == null) return;

        float playerDistance = Vector2.Distance(rb.position, player.position);

        // === Lùi lại khi người chơi quá gần ===
        if (playerDistance <= retreatRange)
        {
            isRetreating = true;
            RetreatFromPlayer();
            UpdateAnimationAndDirection();
            return;
        }
        else
        {
            isRetreating = false;
        }

        // === Đuổi theo khi phát hiện ===
        if (playerDistance <= chaseRange)
        {
            if (!isChasing)
            {
                AlertNearbyEnemies(); // Báo động các quái khác
            }

            isChasing = true;
            isPatrolling = false;
            isReturning = false;

            ChasePlayerDirectly(); // Bay thẳng không cần path
        }
        else
        {
            if (isChasing)
            {
                lastKnownPosition = player.position;
                isChasing = false;
                isPatrolling = true;
                totalPatrolTime = 0f;
                SetRandomPatrolPoint();
                hasAlertedOthers = false; // reset lại khi mất dấu
            }

            if (isPatrolling || isReturning)
            {
                FollowPath();
            }
        }

        UpdateAnimationAndDirection();
    }

    // === Bay thẳng theo hướng người chơi (bỏ qua tường) ===
    void ChasePlayerDirectly()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        Vector2 force = direction * speed * Time.fixedDeltaTime;
        rb.AddForce(force);
        lastVelocity = rb.linearVelocity;
    }

    void FollowPath()
    {
        if (path == null || currentWaypoint >= path.vectorPath.Count) return;

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.fixedDeltaTime;
        rb.AddForce(force);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance) currentWaypoint++;

        lastVelocity = rb.linearVelocity;
    }

    void RetreatFromPlayer()
    {
        if (player == null) return;

        Vector2 direction = (rb.position - (Vector2)player.position).normalized;
        Vector2 force = direction * (speed * retreatSpeedMultiplier) * Time.fixedDeltaTime;
        rb.AddForce(force);

        lastVelocity = rb.linearVelocity;
    }

    // === Báo động quái xung quanh ===
    void AlertNearbyEnemies()
    {
        if (hasAlertedOthers) return; // tránh báo động liên tục
        hasAlertedOthers = true;

        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(transform.position, alertRange, enemyLayer);
        foreach (Collider2D enemy in nearbyEnemies)
        {
            if (enemy.gameObject != this.gameObject)
            {
                FlyingEnemyMovement other = enemy.GetComponent<FlyingEnemyMovement>();
                if (other != null)
                {
                    other.AlertToPlayer(player);
                }
            }
        }
    }

    // === Khi bị báo động ===
    public void AlertToPlayer(Transform detectedPlayer)
    {
        player = detectedPlayer;
        isChasing = true;
        isPatrolling = false;
        isReturning = false;
    }

    void UpdateAnimationAndDirection()
    {
        Vector2 velocity = rb.linearVelocity;
        bool isFlyingNow = velocity.sqrMagnitude > 0.1f;
        animator.SetBool("isFlying", isFlyingNow);

        if (isFlyingNow)
        {
            if (velocity.x > 0.1f)
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            else if (velocity.x < -0.1f)
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    void SetRandomPatrolPoint()
    {
        Vector2 randomDir = Random.insideUnitCircle * patrolRadius;
        currentPatrolTarget = lastKnownPosition + randomDir;
    }

    public void ReturnToStart()
    {
        isReturning = true;
        isPatrolling = false;
        isChasing = false;
        isRetreating = false;
        hasAlertedOthers = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, retreatRange);

        Gizmos.color = Color.cyan;
        Vector2 startPos = Application.isPlaying ? startPosition : (Vector2)transform.position;
        Gizmos.DrawWireSphere(startPos, patrolRadius);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, alertRange);
    }
}
