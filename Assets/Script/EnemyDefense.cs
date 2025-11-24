using UnityEngine;

public class EnemyDefense : MonoBehaviour
{
    [Header("Block Settings")]
    public float blockDuration = 1.5f;   // thời gian giữ khiên
    public float blockRange = 3f;        // tầm kích hoạt khiên
    public float blockCooldown = 1.5f;   // thời gian hồi giữa hai lần block

    [HideInInspector] public Transform player;
    [HideInInspector] public float NextBlockTime = 0f; // để BaseEnemy kiểm soát cooldown

    private bool isBlocking = false;
    private EnemyAnimation anim;
    private Rigidbody2D rb;

    void Start()
    {
        anim = GetComponent<EnemyAnimation>();
        rb = GetComponent<Rigidbody2D>();

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    /// <summary>
    /// Bắt đầu block – ngừng di chuyển, bật animation
    /// </summary>
    public void StartBlock()
    {
        if (isBlocking) return;

        isBlocking = true;
        rb.linearVelocity = Vector2.zero;
        anim?.SetBlocking(true);
        Debug.Log($"[{name}] bắt đầu block!");
    }

    /// <summary>
    /// Kết thúc block – trở lại idle, reset trạng thái
    /// </summary>
    public void StopBlock()
    {
        if (!isBlocking) return;

        isBlocking = false;
        anim?.SetIdle();
        Debug.Log($"[{name}] dừng block!");
    }

    /// <summary>
    /// Kiểm tra có đang block không
    /// </summary>
    public bool IsBlocking() => isBlocking;

    /// <summary>
    /// Giảm sát thương nhận vào nếu đang block
    /// </summary>
    public int ModifyDamage(int damage)
    {
        if (isBlocking)
        {
            Debug.Log($"[{name}] đã chặn {damage} sát thương!");
            return 0;
        }
        return damage;
    }
}
