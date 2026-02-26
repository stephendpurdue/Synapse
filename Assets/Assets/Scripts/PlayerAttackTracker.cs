using UnityEngine;

public class PlayerAttackTracker : MonoBehaviour
{
    [SerializeField] private float trackingWindow = 5f;
    [SerializeField] private float maxExpectedAttacks = 10f;

    private AttackTracker tracker;

    public float NormalisedAttackFrequency => 
        tracker.NormalisedAttackFrequency;
    public int RecentAttackCount => 
        tracker.RecentAttackCount;

    void Awake()
    {
        tracker = new AttackTracker(trackingWindow, maxExpectedAttacks);
    }

    void Update()
    {
        tracker.PurgeOldTimestamps(Time.time);
    }

    public void RegisterAttack()
    {
        tracker.RegisterAttack(Time.time);
    }

    public void Reset()
    {
        tracker.Reset();
    }
}