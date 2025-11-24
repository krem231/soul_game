using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class inventory_slot : MonoBehaviour, IPointerClickHandler
{
    [Header("UI References")]
    public Image iconUI;
    public TextMeshProUGUI quantityText;

    [Header("Slot Data")]
    private Scriptable_object currentItem;
    private int currentQuantity = 0;

    [HideInInspector] public int slotIndex;
    [HideInInspector] public bool isSpecialSlot = false;

    private Inventory_mananegment inventoryManager;
    private bool isInitialized = false;

    void Awake()
    {
        Debug.Log($"[Slot {gameObject.name}] Awake - Đang tìm references...");

        // Tự động tìm Icon nếu chưa gán
        if (iconUI == null)
        {
            Transform iconTransform = transform.Find("Icon");
            if (iconTransform != null)
            {
                iconUI = iconTransform.GetComponent<Image>();
                Debug.Log($"[Slot {gameObject.name}] Tìm thấy Icon tự động");
            }
            else
            {
                Debug.LogError($"❌ Slot '{gameObject.name}' KHÔNG có child GameObject tên 'Icon'!");
            }
        }

        // Tự động tìm Quantity nếu chưa gán
        if (quantityText == null)
        {
            Transform qtyTransform = transform.Find("Quantity");
            if (qtyTransform != null)
            {
                quantityText = qtyTransform.GetComponent<TextMeshProUGUI>();
            }
        }

        // Kiểm tra cuối cùng
        if (iconUI == null)
        {
            Debug.LogError($"❌ Slot '{gameObject.name}' iconUI vẫn NULL sau Awake!");
        }
        else
        {
            Debug.Log($"✅ Slot '{gameObject.name}' iconUI OK: {iconUI.gameObject.name}");
        }

        // ⚠️ KHÔNG gọi ClearSlot() ở đây nữa
        // ClearSlot();  ❌ Bỏ dòng này đi để không reset icon mỗi khi Awake chạy
    }


    void Start()
    {
        // ✅ Lấy reference Inventory Manager trong Start
        inventoryManager = Inventory_mananegment.Instance;
        if (inventoryManager == null)
        {
            inventoryManager = FindObjectOfType<Inventory_mananegment>();
        }

        if (inventoryManager == null)
        {
            Debug.LogError($"❌ Slot {slotIndex}: Không tìm thấy Inventory_mananegment!");
        }

        isInitialized = true;
    }

    public bool IsEmpty()
    {
        return currentItem == null;
    }

    public Scriptable_object GetItem()
    {
        return currentItem;
    }

    public int GetQuantity()
    {
        return currentQuantity;
    }

    public void AddItem(Scriptable_object newItem)
    {
        Debug.Log($"🔵 [Slot {slotIndex}] AddItem được gọi với item: {newItem?.item_name}");

        if (newItem == null)
        {
            Debug.LogWarning("⚠️ Không thể thêm item NULL vào slot!");
            return;
        }

        if (IsEmpty())
        {
            Debug.Log($"🔵 [Slot {slotIndex}] Slot trống, đang gọi UpdateSlot...");
            UpdateSlot(newItem, 1);
            Debug.Log($"✅ Slot {slotIndex} nhận item: {newItem.item_name}");
        }
        else if (currentItem == newItem)
        {
            IncreaseQuantity(1);
            Debug.Log($"✅ Slot {slotIndex} tăng số lượng {newItem.item_name}: {currentQuantity}");
        }
        else
        {
            Debug.LogWarning($"⚠️ Slot {slotIndex} đã có item khác!");
        }
    }

    public void UpdateSlot(Scriptable_object newItem, int newQuantity)
    {
        Debug.Log($"🟢 [Slot {slotIndex}] UpdateSlot BẮT ĐẦU - item: {newItem?.item_name}, qty: {newQuantity}");

        currentItem = newItem;
        currentQuantity = Mathf.Max(0, newQuantity);

        // ✅ Kiểm tra iconUI an toàn hơn
        if (iconUI == null)
        {
            Debug.LogError($"❌ [Slot {slotIndex}] iconUI NULL trong UpdateSlot!");
            // ✅ Thử tìm lại
            Transform iconTransform = transform.Find("Icon");
            if (iconTransform != null)
            {
                iconUI = iconTransform.GetComponent<Image>();
            }

            if (iconUI == null)
            {
                Debug.LogError($"❌ [Slot {slotIndex}] Không thể tìm thấy iconUI!");
                return;
            }
        }

        Debug.Log($"🟢 [Slot {slotIndex}] iconUI OK, đang update sprite...");

        if (newItem != null)
        {
            // ✅ Kiểm tra itemIcon (chú ý: trong Scriptable_object có thể là item_icon hoặc itemIcon)
            Sprite iconSprite = newItem.itemIcon; // hoặc newItem.item_icon nếu tên property khác

            if (iconSprite == null)
            {
                Debug.LogError($"❌ [Slot {slotIndex}] Item '{newItem.item_name}' không có itemIcon!");
                iconUI.sprite = null;
                iconUI.enabled = false;
                iconUI.color = new Color(1, 1, 1, 0); // Trong suốt
            }
            else
            {
                iconUI.sprite = iconSprite;
                iconUI.color = Color.white;
                iconUI.enabled = true;

                // ✅ Force Canvas rebuild để đảm bảo hiển thị
                Canvas.ForceUpdateCanvases();

                Debug.Log($"✅ [Slot {slotIndex}] ĐÃ SET SPRITE: {iconSprite.name}");
            }
        }
        else
        {
            iconUI.sprite = null;
            iconUI.enabled = false;
            iconUI.color = new Color(1, 1, 1, 0);
            Debug.Log($"🗑️ [Slot {slotIndex}] Cleared");
        }

        // ✅ Update quantity text với null check
        if (quantityText != null)
        {
            if (currentQuantity > 1)
            {
                quantityText.text = currentQuantity.ToString();
                quantityText.enabled = true;
            }
            else
            {
                quantityText.text = "";
                quantityText.enabled = false;
            }
        }
        else if (currentQuantity > 1)
        {
            Debug.LogWarning($"⚠️ [Slot {slotIndex}] quantityText NULL, không thể hiển thị số lượng!");
        }

        Debug.Log($"🟢 [Slot {slotIndex}] UpdateSlot HOÀN TẤT");
    }

    public void ClearSlot()
    {
        currentItem = null;
        currentQuantity = 0;

        if (iconUI != null)
        {
            iconUI.sprite = null;
            iconUI.enabled = false;
            iconUI.color = new Color(1, 1, 1, 0);
        }

        if (quantityText != null)
        {
            quantityText.text = "";
            quantityText.enabled = false;
        }
    }

    public void IncreaseQuantity(int amount)
    {
        if (currentItem == null) return;
        UpdateSlot(currentItem, currentQuantity + Mathf.Max(1, amount));
    }

    public void DecreaseQuantity(int amount)
    {
        if (currentItem == null) return;
        int newQty = currentQuantity - Mathf.Max(1, amount);

        if (newQty > 0)
        {
            UpdateSlot(currentItem, newQty);
        }
        else
        {
            ClearSlot();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isInitialized)
        {
            Debug.LogWarning($"⚠️ Slot {slotIndex} chưa được khởi tạo!");
            return;
        }

        if (IsEmpty())
        {
            Debug.Log($"ℹ️ Slot {slotIndex} trống, bỏ qua click");
            return;
        }

        if (inventoryManager == null)
        {
            Debug.LogError($"❌ Slot {slotIndex}: Inventory Manager NULL!");
            // ✅ Thử tìm lại
            inventoryManager = Inventory_mananegment.Instance;
            if (inventoryManager == null)
            {
                return;
            }
        }

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log($"🖱️ Left-click slot {slotIndex}: {currentItem.item_name}");
            inventoryManager.UseItem(slotIndex, isSpecialSlot);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log($"🖱️ Right-click slot {slotIndex}: {currentItem.item_name}");
            // TODO: Thêm chức năng right-click (drop, info, etc.)
        }
    }

    public void RefreshSlot()
    {
        if (iconUI == null)
        {
            Debug.LogWarning($"⚠️ [Slot {slotIndex}] iconUI NULL trong RefreshSlot!");
            return;
        }

        if (currentItem != null)
        {
            Sprite iconSprite = currentItem.itemIcon;

            if (iconSprite != null)
            {
                iconUI.sprite = iconSprite;
                iconUI.enabled = true;
                iconUI.color = Color.white;
            }
            else
            {
                Debug.LogWarning($"⚠️ [Slot {slotIndex}] Item {currentItem.item_name} không có icon!");
                iconUI.sprite = null;
                iconUI.enabled = false;
            }
        }
        else
        {
            iconUI.sprite = null;
            iconUI.enabled = false;
            iconUI.color = new Color(1, 1, 1, 0);
        }

        // ✅ Force Canvas update
        Canvas.ForceUpdateCanvases();
    }
}