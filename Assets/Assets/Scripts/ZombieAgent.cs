using UnityEngine;
using UnityEngine.AI;
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
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float minimumEpisodeDuration = 15f;

    private NavMeshAgent navMeshAgent;
    private float attackCooldownTimer = 0f;
    private bool episodeEnding = false;
    private HealthSystem zombieHealth;
    private float maxHealth = 100f;
    private float episodeStartTime;

    public float HealthPercentage => zombieHealth.HealthPercentage;
    public float CurrentHealth => zombieHealth.CurrentHealth;

    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.stoppingDistance = 0;
        navMeshAgent.angularSpeed = 120f;
    }

    public override void OnEpisodeBegin()
    {
        episodeStartTime = Time.time;
        episodeEnding = false;
        attackCooldownTimer = 0f;
        zombieHealth = new HealthSystem(maxHealth);
        playerHealth.ResetHealth();
        attackTracker.Reset();
        navMeshAgent.isStopped = false;
        navMeshAgent.ResetPath();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(playerHealth.HealthPercentage);
        sensor.AddObservation(attackTracker.NormalisedAttackFrequency);
        float distance = Vector3.Distance(transform.position, player.position);
        sensor.AddObservation(Mathf.Clamp01(distance / 20f));
        sensor.AddObservation(zombieHealth.HealthPercentage);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Movement
        if (distanceToPlayer > attackRange)
        {
            navMeshAgent.isStopped = false;
            navMeshAgent.SetDestination(player.position);
        }
        else
        {
            navMeshAgent.isStopped = true;
            navMeshAgent.ResetPath();

            // Face the player
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            directionToPlayer.y = 0;
            if (directionToPlayer != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(directionToPlayer),
                    10f * Time.deltaTime
                );
            }
        }

        // Update animator
        zombieController.SetSpeed(navMeshAgent.velocity.magnitude);

        // Distance based rewards
        AddReward(distanceToPlayer > 5f ? -0.01f : 0.01f);
        AddReward(0.001f);

        // Attack when in range
        attackCooldownTimer -= Time.deltaTime;
        if (distanceToPlayer <= attackRange && attackCooldownTimer <= 0f)
        {
            float healthBefore = playerHealth.HealthPercentage;
            playerHealth.TakeDamage(attackDamage);
            float healthAfter = playerHealth.HealthPercentage;

            AddReward(0.1f);

            if (healthAfter < 0.3f && healthBefore >= 0.3f)
            {
                AddReward(0.2f);
            }

            zombieController.TriggerAttack();
            attackCooldownTimer = attackCooldown;
        }

        // Player death
        if (playerHealth.IsDead)
        {
            float episodeDuration = Time.time - episodeStartTime;
            AddReward(episodeDuration < minimumEpisodeDuration ? -0.5f : 1.0f);
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

    public override void Heuristic(in ActionBuffers actionsOut) { }

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