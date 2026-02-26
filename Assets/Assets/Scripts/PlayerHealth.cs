using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    
    private HealthSystem healthSystem;

    public float HealthPercentage => healthSystem.HealthPercentage;
    public float CurrentHealth => healthSystem.CurrentHealth;
    public bool IsDead => healthSystem.IsDead;

    void Awake()
    {
        healthSystem = new HealthSystem(maxHealth);
    }

    public void TakeDamage(float amount)
    {
        if (IsDead) return;
        healthSystem.TakeDamage(amount);
    }

    public void Heal(float amount) => healthSystem.Heal(amount);
    
    public void ResetHealth() => healthSystem.Reset();
}