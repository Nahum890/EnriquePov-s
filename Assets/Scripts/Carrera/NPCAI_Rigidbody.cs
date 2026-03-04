using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class NPCAI_Rigidbody : MonoBehaviour, IStunnable
{
    public Transform goal;

    [Header("Movement")]
    public float moveSpeed = 7.5f;
    public float acceleration = 20f;
    public float rotationSpeed = 10f;

    [Header("Jump")]
    public float jumpForce = 6f;
    public float jumpForwardBoost = 1.2f;
    public float jumpCooldown = 1.2f;

    [Header("Obstacle Detection")]
    public float detectDistance = 1.4f;
    public float lowRayHeight = 0.4f;
    public float highRayHeight = 1.2f;

    [Header("Avoidance")]
    public float avoidTime = 0.6f;
    public float mistakeChance = 0.05f;

    public bool canMove = false;

    private Rigidbody rb;
    private Animator animator;

    private Vector3 desiredVelocity;
    private float jumpTimer;

    private float avoidTimer;
    private int avoidDir;

    private bool grounded;
    private bool stunned;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
    }

    void FixedUpdate()
    {
        if (!canMove || stunned || goal == null)
        {
            animator?.SetFloat("Speed", 0);
            return;
        }

        CheckGround();

        Vector3 toGoal = goal.position - transform.position;
        toGoal.y = 0;
        Vector3 forward = toGoal.normalized;

        Vector3 moveDir = forward;

        // ===== DETECCIÓN DE OBSTÁCULOS =====
        bool obstacleLow = Physics.Raycast(
            transform.position + Vector3.up * lowRayHeight,
            transform.forward,
            detectDistance
        );

        bool obstacleHigh = Physics.Raycast(
            transform.position + Vector3.up * highRayHeight,
            transform.forward,
            detectDistance
        );

        Debug.DrawRay(transform.position + Vector3.up * lowRayHeight, transform.forward * detectDistance, Color.red);
        Debug.DrawRay(transform.position + Vector3.up * highRayHeight, transform.forward * detectDistance, Color.blue);

        // ===== DECISIÓN =====
        if (grounded && jumpTimer <= 0)
        {
            if (obstacleLow && !obstacleHigh)
                Jump(forward);
            else if (obstacleHigh)
                StartAvoid();
        }

        // ===== EVASIÓN =====
        if (avoidTimer > 0)
        {
            avoidTimer -= Time.fixedDeltaTime;
            Vector3 right = Vector3.Cross(Vector3.up, forward);
            moveDir = (forward * 0.4f + right * avoidDir * 0.6f).normalized;
        }

        // ===== MOVIMIENTO =====
        desiredVelocity = moveDir * moveSpeed;
        rb.linearVelocity = Vector3.Lerp(
            rb.linearVelocity,
            new Vector3(desiredVelocity.x, rb.linearVelocity.y, desiredVelocity.z),
            acceleration * Time.fixedDeltaTime
        );

        // ===== ROTACIÓN =====
        if (moveDir.magnitude > 0.1f)
        {
            Quaternion rot = Quaternion.LookRotation(moveDir);
            rb.MoveRotation(
                Quaternion.Slerp(rb.rotation, rot, rotationSpeed * Time.fixedDeltaTime)
            );
        }

        jumpTimer -= Time.fixedDeltaTime;

        // ===== ANIMACIONES =====
        animator?.SetFloat("Speed", rb.linearVelocity.magnitude / moveSpeed);
        animator?.SetBool("IsGrounded", grounded);
    }

    // =============================
    // SALTO
    // =============================
    void Jump(Vector3 forward)
    {
        if (Random.value < mistakeChance) return;

        rb.AddForce(
            Vector3.up * jumpForce + forward * jumpForce * jumpForwardBoost,
            ForceMode.Impulse
        );

        jumpTimer = jumpCooldown;
        animator?.SetTrigger("Jump");
    }

    // =============================
    // EVASIÓN
    // =============================
    void StartAvoid()
    {
        if (Random.value < mistakeChance) return;

        avoidDir = Random.value > 0.5f ? 1 : -1;
        avoidTimer = avoidTime;
    }

    // =============================
    // GROUND CHECK
    // =============================
    void CheckGround()
    {
        grounded = Physics.Raycast(
            transform.position + Vector3.up * 0.1f,
            Vector3.down,
            0.3f
        );
    }

    // =============================
    // STUN
    // =============================
    public void Stun(float duration)
    {
        if (!gameObject.activeInHierarchy) return;
        StartCoroutine(StunRoutine(duration));
    }

    IEnumerator StunRoutine(float t)
    {
        stunned = true;
        rb.linearVelocity = Vector3.zero;
        yield return new WaitForSeconds(t);
        stunned = false;
    }
}
