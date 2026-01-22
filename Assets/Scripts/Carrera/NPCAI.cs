using UnityEngine;

[RequireComponent(typeof(RaceCharacterController))]
public class NPCAI : MonoBehaviour
{
    public Transform goal;

    [Header("Movement")]
    [SerializeField] private float npcSpeed = 5.5f;  // Más lento que el jugador (7f)

    [Header("Obstacle Avoidance")]
    [SerializeField] private float detectionDistance = 4f;
    [SerializeField] private float detectionRadius = 0.8f;
    [SerializeField] private LayerMask obstacleLayer;

    [Header("Imperfección (Balance)")]
    [SerializeField] private float reactionDelay = 0.1f;
    [Range(0f, 0.3f)]
    [SerializeField] private float mistakeChance = 0.08f;

    private RaceCharacterController controller;
    private CharacterController cc;
    private float lastCheckTime;
    private Vector3 cachedMoveDir;

    void Start()
    {
        controller = GetComponent<RaceCharacterController>();
        cc = GetComponent<CharacterController>();
        controller.canMove = false;
    }

    void Update()
    {
        if (!controller.canMove || goal == null) return;

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
            transform.forward = Vector3.Lerp(transform.forward, moveDir, Time.deltaTime * 5f);
        }

        // Mover usando la velocidad del NPC (no la del jugador)
        cc.Move(moveDir * npcSpeed * Time.deltaTime);
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
