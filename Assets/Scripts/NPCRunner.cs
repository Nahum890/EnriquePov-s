using UnityEngine;
using System.Collections;

public class NPCRunner : MonoBehaviour, IStunnable
{
    public Transform goal;
    public float speed = 6f;
    public bool canMove = false;

    private CharacterController controller;
    private Animator animator;
    private bool stunned;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!canMove || stunned) return;

        Vector3 dir = (goal.position - transform.position).normalized;
        controller.Move(dir * speed * Time.deltaTime);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(dir),
            Time.deltaTime * 6f
        );

        animator.SetFloat("Speed", 1f);
    }

    public void Stun(float duration)
    {
        if (!gameObject.activeInHierarchy) return;
        StartCoroutine(StunRoutine(duration));
    }

    IEnumerator StunRoutine(float duration)
    {
        stunned = true;
        animator.SetBool("Stunned", true);
        yield return new WaitForSeconds(duration);
        stunned = false;
        animator.SetBool("Stunned", false);
    }
}
