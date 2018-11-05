using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class GameObjectCreationTest
{

    [Test]
    public void GameObjectCreationTestSimplePasses()
    {
        // Use the Assert class to test conditions.
        var go = new GameObject("New GameObject");
        Assert.AreEqual("New GameObject", go.name);
    }

    [UnityTest]
    public IEnumerator RigidbodyTest()
    {
        var go = new GameObject(); // Create a new GameObject
        go.AddComponent<Rigidbody>(); // Add Rigidbody to it
        Transform t = go.transform; // Simplify transform variable name
        Vector3 originalPos = t.position; // Copy original position

        yield return new WaitForFixedUpdate(); // Wait till the FixedUpdate frame is complete
        Assert.AreNotEqual(originalPos, t.position); // Compare original to current
    }

    // A UnityTest behaves like a coroutine in PlayMode
    // and allows you to yield null to skip a frame in EditMode
    [UnityTest]
    public IEnumerator GameObjectCreationTestWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // yield to skip a frame
        yield return null;
    }
}
