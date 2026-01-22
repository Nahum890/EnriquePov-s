using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class RaceCharacterController : MonoBehaviour, IStunnable
{
    public float walkSpeed = 4f;
    public float runSpeed = 7f;
    public float jumpForce = 6f;
    public float gravity = -15f;
    public float rotationSpeed = 12f;

    public bool canMove = true;

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;
    private Vector3 knockbackVelocity;
    private Transform cameraTransform;

    private bool stunned;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        // Obtener referencia a la cámara principal
        if (Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        // Aplicar y reducir knockback
        if (knockbackVelocity.magnitude > 0.5f)
        {
            controller.Move(knockbackVelocity * Time.deltaTime);
            knockbackVelocity *= 0.92f; // Reducción más gradual
        }
        else
        {
            knockbackVelocity = Vector3.zero;
        }

        if (!canMove || stunned)
        {
            animator.SetFloat("Speed", 0);
            return;
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = Vector3.zero;
        float inputMagnitude = Mathf.Clamp01(new Vector3(h, 0, v).magnitude);

        if (inputMagnitude > 0.1f)
        {
            // Calcular dirección relativa a la cámara
            Vector3 cameraForward = cameraTransform != null ? cameraTransform.forward : Vector3.forward;
            Vector3 cameraRight = cameraTransform != null ? cameraTransform.right : Vector3.right;

            // Proyectar en el plano horizontal
            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();

            // Movimiento relativo a la cámara
            move = (cameraForward * v + cameraRight * h).normalized;

            // Rotar el personaje hacia la dirección de movimiento
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        bool running = Input.GetKey(KeyCode.LeftShift);
        float speed = running ? runSpeed : walkSpeed;

        controller.Move(move * speed * Time.deltaTime);

        animator.SetFloat("Speed", inputMagnitude * (running ? 1f : 0.5f));

        // Ground check
        bool grounded = controller.isGrounded;
        animator.SetBool("IsGrounded", grounded);

        if (grounded)
        {
            if (velocity.y < 0) velocity.y = -2f;

            if (Input.GetButtonDown("Jump"))
            {
                velocity.y = jumpForce;
                animator.SetTrigger("Jump");
            }
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    public void Stun(float duration)
    {
        if (!gameObject.activeInHierarchy) return;
        StartCoroutine(StunRoutine(duration));
    }

    IEnumerator StunRoutine(float time)
    {
        stunned = true;
        animator.SetFloat("Speed", 0);
        yield return new WaitForSeconds(time);
        stunned = false;
    }

    public void ApplyKnockback(Vector3 direction, float force)
    {
        knockbackVelocity = direction.normalized * force;
    }
}
