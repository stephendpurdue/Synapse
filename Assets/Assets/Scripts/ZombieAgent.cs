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
    [SerializeField] private float moveSpeed = 3.5f;
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
        navMeshAgent.speed = moveSpeed;
        navMeshAgent.stoppingDistance = attackRange;
        navMeshAgent.angularSpeed = 360f;
    }

    public override void OnEpisodeBegin()
    {
        episodeStartTime = Time.time;
        episodeEnding = false;
        attackCooldownTimer = 0f;
        zombieHealth = new HealthSystem(maxHealth);
        playerHealth.ResetHealth();
        attackTracker.Reset();
        navMeshAgent.ResetPath();
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

    // NavMesh always chases player directly
    navMeshAgent.SetDestination(player.position);

    // Update animator based on actual velocity
    float speed = navMeshAgent.velocity.magnitude;
    zombieController.SetSpeed(speed);

    // Face the player
    Vector3 directionToPlayer = (player.position - transform.position).normalized;
    if (directionToPlayer != Vector3.zero)
    {
        transform.rotation = Quaternion.LookRotation(directionToPlayer);
    }

    // Distance based rewards
    float distanceToPlayer = Vector3.Distance(transform.position, player.position);
    if (distanceToPlayer > 5f)
    {
        AddReward(-0.01f);
    }
    else
    {
        AddReward(0.01f);
    }

    AddReward(0.001f);

    // PPO controls attack decision only
    int attackAction = actions.DiscreteActions[0];
    attackCooldownTimer -= Time.deltaTime;

    if (attackAction == 1 && attackCooldownTimer <= 0f)
    {
        if (distanceToPlayer <= attackRange)
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

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        if (actionsOut.ContinuousActions.Length < 2 ||
            actionsOut.DiscreteActions.Length < 1)
            return;

        var continuousActions = actionsOut.ContinuousActions;
        var discreteActions = actionsOut.DiscreteActions;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        continuousActions[0] = directionToPlayer.x;
        continuousActions[1] = directionToPlayer.z;

        float distance = Vector3.Distance(transform.position, player.position);
        discreteActions[0] = distance <= attackRange ? 1 : 0;
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