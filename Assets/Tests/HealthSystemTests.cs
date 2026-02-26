using NUnit.Framework;

public class HealthSystemTests
{
    [Test]
    public void Health_StartsAtMax()
    {
        var health = new HealthSystem(100f);
        Assert.AreEqual(1f, health.HealthPercentage, 0.001f);
    }

    [Test]
    public void Health_TakeDamage_ReducesCorrectly()
    {
        var health = new HealthSystem(100f);
        health.TakeDamage(50f);
        Assert.AreEqual(0.5f, health.HealthPercentage, 0.001f);
    }

    [Test]
    public void Health_CannotGoBelowZero()
    {
        var health = new HealthSystem(100f);
        health.TakeDamage(999f);
        Assert.GreaterOrEqual(health.HealthPercentage, 0f);
    }

    [Test]
    public void Health_IsDead_WhenDepleted()
    {
        var health = new HealthSystem(100f);
        health.TakeDamage(100f);
        Assert.IsTrue(health.IsDead);
    }

    [Test]
    public void Health_NoDamage_WhenAlreadyDead()
    {
        var health = new HealthSystem(100f);
        health.TakeDamage(100f);
        health.TakeDamage(50f);
        Assert.AreEqual(0f, health.HealthPercentage, 0.001f);
    }

    [Test]
    public void Health_Resets_Correctly()
    {
        var health = new HealthSystem(100f);
        health.TakeDamage(100f);
        health.Reset();
        Assert.AreEqual(1f, health.HealthPercentage, 0.001f);
    }
}