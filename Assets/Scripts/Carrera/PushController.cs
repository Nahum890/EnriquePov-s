using UnityEngine;

public class PushController : MonoBehaviour
{
    public float pushForce = 20f;
    public float stunDuration = 0.8f;
    public float pushRadius = 1.5f;
    public float pushCooldown = 1f;

    private float lastPushTime;
    private Animator animator;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryPush();
        }
    }

    public void TryPush()
    {
        if (Time.time - lastPushTime < pushCooldown)
            return;

        lastPushTime = Time.time;

        if (animator)
            animator.SetTrigger("Push");

        Collider[] hits = Physics.OverlapSphere(transform.position + transform.forward * 0.8f, pushRadius);

        foreach (Collider hit in hits)
        {
            if (hit.gameObject == gameObject) continue;

            Vector3 dir = (hit.transform.position - transform.position).normalized;
            dir.y = 0;

            // Empujar NPCs
            NPCAI npc = hit.GetComponent<NPCAI>();
            if (npc != null)
            {
                npc.ApplyKnockback(dir, pushForce);
                npc.Stun(stunDuration);
                continue;
            }

            // Empujar otros jugadores
            RaceCharacterController player = hit.GetComponent<RaceCharacterController>();
            if (player != null)
            {
                player.ApplyKnockback(dir, pushForce);
                player.Stun(stunDuration);
            }
        }
    }
}
