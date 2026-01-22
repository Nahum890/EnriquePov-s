using UnityEngine;

public class ObstacleForce : MonoBehaviour
{
    public float pushForce = 8f;
    public float stunDuration = 1.5f;
    public bool rotateObstacle = false;
    public float rotationSpeed = 120f;

    void Update()
    {
        if (rotateObstacle)
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        Vector3 dir = (other.transform.position - transform.position).normalized;
        dir.y = 0;

        // Aplicar knockback a NPCs
        NPCAI npc = other.GetComponent<NPCAI>();
        if (npc != null)
        {
            npc.ApplyKnockback(dir, pushForce);
        }

        // Aplicar knockback a jugador
        RaceCharacterController player = other.GetComponent<RaceCharacterController>();
        if (player != null)
        {
            player.ApplyKnockback(dir, pushForce);
        }

        // Aplicar stun
        IStunnable stun = other.GetComponent<IStunnable>();
        if (stun != null)
        {
            stun.Stun(stunDuration);
        }
    }
}
