using UnityEngine;

public class ObstacleForce : MonoBehaviour
{
    public float pushForce = 8f;
    public float stunDuration = 1.5f;
    public bool rotateObstacle = false;
    public float rotationSpeed = 120f;

    void Update()
    {
        if (rotateObstacle)
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        CharacterController cc = other.GetComponent<CharacterController>();
        if (cc)
        {
            Vector3 dir = (other.transform.position - transform.position).normalized;
            cc.Move(dir * pushForce * Time.deltaTime);
        }

        IStunnable stun = other.GetComponent<IStunnable>();
        if (stun != null)
        {
            stun.Stun(stunDuration);
        }
    }
}
