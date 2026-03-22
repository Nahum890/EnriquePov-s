using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class RaceCharacterController : MonoBehaviour, IStunnable, IKnockbackable
{
    public float walkSpeed = 4f;
    public float runSpeed = 7f;
    public float jumpForce = 6f;
    public float gravity = -15f;
    public float rotationSpeed = 12f;

    [Header("Momentum / Aceleración")]
    public float acceleration = 10f;
    public float deceleration = 8f;
    public float airControl = 3f;

    [Header("Salto Parkour")]
    public float runJumpMultiplier = 1.3f;

    [Header("Knockback")]
    public float knockbackDecay = 5f;

    [Header("State")]
    public bool canMove = true;

    private CharacterController controller;
    private Animator animator;
    private Transform cam;

    private Vector3 velocity;
    private Vector3 externalForce;
    private Vector3 moveDirection;

    private float currentSpeed;
    private bool stunned;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        cam = Camera.main.transform;
    }

    void Update()
    {
        if (!canMove || stunned) return;

        bool grounded = controller.isGrounded;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        bool running = Input.GetKey(KeyCode.LeftShift);
        bool jumpPressed = Input.GetButtonDown("Jump");

        Vector3 inputDir = new Vector3(h, 0, v).normalized;

        // =============================
        // MOVIMIENTO RELATIVO A CÁMARA
        // =============================
        Vector3 camForward = cam.forward;
        Vector3 camRight = cam.right;

        camForward.y = 0;
        camRight.y = 0;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 desiredMove =
            camForward * inputDir.z +
            camRight * inputDir.x;

        // =============================
        // ROTACIÓN DEL MODELO
        // =============================
        if (desiredMove.magnitude > 0.1f)
        {
            Quaternion targetRot = Quaternion.LookRotation(desiredMove);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                12f * Time.deltaTime
            );
        }

        // =============================
        // ACELERACIÓN / MOMENTUM
        // =============================
        float targetSpeed = running ? runSpeed : walkSpeed;

        if (inputDir.magnitude > 0)
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
        else
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, deceleration * Time.deltaTime);

        if (grounded)
            moveDirection = desiredMove;
        else
            moveDirection = Vector3.Lerp(moveDirection, desiredMove, airControl * Time.deltaTime);

        velocity.x = moveDirection.x * currentSpeed;
        velocity.z = moveDirection.z * currentSpeed;

        // =============================
        // SALTO PARKOUR
        // =============================
        if (grounded)
        {
            if (velocity.y < 0)
                velocity.y = -2f;

            if (jumpPressed)
            {
                float boost = running ? runJumpMultiplier : 1f;
                velocity.y = jumpForce;
                velocity.x *= boost;
                velocity.z *= boost;

                animator?.SetTrigger("Jump");
            }
        }

        velocity.y += gravity * Time.deltaTime;

        // =============================
        // KNOCKBACK
        // =============================
        externalForce = Vector3.Lerp(
            externalForce,
            Vector3.zero,
            knockbackDecay * Time.deltaTime
        );

        Vector3 finalMove = velocity + externalForce;
        controller.Move(finalMove * Time.deltaTime);

        UpdateAnimator(grounded, running);
    }

    void UpdateAnimator(bool grounded, bool running)
    {
        if (!animator) return;

        float speedPercent = currentSpeed / runSpeed;
        animator.SetFloat("Speed", speedPercent, 0.12f, Time.deltaTime);
        animator.SetBool("IsRunning", running);
        animator.SetBool("IsGrounded", grounded);
    }

    // =============================
    // EMPUJONES / STUN
    // =============================
    public void ApplyKnockback(Vector3 direction, float force)
    {
        ApplyKnockback(direction, force, 0.15f);
    }

    public void ApplyKnockback(Vector3 direction, float force, float stunTime)
    {
        direction.y = 0;
        direction.Normalize();

        externalForce = direction * force;

        if (stunTime > 0)
            Stun(stunTime);

        animator?.SetTrigger("Push");
    }

    public void Stun(float duration)
    {
        if (!gameObject.activeInHierarchy) return;
        StartCoroutine(StunRoutine(duration));
    }

    IEnumerator StunRoutine(float time)
    {
        stunned = true;
        yield return new WaitForSeconds(time);
        stunned = false;
    }
}
