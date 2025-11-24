using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EnemyAnimation))]
public class EnemyDash : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashSpeed = 12f;
    public float dashDuration = 0.35f;
    public float dashCooldown = 1.2f;
    public float stunDuration = 1.5f;
    public float dashDistance = 5f;

    private bool isDashing = false;
    private float nextDashTime = 0f;

    private Rigidbody2D rb;
    private Animator animator;
    private Collider2D col;
    private Transform player;
    private EnemyAnimation anim;

    public bool IsDashing => isDashing;
    public float NextDashTime => nextDashTime;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        anim = GetComponent<EnemyAnimation>();

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    public bool CanDash()
    {
        return !isDashing && Time.time >= nextDashTime;
    }

    public void DoDash(Vector2 playerPosition)
    {
        if (!CanDash()) return;
        StartCoroutine(DashBackRoutine(playerPosition));
    }

    private IEnumerator DashBackRoutine(Vector2 playerPosition)
    {
        isDashing = true;
        nextDashTime = Time.time + dashCooldown;

        // Trigger animation
        if (animator != null)
        {
            animator.ResetTrigger("Dash");
            animator.SetTrigger("Dash");
        }

        // Blend Tree animation (HeavyEnemy -> dùng blend 0.5)
        if (anim != null)
            anim.SetDashing(true);

        Vector2 directionAway = ((Vector2)transform.position - playerPosition).normalized;

        if (rb.bodyType != RigidbodyType2D.Dynamic)
            rb.bodyType = RigidbodyType2D.Dynamic;

        if (col != null)
            col.enabled = false;

        int enemyLayer = LayerMask.NameToLayer("Enemy");
        int playerLayer = LayerMask.NameToLayer("Player");
        if (enemyLayer >= 0 && playerLayer >= 0)
            Physics2D.IgnoreLayerCollision(enemyLayer, playerLayer, true);

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(directionAway * dashSpeed, ForceMode2D.Impulse);

        yield return new WaitForSeconds(dashDuration);

        rb.linearVelocity = Vector2.zero;

        if (col != null)
            col.enabled = true;

        if (enemyLayer >= 0 && playerLayer >= 0)
            Physics2D.IgnoreLayerCollision(enemyLayer, playerLayer, false);

        yield return new WaitForSeconds(0.02f);

        // Quay lại Idle animation
        if (anim != null)
            anim.SetIdle();

        isDashing = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isDashing) return;
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerStun stun = collision.gameObject.GetComponent<PlayerStun>();
            if (stun == null) stun = collision.gameObject.AddComponent<PlayerStun>();
            stun.ApplyStun(stunDuration);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && CanDash())
        {
            DoDash(other.transform.position);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && CanDash())
        {
            DoDash(other.transform.position);
        }
    }
}
