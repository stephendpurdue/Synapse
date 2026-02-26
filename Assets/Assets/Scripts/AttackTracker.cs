using System.Collections.Generic;

public class AttackTracker
{
    private Queue<float> attackTimestamps = new Queue<float>();
    private float trackingWindow;
    private float maxExpectedAttacks;

    public float NormalisedAttackFrequency
    {
        get
        {
            return UnityEngine.Mathf.Clamp01(
                attackTimestamps.Count / maxExpectedAttacks);
        }
    }

    public int RecentAttackCount => attackTimestamps.Count;

    public AttackTracker(float trackingWindow = 5f,
                         float maxExpectedAttacks = 10f)
    {
        this.trackingWindow = trackingWindow;
        this.maxExpectedAttacks = maxExpectedAttacks;
    }

    public void RegisterAttack(float currentTime)
    {
        attackTimestamps.Enqueue(currentTime);
        PurgeOldTimestamps(currentTime);
    }

    public void PurgeOldTimestamps(float currentTime)
    {
        while (attackTimestamps.Count > 0 &&
               currentTime - attackTimestamps.Peek() > trackingWindow)
        {
            attackTimestamps.Dequeue();
        }
    }

    public void Reset()
    {
        attackTimestamps.Clear();
    }
}