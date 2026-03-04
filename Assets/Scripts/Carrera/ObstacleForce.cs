using UnityEngine;

public class ObstacleForce : MonoBehaviour
{
    public float pushForce = 25f;
    public float stunDuration = 1f;
    public bool rotateObstacle = false;
    public float rotationSpeed = 120f;

    void Update()
    {
        if (rotateObstacle)
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        Vector3 dir = (other.transform.position - transform.position).normalized;
        dir.y = 0;

        IKnockbackable knockbackable = other.GetComponent<IKnockbackable>();
        if (knockbackable != null)
        {
            knockbackable.ApplyKnockback(dir, pushForce);
        }

        IStunnable stun = other.GetComponent<IStunnable>();
        if (stun != null)
        {
            stun.Stun(stunDuration);
        }
    }
}
