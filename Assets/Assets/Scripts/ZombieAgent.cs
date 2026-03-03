using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class ZombieAgent : Agent
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerAttackTracker attackTracker;
    [SerializeField] private ZombieController zombieController;

    [Header("Settings")]
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackCooldown = 1.5f;

    private float attackCooldownTimer = 0f;
    private HealthSystem zombieHealth;
    private float maxHealth = 100f;

    public float HealthPercentage => zombieHealth.HealthPercentage;
    public float CurrentHealth => zombieHealth.CurrentHealth;

    public override void OnEpisodeBegin()
    {
        // Reset zombie health
        zombieHealth = new HealthSystem(maxHealth);

        // Reset player
        playerHealth.ResetHealth();
        attackTracker.Reset();

        // Reset attack cooldown
        attackCooldownTimer = 0f;
        
        episodeEnding = false;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Observation 1: Player health percentage (0-1)
        sensor.AddObservation(playerHealth.HealthPercentage);

        // Observation 2: Player attack frequency (0-1)
        sensor.AddObservation(attackTracker.NormalisedAttackFrequency);

        // Observation 3: Distance to player (normalised)
        float distance = Vector3.Distance(transform.position, player.position);
        sensor.AddObservation(Mathf.Clamp01(distance / 20f));

        // Observation 4: Zombie health percentage (0-1)
        sensor.AddObservation(zombieHealth.HealthPercentage);

        // Observation 5: Attack cooldown remaining (normalised)
        sensor.AddObservation(Mathf.Clamp01(attackCooldownTimer / attackCooldown));
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Action 1: Movement (continuous, -1 to 1 on X and Z)
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        Vector3 moveDirection = new Vector3(moveX, 0f, moveZ).normalized;
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        // Face the player
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        if (directionToPlayer != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(directionToPlayer);
        }

        // Update animator
        zombieController.SetSpeed(moveDirection.magnitude);

        // Action 2: Attack (discrete, 0 = no attack, 1 = attack)
        int attackAction = actions.DiscreteActions[0];
        attackCooldownTimer -= Time.deltaTime;

        if (attackAction == 1 && attackCooldownTimer <= 0f)
        {
            float distanceToPlayer = Vector3.Distance(
                transform.position, player.position);

            if (distanceToPlayer <= attackRange)
            {
                playerHealth.TakeDamage(attackDamage);
                zombieController.TriggerAttack();
                attackCooldownTimer = attackCooldown;
            }
        }

        // End episode if player dies
        if (playerHealth.IsDead)
        {
            StartCoroutine(EndEpisodeAfterDelay(2f));
        }

        // End episode if zombie dies
        if (zombieHealth.IsDead)
        {
            zombieController.Die();
            StartCoroutine(EndEpisodeAfterDelay(2f)); 
        }
    }

    public void TakeDamage(float amount)
    {
        zombieHealth.TakeDamage(amount);
        zombieController.TakeHit(); // trigger hit reaction animation
    }

    private bool episodeEnding = false;

    private System.Collections.IEnumerator EndEpisodeAfterDelay(float delay)
    {
        if (episodeEnding) yield break; // prevent calling twice
        episodeEnding = true;
        yield return new WaitForSeconds(delay);
        episodeEnding = false;
        EndEpisode();
    }
}