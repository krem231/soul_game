using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EnemyAnimation : MonoBehaviour
{
    public enum EnemyType
    {
        Base,
        Medium,
        Heavy
    }

    [Header("Enemy Type")]
    public EnemyType enemyType = EnemyType.Base;

    [Header("Target")]
    public Transform player;

    [Header("Flip Settings")]
    [Tooltip("Nếu sprite gốc hướng về trái, check box này")]
    public bool spriteDefaultFacingLeft = false;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    private static readonly int BlendParam = Animator.StringToHash("Blend");

    [Range(0f, 1f)]
    public float blendValue = 0f;

    // BASE
    private const float BASE_IDLE = 0f;
    private const float BASE_WALK = 0.25f;
    private const float BASE_BLOCK = 0.5f;
    private const float BASE_ATTACK = 0.75f;
    private const float BASE_CLOSE_ATTACK = 1f;

    // MEDIUM
    private const float MEDIUM_IDLE = 0f;
    private const float MEDIUM_WALK = 0.25f;
    private const float MEDIUM_BLOCK = 0.5f;
    private const float MEDIUM_SLASH = 0.75f;
    private const float MEDIUM_SHOOT = 1f;

    // HEAVY
    private const float HEAVY_IDLE = 0f;
    private const float HEAVY_WALK = 0.25f;
    private const float HEAVY_DASH = 0.5f;
    private const float HEAVY_FIRE = 0.75f;
    private const float HEAVY_WALK_SHOOT = 1f;


    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    void Update()
    {
        animator.SetFloat(BlendParam, blendValue);

        HandleFlip();
    }

    private void HandleFlip()
    {
        float vx = rb.linearVelocity.x;

        // Đang di chuyển → flip theo velocity
        if (Mathf.Abs(vx) > 0.05f)
        {
            bool shouldFaceRight = vx > 0f;

            // Nếu sprite gốc hướng trái: flipX = true khi nhìn phải
            // Nếu sprite gốc hướng phải: flipX = true khi nhìn trái
            if (spriteDefaultFacingLeft)
            {
                spriteRenderer.flipX = shouldFaceRight;
            }
            else
            {
                spriteRenderer.flipX = !shouldFaceRight;
            }
            return;
        }

    }
    public void SetIdle() => blendValue = GetBlend("idle");
    public void SetWalking() => blendValue = GetBlend("walk");
    public void SetBlocking(bool active = true) => blendValue = active ? GetBlend("block") : GetBlend("idle");
    public void SetSlashing(bool active = true) => blendValue = active ? GetBlend("slash") : GetBlend("idle");
    public void SetShooting(bool active = true) => blendValue = active ? GetBlend("shoot") : GetBlend("idle");
    public void SetDashing(bool active = true) => blendValue = active ? GetBlend("dash") : GetBlend("idle");


    private float GetBlend(string action)
    {
        switch (enemyType)
        {
            case EnemyType.Medium:
                return action switch
                {
                    "idle" => MEDIUM_IDLE,
                    "walk" => MEDIUM_WALK,
                    "block" => MEDIUM_BLOCK,
                    "slash" => MEDIUM_SLASH,
                    "shoot" => MEDIUM_SHOOT,
                    _ => MEDIUM_IDLE
                };

            case EnemyType.Heavy:
                return action switch
                {
                    "idle" => HEAVY_IDLE,
                    "walk" => HEAVY_WALK,
                    "dash" => HEAVY_DASH,
                    "shoot" => HEAVY_FIRE,
                    "walk_shoot" => HEAVY_WALK_SHOOT,
                    _ => HEAVY_IDLE
                };

            default:
                return action switch
                {
                    "idle" => BASE_IDLE,
                    "walk" => BASE_WALK,
                    "block" => BASE_BLOCK,
                    "slash" => BASE_ATTACK,
                    "attack" => BASE_ATTACK,
                    "close_attack" => BASE_CLOSE_ATTACK,
                    _ => BASE_IDLE
                };
        }
    }
}