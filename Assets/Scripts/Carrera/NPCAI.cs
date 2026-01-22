using UnityEngine;

[RequireComponent(typeof(RaceCharacterController))]
public class NPCAI : MonoBehaviour
{
    public Transform goal;
    private RaceCharacterController controller;
    private CharacterController cc;

    void Start()
    {
        controller = GetComponent<RaceCharacterController>();
        cc = GetComponent<CharacterController>();
        controller.canMove = false;
    }

    void Update()
    {
        if (!controller.canMove || goal == null) return;

        Vector3 dir = (goal.position - transform.position).normalized;

        transform.forward = Vector3.Lerp(transform.forward, dir, Time.deltaTime * 5f);
        cc.Move(dir * controller.runSpeed * Time.deltaTime);
    }
}
