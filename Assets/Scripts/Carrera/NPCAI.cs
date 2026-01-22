using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class NPCAI : MonoBehaviour, IStunnable
{
    public Transform goal;

    [Header("Movement")]
    [SerializeField] private float npcSpeed = 5.5f;  // Más lento que el jugador (7f)
    [SerializeField] private float gravity = -15f;
    [SerializeField] private float rotationSpeed = 5f;

    [Header("Obstacle Avoidance")]
    [SerializeField] private float detectionDistance = 4f;
    [SerializeField] private float detectionRadius = 0.8f;
    [SerializeField] private LayerMask obstacleLayer;

    [Header("Imperfección (Balance)")]
    [SerializeField] private float reactionDelay = 0.1f;
    [Range(0f, 0.3f)]
    [SerializeField] private float mistakeChance = 0.08f;

    [HideInInspector] public bool canMove = false;

    private CharacterController cc;
    private Animator animator;
    private float lastCheckTime;
    private Vector3 cachedMoveDir;
    private Vector3 velocity;
    private bool stunned;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        // Aplicar gravedad siempre
        if (cc.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        velocity.y += gravity * Time.deltaTime;
        cc.Move(velocity * Time.deltaTime);

        if (!canMove || goal == null || stunned)
        {
            if (animator != null) animator.SetFloat("Speed", 0);
            return;
        }

        Vector3 dirToGoal = (goal.position - transform.position);
        dirToGoal.y = 0;
        dirToGoal.Normalize();

        Vector3 moveDir = dirToGoal;

        // Solo recalcular evasión cada reactionDelay segundos
        if (Time.time - lastCheckTime >= reactionDelay)
        {
            lastCheckTime = Time.time;
            cachedMoveDir = CalculateMovementDirection(dirToGoal);
        }

        // Usar dirección cacheada si existe
        if (cachedMoveDir != Vector3.zero)
        {
            moveDir = cachedMoveDir;
        }

        // Rotar hacia la dirección de movimiento
        if (moveDir != Vector3.zero)
        {
            transform.forward = Vector3.Lerp(transform.forward, moveDir, Time.deltaTime * rotationSpeed);
        }

        // Mover usando la velocidad del NPC
        cc.Move(moveDir * npcSpeed * Time.deltaTime);

        // Actualizar animación
        if (animator != null)
        {
            animator.SetFloat("Speed", 0.5f); // Velocidad de caminata
            animator.SetBool("IsGrounded", cc.isGrounded);
        }
    }

    public void Stun(float duration)
    {
        if (!gameObject.activeInHierarchy) return;
        StartCoroutine(StunRoutine(duration));
    }

    private System.Collections.IEnumerator StunRoutine(float time)
    {
        stunned = true;
        if (animator != null) animator.SetFloat("Speed", 0);
        yield return new WaitForSeconds(time);
        stunned = false;
    }

    private Vector3 CalculateMovementDirection(Vector3 dirToGoal)
    {
        // Simular error ocasional - a veces no detecta el obstáculo
        if (Random.value < mistakeChance)
        {
            return dirToGoal;
        }

        // Detectar obstáculos adelante
        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up * 0.5f;

        if (Physics.SphereCast(origin, detectionRadius, dirToGoal, out hit, detectionDistance, obstacleLayer))
        {
            // Calcular dirección de evasión (perpendicular al obstáculo)
            Vector3 avoidDir = Vector3.Cross(Vector3.up, hit.normal).normalized;

            // Elegir la dirección que más se acerque a la meta
            Vector3 rightPath = dirToGoal + avoidDir;
            Vector3 leftPath = dirToGoal - avoidDir;

            // Comparar cuál dirección lleva más cerca de la meta
            float rightDot = Vector3.Dot(rightPath.normalized, dirToGoal);
            float leftDot = Vector3.Dot(leftPath.normalized, dirToGoal);

            Vector3 chosenDir = rightDot > leftDot ? rightPath : leftPath;
            chosenDir.y = 0;

            return chosenDir.normalized;
        }

        return dirToGoal;
    }
}
