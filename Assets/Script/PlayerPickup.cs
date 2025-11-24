using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPickup : MonoBehaviour
{
    public float pickupRange = 1.5f;
    public LayerMask itemLayer;

    [SerializeField] private Inventory_mananegment inventory; 

    private PlayerInputActions inputActions;

    void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.AddItem.performed += OnPickup;
    }

    void OnDisable()
    {
        inputActions.Player.AddItem.performed -= OnPickup;
        inputActions.Player.Disable();
    }

    void Start()
    {
        // fallback: nếu chưa gán, sẽ auto tìm
        if (inventory == null)
        {
            inventory = FindObjectOfType<Inventory_mananegment>();
            if (inventory == null)
            {
                Debug.LogError("❌ Không tìm thấy Inventory_mananegment trong Scene!");
            }
        }
    }

    private void OnPickup(InputAction.CallbackContext context)
    {
        Debug.Log("🎯 Nhấn E -> OnPickup gọi");
        TryPickup();
    }

    void TryPickup()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, pickupRange, itemLayer);
        Debug.Log($"🔍 Số object trong phạm vi pickup: {hits.Length}");

        foreach (Collider2D hit in hits)
        {
            Debug.Log($"👉 Found: {hit.name}");
            Item item = hit.GetComponent<Item>();

            if (item != null && item.itemData != null && inventory != null)
            {
                inventory.Add(item.itemData);
                Debug.Log($"✅ Nhặt {item.itemData.item_name}");
                Destroy(item.gameObject);
                break;
            }
            else
            {
                Debug.LogError("❌ Pickup failed: " +
                               (inventory == null ? "inventory null " : "") +
                               (item == null ? "item null " : "") +
                               (item != null && item.itemData == null ? "itemData null " : ""));
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}
