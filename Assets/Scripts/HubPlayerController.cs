using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 4f;
    public float rotationSpeed = 10f;

    private CharacterController controller;
    private Animator animator;
    private Camera mainCamera;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        mainCamera = Camera.main;
    }

    void Update()
    {
        // 1️⃣ Input
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputZ = Input.GetAxisRaw("Vertical");

        // 2️⃣ Movimiento relativo a la cámara (isométrico)
        Vector3 camForward = mainCamera.transform.forward;
        Vector3 camRight = mainCamera.transform.right;

        camForward.y = 0f;
        camRight.y = 0f;

        Vector3 move = (camForward * inputZ + camRight * inputX).normalized;

        // 3️⃣ Movimiento físico
        controller.Move(move * speed * Time.deltaTime);

        // 4️⃣ Rotación suave hacia la dirección de movimiento
        if (move != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        // 5️⃣ Animaciones
        animator.SetFloat("MoveX", inputX);
        animator.SetFloat("MoveZ", inputZ);
        animator.SetBool("IsMoving", move.magnitude > 0);
    }
}
