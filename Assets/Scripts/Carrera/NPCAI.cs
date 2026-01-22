using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class NPCAI : MonoBehaviour, IStunnable
{
    public Transform goal;

    [Header("Movement")]
    [SerializeField] private float npcSpeed = 5.5f;
    [SerializeField] private float gravity = -15f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Obstacle Avoidance")]
    [SerializeField] private float avoidTime = 0.5f;

    [Header("Imperfección (Balance)")]
    [Range(0f, 0.3f)]
    [SerializeField] private float mistakeChance = 0.05f;

    [HideInInspector] public bool canMove = false;

    private CharacterController cc;
    private Animator animator;
    private Vector3 velocity;
    private Vector3 knockbackVelocity;
    private bool stunned;

    // Para evasión de obstáculos
    private Vector3 lastPosition;
    private float stuckTime;
    private float avoidTimer;
    private int avoidDirection = 0; // -1 izquierda, 1 derecha
    private bool hitObstacle;

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

        // Aplicar knockback
        if (knockbackVelocity.magnitude > 0.5f)
        {
            cc.Move(knockbackVelocity * Time.deltaTime);
            knockbackVelocity *= 0.92f;
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
        if (distanceMoved < 0.02f)
        {
            stuckTime += Time.deltaTime;
        }
        else
        {
            stuckTime = 0f;
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

        // Resetear flag de colisión
        hitObstacle = false;
    }

    private Vector3 CalculateMovementDirection()
    {
        Vector3 dirToGoal = (goal.position - transform.position);
        dirToGoal.y = 0;
        dirToGoal.Normalize();

        Vector3 right = Vector3.Cross(Vector3.up, dirToGoal).normalized;

        // Reducir timer de evasión
        if (avoidTimer > 0)
        {
            avoidTimer -= Time.deltaTime;
            // Mezclar dirección de evasión con dirección a meta
            return (dirToGoal * 0.3f + right * avoidDirection * 0.7f).normalized;
        }

        // Si chocó con obstáculo o está atascado, iniciar evasión
        if (hitObstacle || stuckTime > 0.2f)
        {
            // Error ocasional - no esquivar
            if (Random.value < mistakeChance)
            {
                return dirToGoal;
            }

            // Elegir dirección aleatoria si no hay una
            if (avoidDirection == 0 || stuckTime > 0.5f)
            {
                avoidDirection = Random.value > 0.5f ? 1 : -1;
            }

            avoidTimer = avoidTime;
            stuckTime = 0f;
            return (dirToGoal * 0.3f + right * avoidDirection * 0.7f).normalized;
        }

        // Sin obstáculo, ir directo a la meta
        avoidDirection = 0;
        return dirToGoal;
    }

    // Detectar colisión con obstáculos físicamente
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Ignorar suelo
        if (hit.normal.y > 0.5f) return;

        // Ignorar otros personajes
        if (hit.gameObject.GetComponent<NPCAI>() != null) return;
        if (hit.gameObject.GetComponent<RaceCharacterController>() != null) return;

        // Chocó con obstáculo
        hitObstacle = true;
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
