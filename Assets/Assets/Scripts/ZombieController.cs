using UnityEngine;

public class ZombieController : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Reset()
    {
        animator.SetBool("IsDead", false);
        animator.ResetTrigger("Attack");
        animator.ResetTrigger("Hit");
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