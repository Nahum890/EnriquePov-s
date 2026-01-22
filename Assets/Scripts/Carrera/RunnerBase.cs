using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class RunnerBase : MonoBehaviour
{
    [Header("Movimiento")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float jumpForce = 6f;

    [Header("Física")]
    public float gravity = -9.8f;
    public float pushStrength = 6f;
    public float forceDamping = 6f;

    protected CharacterController controller;
    protected Vector3 velocity;
    protected Vector3 externalForce;

    protected virtual void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    protected void ApplyGravity()
    {
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
    }

    public void ApplyPush(Vector3 direction)
    {
        externalForce += direction.normalized * pushStrength;
    }

    protected void DampenForces()
    {
        externalForce = Vector3.Lerp(
            externalForce,
            Vector3.zero,
            forceDamping * Time.deltaTime
        );
    }
}
