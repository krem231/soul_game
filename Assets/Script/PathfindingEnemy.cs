using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;

public class PathfindingEnemy : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform player;

    [Header("Movement Settings")]
    public float speed = 200f;
    public float nextWaypointDistance = 3f;
    public float chaseRange = 20f;
    public LayerMask obstacleMask;

    [Header("Patrol Settings")]
    public float patrolRadius = 10f;
    public float patrolWaitTime = 2f;
    public float patrolDuration = 10f;

    private Path path;
    private int currentWaypoint = 0;
    private bool reachedEndOfPath = false;
    private bool isChasing = false;
    private bool isPatrolling = false;
    private bool isReturning = false;

    private Vector2 startPosition;
    private Vector2 lastKnownPosition;
    private Vector2 currentPatrolTarget;
    private float patrolWaitTimer = 0f;
    private float totalPatrolTime = 0f;

    private Seeker seeker;
    private Rigidbody2D rb;

    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        startPosition = rb.position;
        InvokeRepeating("UpdatePath", 0f, 0.5f);
    }

    void UpdatePath()
    {
        if (player == null) return;

        if (seeker.IsDone())
        {
            if (isChasing)
            {
                seeker.StartPath(rb.position, player.position, OnPathComplete);
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

        if (playerDistance > chaseRange)
        {
            if (isChasing)
            {

                lastKnownPosition = player.position;
                isChasing = false;
                isPatrolling = true;
                isReturning = false;
                totalPatrolTime = 0f;
                SetRandomPatrolPoint();
            }

            if (isPatrolling)
            {
                totalPatrolTime += Time.fixedDeltaTime;
                if (totalPatrolTime >= patrolDuration)
                {
                    isPatrolling = false;
                    isReturning = true;
                }
                else
                {
                    HandlePatrol();
                }
            }

            if (isReturning)
            {
                HandleReturn();
            }
            return;
        }

        if (!isChasing)
        {
            Vector2 dirToPlayer = (player.position - transform.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dirToPlayer, chaseRange, obstacleMask);

            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                isChasing = true;
                isPatrolling = false;
                isReturning = false;
            }
            else if (!isPatrolling && !isReturning)
            {
                rb.linearVelocity = Vector2.zero;
                return;
            }
        }

        if (isChasing)
        {
            if (path == null) return;

            if (currentWaypoint >= path.vectorPath.Count)
            {
                reachedEndOfPath = true;
                return;
            }
            else
            {
                reachedEndOfPath = false;
            }

            Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
            Vector2 force = direction * speed * Time.fixedDeltaTime;
            rb.AddForce(force);

            // debug path của a*
            Vector2 dirToPlayer = (player.position - transform.position).normalized;
            Debug.DrawRay(transform.position, dirToPlayer * chaseRange, Color.green);

            float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
            if (distance < nextWaypointDistance)
            {
                currentWaypoint++;
            }
        }
    }

    // tuần tra
    void HandlePatrol()
    {
        if (path == null) return;

        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            rb.linearVelocity = Vector2.zero;

            patrolWaitTimer += Time.fixedDeltaTime;
            if (patrolWaitTimer >= patrolWaitTime)
            {
                patrolWaitTimer = 0f;
                SetRandomPatrolPoint();
            }
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.fixedDeltaTime;
        rb.AddForce(force);

        // debug ray khi tuần tra 
        Debug.DrawLine(transform.position, currentPatrolTarget, Color.cyan);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }

    void SetRandomPatrolPoint()
    {

        Vector2 randomDir = Random.insideUnitCircle * patrolRadius;
        currentPatrolTarget = lastKnownPosition + randomDir;
    }
    // quay về chỗ cũ nếu không tìm thấy player
    void HandleReturn()
    {
        if (path == null) return;

        float distanceToStart = Vector2.Distance(rb.position, startPosition);
        if (distanceToStart < nextWaypointDistance)
        {
            rb.linearVelocity = Vector2.zero;
            isReturning = false;
            return;
        }

        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            rb.linearVelocity = Vector2.zero;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.fixedDeltaTime;
        rb.AddForce(force);

        Debug.DrawLine(transform.position, startPosition, Color.yellow);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        Gizmos.color = Color.blue;
        Vector2 startPos = Application.isPlaying ? startPosition : (Vector2)transform.position;
        Gizmos.DrawWireSphere(startPos, 1f);

        if (isPatrolling)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(lastKnownPosition, patrolRadius);
            Gizmos.DrawSphere(currentPatrolTarget, 0.5f);
        }
    }
}