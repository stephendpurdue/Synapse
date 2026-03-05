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
    private bool episodeEnding = false;
    private HealthSystem zombieHealth;
    private float maxHealth = 100f;
    private float episodeStartTime;
    private float minimumEpisodeDuration = 15f;

    public float HealthPercentage => zombieHealth.HealthPercentage;
    public float CurrentHealth => zombieHealth.CurrentHealth;

    public override void OnEpisodeBegin()
    {
        episodeStartTime = Time.time;
        zombieHealth = new HealthSystem(maxHealth);
        playerHealth.ResetHealth();
        attackTracker.Reset();
        attackCooldownTimer = 0f;
        episodeEnding = false;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(playerHealth.HealthPercentage);
        sensor.AddObservation(attackTracker.NormalisedAttackFrequency);
        float distance = Vector3.Distance(transform.position, player.position);
        sensor.AddObservation(Mathf.Clamp01(distance / 20f));
        sensor.AddObservation(zombieHealth.HealthPercentage);
        sensor.AddObservation(Mathf.Clamp01(attackCooldownTimer / attackCooldown));
    }

    public override void OnActionReceived(ActionBuffers actions)
{
    if (actions.ContinuousActions.Length < 2 || actions.DiscreteActions.Length < 1)
        return;

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

    zombieController.SetSpeed(moveDirection.magnitude);

    // Distance based rewards
    float distanceToPlayer = Vector3.Distance(transform.position, player.position);
    if (distanceToPlayer > 5f)
    {
        // Punish passivity
        AddReward(-0.01f);
    }
    else
    {
        // Reward engagement
        AddReward(0.01f);
    }

    // Survival reward
    AddReward(0.001f);

    // Attack logic
    int attackAction = actions.DiscreteActions[0];
    attackCooldownTimer -= Time.deltaTime;

    if (attackAction == 1 && attackCooldownTimer <= 0f)
    {
        if (distanceToPlayer <= attackRange)
        {
            float healthBefore = playerHealth.HealthPercentage;
            playerHealth.TakeDamage(attackDamage);
            float healthAfter = playerHealth.HealthPercentage;

            // Reward for dealing damage
            AddReward(0.1f);

            // Reward for getting player low
            if (healthAfter < 0.3f && healthBefore >= 0.3f)
            {
                AddReward(0.2f);
            }

            zombieController.TriggerAttack();
            attackCooldownTimer = attackCooldown;
        }
    }

    // Player death
    if (playerHealth.IsDead)
    {
        float episodeDuration = Time.time - episodeStartTime;

        if (episodeDuration < minimumEpisodeDuration)
        {
            // Punish killing player too quickly
            AddReward(-0.5f);
        }
        else
        {
            // Reward for a well fought encounter
            AddReward(1.0f);
        }

        StartCoroutine(EndEpisodeAfterDelay(2f));
    }

    // Zombie death
    if (zombieHealth.IsDead)
    {
        AddReward(-1.0f);
        zombieController.Die();
        StartCoroutine(EndEpisodeAfterDelay(2f));
    }
}

    public void TakeDamage(float amount)
    {
        zombieHealth.TakeDamage(amount);
        zombieController.TakeHit();
        AddReward(-0.05f);
    }

    private System.Collections.IEnumerator EndEpisodeAfterDelay(float delay)
    {
        if (episodeEnding) yield break;
        episodeEnding = true;
        yield return new WaitForSeconds(delay);
        episodeEnding = false;
        EndEpisode();
    }
}