using UnityEngine;

public class PushController : MonoBehaviour
{
    public float pushForce = 6f;
    public float pushRadius = 1.2f;
    public float pushCooldown = 1f;

    private float lastPushTime;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void TryPush()
    {
        if (Time.time - lastPushTime < pushCooldown)
            return;

        lastPushTime = Time.time;

        if (animator)
            animator.SetTrigger("Push");

        Collider[] hits = Physics.OverlapSphere(transform.position + transform.forward, pushRadius);

        foreach (Collider hit in hits)
        {
            if (hit.gameObject == gameObject) continue;

            CharacterController cc = hit.GetComponent<CharacterController>();
            if (cc)
            {
                Vector3 dir = (hit.transform.position - transform.position).normalized;
                cc.Move(dir * pushForce * Time.deltaTime);
            }
        }
    }
}
