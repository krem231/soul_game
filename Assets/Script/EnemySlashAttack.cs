using UnityEngine;
using System.Collections;

[RequireComponent(typeof(EnemyAnimation))]
public class EnemySlashAttack : MonoBehaviour
{
    [Header("Slash Settings")]
    public float counterDelay = 0.5f;
    public int slashDamage = 15;
    public float slashRange = 1.5f;
    public LayerMask playerLayer;

    private EnemyAnimation anim;
    private bool isSlashing;

    private void Awake()
    {
        anim = GetComponent<EnemyAnimation>();
    }

    public void TriggerCounterSlash(bool returnToBlock = false)
    {
        if (isSlashing) return;
        StartCoroutine(CounterSlashRoutine(returnToBlock));
    }

    private IEnumerator CounterSlashRoutine(bool returnToBlock)
    {
        isSlashing = true;
        yield return new WaitForSeconds(counterDelay);

        // Blend sang Slash
        anim?.SetSlashing(true);
        yield return new WaitForSeconds(0.2f);
        PerformSlashAttack();

        yield return new WaitForSeconds(0.4f);

        // Quay về block hoặc idle
        if (returnToBlock)
            anim?.SetBlocking(true);
        else
            anim?.SetIdle();

        isSlashing = false;
    }

    private void PerformSlashAttack()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, slashRange, playerLayer);
        if (hit != null)
        {
            PlayerController player = hit.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(slashDamage);
                Debug.Log($"[{name}] Slash hit player for {slashDamage} damage!");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, slashRange);
    }
}
