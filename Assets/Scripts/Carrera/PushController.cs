using UnityEngine;

public class PushController : MonoBehaviour
{
    public float pushForce = 20f;
    public float stunDuration = 0.8f;
    public float pushRadius = 1.5f;
    public float pushCooldown = 1f;

    private float lastPushTime;
    private Animator animator;
    private bool isPlayer;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        // Solo el jugador puede usar input
        isPlayer = CompareTag("Player");
    }

    void Update()
    {
        // Solo el jugador responde al input E
        if (isPlayer && Input.GetKeyDown(KeyCode.E))
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

            IKnockbackable knockbackable = hit.GetComponent<IKnockbackable>();
            if (knockbackable != null)
            {
                knockbackable.ApplyKnockback(dir, pushForce);
            }

            IStunnable stunnable = hit.GetComponent<IStunnable>();
            if (stunnable != null)
            {
                stunnable.Stun(stunDuration);
            }
        }
    }
}
