using UnityEngine;
using TMPro;

public class Item : MonoBehaviour
{
    public Scriptable_object itemData;
    public GameObject pickupHintPrefab; // Prefab trong Canvas
    private GameObject pickupHintInstance;
    private Transform player;
    private bool isPlayerNear = false;
    private Canvas mainCanvas;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        mainCanvas = FindObjectOfType<Canvas>(); // tìm Canvas chính

        if (mainCanvas == null)
        {
            Debug.LogError("❌ Không tìm thấy Canvas chính trong scene!");
            return;
        }

        // Tạo hint trong Canvas (đảm bảo hiển thị được)
        pickupHintInstance = Instantiate(pickupHintPrefab, mainCanvas.transform);
        pickupHintInstance.SetActive(false);
    }

    private void Update()
    {
        if (player == null || pickupHintInstance == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance < 1.5f)
        {
            if (!isPlayerNear)
            {
                isPlayerNear = true;
                pickupHintInstance.SetActive(true);
                Debug.Log($"📍 {name}: Player tới gần, hiển thị gợi ý");
            }

            // Cập nhật vị trí UI hint theo item (chuyển từ world sang screen)
            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 1.5f);
            pickupHintInstance.transform.position = screenPos;
        }
        else if (isPlayerNear)
        {
            isPlayerNear = false;
            pickupHintInstance.SetActive(false);
            Debug.Log($"🚫 {name}: Player rời xa, ẩn gợi ý");
        }
    }

    private void OnDestroy()
    {
        if (pickupHintInstance != null)
            Destroy(pickupHintInstance);
    }
}
