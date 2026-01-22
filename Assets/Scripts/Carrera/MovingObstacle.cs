using UnityEngine;

public class MovingObstacle : MonoBehaviour
{
    public Vector3 direction = Vector3.right;
    public float distance = 3f;
    public float speed = 2f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float offset = Mathf.Sin(Time.time * speed) * distance;
        transform.position = startPos + direction.normalized * offset;
    }
}
