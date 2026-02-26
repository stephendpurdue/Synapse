using NUnit.Framework;

public class AttackTrackerTests
{
    [Test]
    public void AttackTracker_StartsAtZero()
    {
        var tracker = new AttackTracker();
        Assert.AreEqual(0, tracker.RecentAttackCount);
    }

    [Test]
    public void AttackTracker_RegistersAttacks()
    {
        var tracker = new AttackTracker();
        tracker.RegisterAttack(0f);
        tracker.RegisterAttack(0f);
        Assert.AreEqual(2, tracker.RecentAttackCount);
    }

    [Test]
    public void AttackTracker_PurgesAttacksOutsideWindow()
    {
        var tracker = new AttackTracker(trackingWindow: 5f);
        tracker.RegisterAttack(0f);  // at t=0
        tracker.PurgeOldTimestamps(6f);  // purge at t=6, window is 5s
        Assert.AreEqual(0, tracker.RecentAttackCount);
    }

    [Test]
    public void AttackTracker_RetainsAttacksInsideWindow()
    {
        var tracker = new AttackTracker(trackingWindow: 5f);
        tracker.RegisterAttack(0f);  // at t=0
        tracker.PurgeOldTimestamps(4f);  // purge at t=4, still inside window
        Assert.AreEqual(1, tracker.RecentAttackCount);
    }

    [Test]
    public void AttackTracker_NormalisedValue_IsClampedBetween0And1()
    {
        var tracker = new AttackTracker();
        for (int i = 0; i < 100; i++) tracker.RegisterAttack(0f);
        Assert.LessOrEqual(tracker.NormalisedAttackFrequency, 1f);
        Assert.GreaterOrEqual(tracker.NormalisedAttackFrequency, 0f);
    }

    [Test]
    public void AttackTracker_Resets_Correctly()
    {
        var tracker = new AttackTracker();
        tracker.RegisterAttack(0f);
        tracker.Reset();
        Assert.AreEqual(0, tracker.RecentAttackCount);
    }
}