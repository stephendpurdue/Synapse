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
    [SerializeField] private float maxEpisodeDuration = 120f;

    private NavMeshAgent navMeshAgent;
    private float attackCooldownTimer = 0f;
    private bool episodeEnding = false;
    private HealthSystem zombieHealth;
    private float maxHealth = 100f;
    private float episodeStartTime;
    private float damageDealtThisStep = 0f;

    public float HealthPercentage => zombieHealth?.HealthPercentage ?? 0f;
    public float CurrentHealth => zombieHealth?.CurrentHealth ?? 0f;

    protected override void Awake()
    {
        base.Awake();
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.stoppingDistance = 0f;
        navMeshAgent.speed = moveSpeed;
        navMeshAgent.angularSpeed = 120f;
        navMeshAgent.acceleration = 8f;
        navMeshAgent.autoBraking = true;
    }

    public override void OnEpisodeBegin()
    {
        episodeEnding = false;

        if (playerHealth == null || attackTracker == null ||
            navMeshAgent == null || zombieController == null)
        {
            Debug.LogError("[ZombieAgent] Missing references in Inspector.");
            return;
        }

        episodeStartTime = Time.time;
        attackCooldownTimer = attackCooldown;
        damageDealtThisStep = 0f;
        zombieHealth = new HealthSystem(maxHealth);
        playerHealth.ResetHealth();
        attackTracker.Reset();
        navMeshAgent.isStopped = false;
        navMeshAgent.ResetPath();
    }

    void Update()
    {
        if (zombieHealth == null) return;
        if (episodeEnding) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > attackRange)
        {
            navMeshAgent.isStopped = false;
            navMeshAgent.SetDestination(player.position);
        }
        else
        {
            navMeshAgent.isStopped = true;
            navMeshAgent.ResetPath();
            navMeshAgent.velocity = Vector3.zero;
            navMeshAgent.nextPosition = transform.position;

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

            // Attack
            attackCooldownTimer -= Time.deltaTime;
            if (attackCooldownTimer <= 0f)
            {
                playerHealth.TakeDamage(attackDamage);
                damageDealtThisStep += attackDamage;
                zombieController.TriggerAttack();
                attackCooldownTimer = attackCooldown;
            }
        }

        zombieController.SetSpeed(navMeshAgent.velocity.magnitude);

        // Death checks in Update to ensure they're always caught
        if (playerHealth.IsDead && !episodeEnding)
        {
            float episodeDuration = Time.time - episodeStartTime;
            if (episodeDuration < minimumEpisodeDuration)
            {
                AddReward(-1.0f);
            }
            else if (episodeDuration <= 30f)
            {
                AddReward(1.0f);
            }
            else if (episodeDuration <= 60f)
            {
                AddReward(0.5f);
            }
            else
            {
                AddReward(-0.5f);
            }
            StartCoroutine(EndEpisodeAfterDelay(2f));
        }

        if (zombieHealth.IsDead && !episodeEnding)
        {
            AddReward(-1.0f);
            zombieController.Die();
            StartCoroutine(EndEpisodeAfterDelay(2f));
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        if (zombieHealth == null)
        {
            sensor.AddObservation(0f);
            sensor.AddObservation(0f);
            sensor.AddObservation(0f);
            sensor.AddObservation(0f);
            return;
        }
        sensor.AddObservation(playerHealth.HealthPercentage);
        sensor.AddObservation(attackTracker.NormalisedAttackFrequency);
        float distance = Vector3.Distance(transform.position, player.position);
        sensor.AddObservation(Mathf.Clamp01(distance / 20f));
        sensor.AddObservation(zombieHealth.HealthPercentage);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (zombieHealth == null || episodeEnding) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        float episodeDuration = Time.time - episodeStartTime;

        // Reward for damage dealt this step
        if (damageDealtThisStep > 0f)
        {
            AddReward(0.05f);
            damageDealtThisStep = 0f;
        }

        // Reward keeping player health in danger zone
        float playerHealthPct = playerHealth.HealthPercentage;
        if (playerHealthPct >= 0.3f && playerHealthPct <= 0.7f)
        {
            AddReward(0.01f);
        }
        else if (playerHealthPct > 0.9f)
        {
            AddReward(-0.01f);
        }

        // Reward staying close
        AddReward(distanceToPlayer > 5f ? -0.01f : 0.005f);

        // Episode timeout
        if (episodeDuration > maxEpisodeDuration && !episodeEnding)
        {
            AddReward(-0.5f);
            StartCoroutine(EndEpisodeAfterDelay(0f));
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut) { }

    public void TakeDamage(float amount)
    {
        if (zombieHealth == null) return;
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