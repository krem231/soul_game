using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Scriptable_object item;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // viết hoa P cho đúng Unity Tag mặc định
        {
            // Nhấn phím E để nhặt
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (inventory_function.instance.equip_item(item)) // gọi đúng script quản lý
                {
                    Destroy(gameObject); // hủy object pickup
                }
                else
                {
                    Debug.Log("Túi đầy, không nhặt được!");
                }
            }
        }
    }
}
