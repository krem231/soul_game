using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryUI : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Kéo InventoryPanel (GameObject UI trong Canvas) vào đây")]
    public GameObject inventoryPanel;  // ✅ Đổi tên rõ ràng hơn

    private PlayerInputActions inputActions;
    private InputAction inventoryAction;

    private void Awake()
    {
        inputActions = new PlayerInputActions();

        if (inventoryPanel == null)
        {
            Debug.LogError("❌ INVENTORY PANEL chưa được gán!");
            return;
        }
        RectTransform rect = inventoryPanel.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.sizeDelta = new Vector2(800, 600); 
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;

            Debug.Log($"🔧 Đã fix RectTransform sizeDelta: {rect.sizeDelta}");
        }
        Debug.Log($"✅ InventoryPanel đã được gán: '{inventoryPanel.name}'");


        // ✅ FIX RECTTRANSFORM
   
        inventoryPanel.SetActive(false);
    }
    private void OnEnable()
    {
        inventoryAction = inputActions.Player.Inventory;
        inputActions.Player.Enable();

        Debug.Log("✅ InventoryUI Input Actions enabled");
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

    private void OnDestroy()
    {
        inputActions?.Dispose();
    }

    void Update()
    {

        // ✅ Kiểm tra null trước khi dùng
        if (inventoryAction == null) return;

        if (inventoryAction.WasPressedThisFrame())
        {
            Debug.Log("📥 Tab pressed -> ToggleInventory()");
            ToggleInventory();
        }
    }

    void ToggleInventory()
    {
        if (inventoryPanel == null)
        {
            Debug.LogError("❌ Inventory Panel NULL!");
            return;
        }

        // Toggle
        bool newState = !inventoryPanel.activeSelf;
        inventoryPanel.SetActive(newState);

        Debug.Log($"✅ Inventory: {(newState ? "MỞ 📂" : "ĐÓNG 📁")}");
        Debug.Log($"   activeSelf: {inventoryPanel.activeSelf}");
        Debug.Log($"   activeInHierarchy: {inventoryPanel.activeInHierarchy}");

        // ✅ KIỂM TRA RECTTRANSFORM
    
        Canvas.ForceUpdateCanvases();
    }

    // ✅ Public methods để code khác có thể gọi
    public void OpenInventory()
    {
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(true);
            Debug.Log("📂 Inventory mở");
        }
    }

    public void CloseInventory()
    {
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
            Debug.Log("📁 Inventory đóng");
        }
    }

    public bool IsInventoryOpen()
    {
        return inventoryPanel != null && inventoryPanel.activeSelf;
    }
    public void RefreshInventory()
    {
        // Duyệt qua tất cả slot trong Inventory_mananegment
        var manager = Inventory_mananegment.Instance;
        if (manager == null || manager.slots == null) return;

        foreach (var slot in manager.slots)
        {
            if (slot != null)
                slot.RefreshSlot(); // gọi hàm cập nhật icon/text của slot
        }

        Debug.Log("🔄 UI Inventory đã được cập nhật!");
    }
}