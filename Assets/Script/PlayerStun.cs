using UnityEngine;
using System.Collections;

public class PlayerStun : MonoBehaviour
{
    private bool isStunned = false;
    private PlayerController playerController; // script điều khiển player (nếu có)

    private void Start()
    {
        playerController = GetComponent<PlayerController>(); // nếu player có script điều khiển
    }

    public void ApplyStun(float duration)
    {
        if (isStunned) return;
        StartCoroutine(StunRoutine(duration));
    }

    private IEnumerator StunRoutine(float duration)
    {
        isStunned = true;

        // Nếu có script điều khiển, tạm khóa input
        if (playerController != null)
            playerController.enabled = false;

        // Nếu không có script riêng thì chỉ tạm thời freeze Rigidbody
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        yield return new WaitForSeconds(duration);

        // Mở khóa
        if (playerController != null)
            playerController.enabled = true;

        if (rb != null)
            rb.constraints = RigidbodyConstraints2D.None;

        isStunned = false;
    }
}
