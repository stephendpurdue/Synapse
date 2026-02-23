using UnityEngine;

public class ZombieController : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {

        float speed = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).magnitude;
        animator.SetFloat("Speed", speed);

        // Trigger attack on Space
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("Attack");
        }
    }

    // Call these from other scripts (e.g., health system)
    public void TakeHit()
    {
        animator.SetTrigger("Hit");
    }

    public void Die()
    {
        animator.SetBool("IsDead", true);
    }
}