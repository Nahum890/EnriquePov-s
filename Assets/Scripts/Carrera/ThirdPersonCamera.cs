using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;

    public Vector3 offset = new Vector3(0, 3, -6);
    public float smoothSpeed = 8f;
    public float rotationSpeed = 10f;

    void LateUpdate()
    {
        if (target == null) return;

        // Rotación basada en el jugador
        Quaternion targetRotation = Quaternion.Euler(0, target.eulerAngles.y, 0);

        Vector3 desiredPosition = target.position + targetRotation * offset;

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );

        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}
