using UnityEngine;
using System.Collections;

public class Inventory_mananegment : MonoBehaviour
{
    public static Inventory_mananegment Instance { get; private set; }
    public inventory_slot[] slots;
    public Scriptable_object[] startingItems;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("⚠️ Đã có Inventory_mananegment khác, hủy duplicate");
            Destroy(gameObject);
            return;
        }

        Instance = this;

        Debug.Log("✅ Inventory_mananegment Awake được gọi");
        if (slots == null || slots.Length == 0)
        {
            Debug.LogError("❌ Slots chưa được gán trong Inspector!");
            return;
        }

        Debug.Log($"✅ Inventory có {slots.Length} slots");

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null)
            {
                slots[i].slotIndex = i;
                slots[i].isSpecialSlot = false;
            }
        }
    }

    void Start()
    {
        if (startingItems != null && startingItems.Length > 0)
        {
            StartCoroutine(AddStartingItems());
        }
    }

    private IEnumerator AddStartingItems()
    {
        // ✅ Đợi 1 frame để đảm bảo UI đã sẵn sàng
        yield return null;

        for (int i = 0; i < startingItems.Length && i < slots.Length; i++)
        {
            if (startingItems[i] != null && slots[i] != null)
            {
                slots[i].AddItem(startingItems[i]);
                Debug.Log($"✅ Đã thêm starting item: {startingItems[i].item_name}");
            }
        }

        Debug.Log("✅ Đã thêm tất cả starting items");
    }

    public bool Add(Scriptable_object newItem)
    {
        Debug.Log($"🔍 Đang thêm item vào inventory: {newItem?.item_name ?? "NULL"}");

        if (newItem == null)
        {
            Debug.LogWarning("⚠️ Không thể thêm item NULL!");
            return false;
        }

        if (slots == null || slots.Length == 0)
        {
            Debug.LogError("❌ Inventory không có slots!");
            return false;
        }

        // ✅ Tìm slot trống đầu tiên
        foreach (inventory_slot slot in slots)
        {
            if (slot != null && slot.IsEmpty())
            {
                slot.AddItem(newItem);
                Debug.Log($"✅ Đã thêm {newItem.item_name} vào slot {slot.slotIndex}");

                // ✅ Force refresh slot UI
                slot.RefreshSlot();

                return true;
            }
        }

        Debug.LogWarning("⚠️ Inventory đầy! Không thể thêm item.");
        return false;
    }

    // ✅ Dùng item từ slot
    public void UseItem(int slotIndex, bool isSpecial)
    {
        if (slotIndex < 0 || slotIndex >= slots.Length)
        {
            Debug.LogWarning($"⚠️ Slot index {slotIndex} không hợp lệ!");
            return;
        }

        inventory_slot slot = slots[slotIndex];

        if (slot == null || slot.IsEmpty())
        {
            Debug.LogWarning($"⚠️ Slot {slotIndex} trống hoặc NULL!");
            return;
        }

        Scriptable_object item = slot.GetItem();

        if (item == null)
        {
            Debug.LogWarning($"⚠️ Item trong slot {slotIndex} là NULL!");
            return;
        }

        Debug.Log($"🎯 Đang dùng item: {item.item_name}");

        slot.DecreaseQuantity(1);

        Debug.Log($"✅ Đã dùng {item.item_name}");
    }

    // ✅ Phương thức tiện ích: Kiểm tra inventory có đầy không
    public bool IsFull()
    {
        if (slots == null || slots.Length == 0) return true;

        foreach (inventory_slot slot in slots)
        {
            if (slot != null && slot.IsEmpty())
            {
                return false;
            }
        }
        return true;
    }

    // ✅ Phương thức tiện ích: Đếm số slot trống
    public int GetEmptySlotCount()
    {
        if (slots == null || slots.Length == 0) return 0;

        int count = 0;
        foreach (inventory_slot slot in slots)
        {
            if (slot != null && slot.IsEmpty())
            {
                count++;
            }
        }
        return count;
    }

    // ✅ Xóa toàn bộ inventory
    public void ClearAll()
    {
        if (slots == null) return;

        foreach (inventory_slot slot in slots)
        {
            if (slot != null)
            {
                slot.ClearSlot();
            }
        }

        Debug.Log("🗑️ Đã xóa toàn bộ inventory");
    }

    public void RefreshAllSlots()
    {
        if (slots == null) return;

        foreach (inventory_slot slot in slots)
        {
            if (slot != null)
            {
                slot.RefreshSlot();
            }
        }

        // ✅ Force Canvas update
        Canvas.ForceUpdateCanvases();
    }
}