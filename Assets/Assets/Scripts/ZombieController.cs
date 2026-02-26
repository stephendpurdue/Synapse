using UnityEngine;

public class ZombieController : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void SetSpeed(float speed)
    {
        animator.SetFloat("Speed", speed);
    }

    public void TriggerAttack()
    {
        animator.SetTrigger("Attack");
    }

    public void TakeHit()
    {
        animator.SetTrigger("Hit");
    }

    public void Die()
    {
        animator.SetBool("IsDead", true);
    }
}