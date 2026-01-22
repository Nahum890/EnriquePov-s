using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class RaceCharacterController : MonoBehaviour, IStunnable
{
    public float walkSpeed = 4f;
    public float runSpeed = 7f;
    public float jumpForce = 6f;
    public float gravity = -15f;

    public bool canMove = true;

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;

    private bool stunned;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (!canMove || stunned) 
        {
            animator.SetFloat("Speed", 0);
            return;
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(h, 0, v);
        float inputMagnitude = Mathf.Clamp01(move.magnitude);

        bool running = Input.GetKey(KeyCode.LeftShift);
        float speed = running ? runSpeed : walkSpeed;

        controller.Move(transform.TransformDirection(move) * speed * Time.deltaTime);

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

        // PUSH
        if (Input.GetKeyDown(KeyCode.E))
        {
            animator.SetTrigger("Push");
        }
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
}
