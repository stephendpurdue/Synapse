using NUnit.Framework;
using Unity.MLAgents;

public class MLAgentsInstallationTest
{
    [Test]
    public void MLAgents_Namespace_IsAccessible()
    {
        var academyType = typeof(Academy);
        Assert.IsNotNull(academyType, 
            "ML-Agents Academy class should be accessible if package is installed.");
    }
}