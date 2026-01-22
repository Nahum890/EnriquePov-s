using UnityEngine;

public class RaceCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Distance & Height")]
    [SerializeField] private float distance = 7f;
    [SerializeField] private float height = 3f;
    [SerializeField] private float minDistance = 3f;
    [SerializeField] private float maxDistance = 12f;

    [Header("Mouse Sensitivity")]
    [SerializeField] private float mouseSensitivityX = 3f;
    [SerializeField] private float mouseSensitivityY = 2f;
    [SerializeField] private float scrollSensitivity = 2f;

    [Header("Vertical Limits")]
    [SerializeField] private float minVerticalAngle = -20f;
    [SerializeField] private float maxVerticalAngle = 60f;

    [Header("Smoothing")]
    [SerializeField] private float positionSmoothSpeed = 10f;
    [SerializeField] private float rotationSmoothSpeed = 8f;

    [Header("Collision")]
    [SerializeField] private LayerMask collisionLayers;
    [SerializeField] private float collisionRadius = 0.3f;

    private float currentYaw;
    private float currentPitch = 15f;
    private Vector3 currentVelocity;

    void Start()
    {
        // Inicializar ángulos basados en la posición inicial
        if (target != null)
        {
            currentYaw = target.eulerAngles.y;
        }

        // Ocultar y bloquear cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (!target) return;

        HandleInput();
        UpdateCameraPosition();
    }

    private void HandleInput()
    {
        // Rotación con mouse
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivityX;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivityY;

        currentYaw += mouseX;
        currentPitch -= mouseY;
        currentPitch = Mathf.Clamp(currentPitch, minVerticalAngle, maxVerticalAngle);

        // Zoom con scroll
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        distance -= scroll * scrollSensitivity;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);
    }

    private void UpdateCameraPosition()
    {
        // Calcular rotación orbital
        Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0);

        // Calcular posición deseada
        Vector3 targetPosition = target.position + Vector3.up * height;
        Vector3 desiredPosition = targetPosition - (rotation * Vector3.forward * distance);

        // Detectar colisiones
        Vector3 finalPosition = HandleCollision(targetPosition, desiredPosition);

        // Suavizar movimiento
        transform.position = Vector3.SmoothDamp(
            transform.position,
            finalPosition,
            ref currentVelocity,
            1f / positionSmoothSpeed
        );

        // Suavizar rotación mirando al jugador
        Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSmoothSpeed * Time.deltaTime
        );
    }

    private Vector3 HandleCollision(Vector3 targetPos, Vector3 desiredPos)
    {
        Vector3 direction = desiredPos - targetPos;
        float targetDistance = direction.magnitude;

        if (Physics.SphereCast(targetPos, collisionRadius, direction.normalized, out RaycastHit hit, targetDistance, collisionLayers))
        {
            // Acercar la cámara si hay obstáculo
            return targetPos + direction.normalized * (hit.distance - collisionRadius);
        }

        return desiredPos;
    }

    void OnDisable()
    {
        // Restaurar cursor al desactivar
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus && enabled)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
