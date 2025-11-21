using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class testing
{
    // A Test behaves as an ordinary method
    [Test]
    public void testingSimplePasses()
    {
        // Use the Assert class to test conditions
    }
    
    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator testingWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
    [Test]
    public void Heal_IncreasesHealth()
    {
        PlayerModel p = new PlayerModel(100f);

        p.TakeDamage(100f);

        p.Heal(10f);

        Assert.AreEqual(10f, p.health);
    }
    [Test]
    public void takeDamage()
    {
        PlayerModel p = new PlayerModel(100f);

        p.TakeDamage(30f);

        Assert.AreEqual(70f, p.health);
    }
    [Test]
    public void Critical()
    {
        PlayerModel p = new PlayerModel(100f);
        float dame = p.Calculate_Skill_Damage(5, -21.5f);
        
        Assert.AreEqual(28f,dame);
    }
}