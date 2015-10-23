using Code;
using UnityEditor;
using UnityEngine;

public class ConfigurationDependencyVerifier
{
    [MenuItem("Dependency Injection/Verify Configuration Now!", priority = 100)]
    public static void VerifyConfigurationNow()
    {
        Debug.ClearDeveloperConsole();
        ConfigurationVerifier.VerifyConfiguration();
    }

    [MenuItem("Dependency Injection/Verify Configuration on Game Start", priority = 100)]
    public static void VerifyConfigurationOnGameStart()
    {
        var existing = GameObject.Find("ConfigurationVerifier");
        if (existing != null)
        {
            Object.DestroyImmediate(existing);
        }

        var gameObject = new GameObject();
        gameObject.name = "ConfigurationVerifier";
        gameObject.AddComponent<ConfigurationVerifier>();

        Debug.Log("Created configuration verifier game object!");
    }
}