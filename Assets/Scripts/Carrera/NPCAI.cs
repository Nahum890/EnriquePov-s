using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class NPCAI : MonoBehaviour, IStunnable
{
    public Transform goal;

    [Header("Movement")]
    [SerializeField] private float npcSpeed = 5.5f;
    [SerializeField] private float gravity = -15f;
    [SerializeField] private float rotationSpeed = 8f;

    [Header("Obstacle Avoidance")]
    [SerializeField] private float detectionDistance = 3f;
    [SerializeField] private float sideDetectionDistance = 1.5f;
    [SerializeField] private LayerMask obstacleLayer;

    [Header("Imperfección (Balance)")]
    [Range(0f, 0.3f)]
    [SerializeField] private float mistakeChance = 0.05f;

    [HideInInspector] public bool canMove = false;

    private CharacterController cc;
    private Animator animator;
    private Vector3 velocity;
    private Vector3 knockbackVelocity;
    private bool stunned;

    // Para detectar si está atascado
    private Vector3 lastPosition;
    private float stuckTime;
    private int avoidDirection = 0; // -1 izquierda, 0 ninguno, 1 derecha

    void Start()
    {
        cc = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        lastPosition = transform.position;
    }

    void Update()
    {
        // Aplicar gravedad
        if (cc.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        velocity.y += gravity * Time.deltaTime;
        cc.Move(velocity * Time.deltaTime);

        // Aplicar knockback (más fuerte, reducción más lenta)
        if (knockbackVelocity.magnitude > 0.5f)
        {
            cc.Move(knockbackVelocity * Time.deltaTime);
            knockbackVelocity *= 0.92f; // Reducción más gradual
        }
        else
        {
            knockbackVelocity = Vector3.zero;
        }

        if (!canMove || goal == null || stunned)
        {
            if (animator != null) animator.SetFloat("Speed", 0);
            return;
        }

        // Detectar si está atascado
        float distanceMoved = Vector3.Distance(transform.position, lastPosition);
        if (distanceMoved < 0.01f)
        {
            stuckTime += Time.deltaTime;
        }
        else
        {
            stuckTime = 0f;
            if (avoidDirection != 0 && !IsObstacleAhead())
            {
                avoidDirection = 0; // Resetear dirección de evasión
            }
        }
        lastPosition = transform.position;

        // Calcular dirección de movimiento
        Vector3 moveDir = CalculateMovementDirection();

        // Rotar hacia la dirección de movimiento
        if (moveDir.magnitude > 0.1f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
        }

        // Mover
        cc.Move(moveDir * npcSpeed * Time.deltaTime);

        // Animación
        if (animator != null)
        {
            animator.SetFloat("Speed", 1f);
            animator.SetBool("IsGrounded", cc.isGrounded);
        }
    }

    private Vector3 CalculateMovementDirection()
    {
        Vector3 dirToGoal = (goal.position - transform.position);
        dirToGoal.y = 0;
        dirToGoal.Normalize();

        // Error ocasional - ir directo sin esquivar
        if (Random.value < mistakeChance * Time.deltaTime)
        {
            return dirToGoal;
        }

        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 right = Vector3.Cross(Vector3.up, dirToGoal).normalized;

        // Si está atascado, forzar evasión lateral
        if (stuckTime > 0.3f)
        {
            if (avoidDirection == 0)
            {
                // Elegir dirección de evasión basada en cuál lado está más libre
                bool rightBlocked = Physics.Raycast(origin, right, sideDetectionDistance, obstacleLayer);
                bool leftBlocked = Physics.Raycast(origin, -right, sideDetectionDistance, obstacleLayer);

                if (!rightBlocked && !leftBlocked)
                {
                    avoidDirection = Random.value > 0.5f ? 1 : -1;
                }
                else if (!rightBlocked)
                {
                    avoidDirection = 1;
                }
                else
                {
                    avoidDirection = -1;
                }
            }

            // Moverse perpendicular hasta salir del obstáculo
            return right * avoidDirection;
        }

        // Detección frontal con raycast
        RaycastHit hit;
        if (Physics.Raycast(origin, dirToGoal, out hit, detectionDistance, obstacleLayer))
        {
            // Revisar qué lado está más libre
            bool rightClear = !Physics.Raycast(origin, (dirToGoal + right * 0.5f).normalized, detectionDistance, obstacleLayer);
            bool leftClear = !Physics.Raycast(origin, (dirToGoal - right * 0.5f).normalized, detectionDistance, obstacleLayer);

            if (rightClear && !leftClear)
            {
                return (dirToGoal + right * 0.8f).normalized;
            }
            else if (leftClear && !rightClear)
            {
                return (dirToGoal - right * 0.8f).normalized;
            }
            else if (rightClear && leftClear)
            {
                // Ambos libres, elegir el más cercano a la meta
                Vector3 rightPath = (dirToGoal + right * 0.5f).normalized;
                Vector3 leftPath = (dirToGoal - right * 0.5f).normalized;
                return Vector3.Dot(rightPath, dirToGoal) > Vector3.Dot(leftPath, dirToGoal) ? rightPath : leftPath;
            }
            else
            {
                // Ambos bloqueados, ir perpendicular
                return right * (Random.value > 0.5f ? 1 : -1);
            }
        }

        return dirToGoal;
    }

    private bool IsObstacleAhead()
    {
        Vector3 dirToGoal = (goal.position - transform.position);
        dirToGoal.y = 0;
        dirToGoal.Normalize();
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        return Physics.Raycast(origin, dirToGoal, detectionDistance * 0.5f, obstacleLayer);
    }

    public void ApplyKnockback(Vector3 direction, float force)
    {
        knockbackVelocity = direction.normalized * force;
    }

    public void Stun(float duration)
    {
        if (!gameObject.activeInHierarchy) return;
        StartCoroutine(StunRoutine(duration));
    }

    private System.Collections.IEnumerator StunRoutine(float time)
    {
        stunned = true;
        avoidDirection = 0;
        stuckTime = 0f;
        if (animator != null) animator.SetFloat("Speed", 0);
        yield return new WaitForSeconds(time);
        stunned = false;
    }
}
