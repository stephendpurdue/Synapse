public class HealthSystem
{
    private float maxHealth;
    private float currentHealth;

    public float HealthPercentage => currentHealth / maxHealth;
    public bool IsDead => currentHealth <= 0f;
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    public HealthSystem(float max)
    {
        maxHealth = max;
        currentHealth = max;
    }

    public void TakeDamage(float amount)
    {
        if (IsDead) return;
        currentHealth = UnityEngine.Mathf.Clamp(
            currentHealth - amount, 0f, maxHealth);
    }

    public void Heal(float amount)
    {
        if (IsDead) return;
        currentHealth = UnityEngine.Mathf.Clamp(
            currentHealth + amount, 0f, maxHealth);
    }

    public void Reset()
    {
        currentHealth = maxHealth;
    }
}