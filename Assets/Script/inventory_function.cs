using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
public class inventory_function : MonoBehaviour
{
    public GameObject InventoryMenu;
    private bool activate;
    [System.Serializable]
    public class InventorySlotData
    {
        public Scriptable_object item;
        public int amount;

        public void ClearSlot()
        {
            item = null;
            amount = 0;
        }
    }

    public static inventory_function instance;
    public int slot = 20;
    public InventorySlotData[] bagslot;
    public InventorySlotData[] specialslot;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }

        bagslot = new InventorySlotData[slot];
        specialslot = new InventorySlotData[2];
    }

    public bool equip_item(Scriptable_object item)
    {
    
        if (item.itemType == ItemType.Weapon ||
    item.itemType == ItemType.Gem ||
    item.itemType == ItemType.UpgradeItem)
        {
            for (int i = 0; i < specialslot.Length; i++)
            {
                if (specialslot[i] == null || specialslot[i].item == null)
                {
                    specialslot[i] = new InventorySlotData { item = item, amount = 1 };
                    return true;
                }
            }
            return false;
        }
        else
        {
            for (int i = 0; i < bagslot.Length; i++)
            {
                if (bagslot[i] == null || bagslot[i].item == null)
                {
                    bagslot[i] = new InventorySlotData { item = item, amount = 1 };
                    return true;
                }
            }
            return false;
        }
    }

    public void use_item(int slotIndex, bool isSpecial)
    {
        InventorySlotData slot = isSpecial ? specialslot[slotIndex] : bagslot[slotIndex];

        if (slot == null || slot.item == null) return;

        if (slot.item.IsUseableAnywhere)
        {
            slot.ClearSlot();
        }
        else if (checkPlayerValidLocation())
        {
            slot.ClearSlot();
        }
    }

    private bool checkPlayerValidLocation()
    {
        return false;
    }
}
