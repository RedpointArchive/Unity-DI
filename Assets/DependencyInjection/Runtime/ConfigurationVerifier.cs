using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Code;
using UnityEngine;
using Object = UnityEngine.Object;

public class ConfigurationVerifier : MonoBehaviour
{
#if UNITY_EDITOR
    public void Awake()
    {
        // This only applies inside the Editor
        VerifyConfiguration();
    }
#endif

    public static void VerifyConfiguration()
    {
        _configurationGameObjects = null;

        Debug.Log("[dependency injection] Beginning dependency injection verification...");

        var types =
            from assembly in AppDomain.CurrentDomain.GetAssemblies()
            from type in assembly.GetTypes()
            where !type.IsAbstract && !type.IsInterface && typeof (BaseBehaviour).IsAssignableFrom(type)
            select type;

        var instances = new List<BaseBehaviour>();
        foreach (var type in types)
        {
            Debug.Log("[dependency injection] Calling default constructor of " + type.FullName + "...");
            var gameObject = new GameObject();
            BaseBehaviour.IsForDependencyInjectionVerification = true;
            instances.Add((BaseBehaviour)gameObject.AddComponent(type));
            BaseBehaviour.IsForDependencyInjectionVerification = false;
            Object.DestroyImmediate(gameObject);
        }

        Debug.Log("[dependency injection] All instances constructed successfully.");

        Debug.Log("[dependency injection] Testing dependencies can be satisifed...");

        var errors = 0;
        foreach (var instance in instances)
        {
            var combinations = GetAllProfileCombinationsForInstance(instance).ToArray();

            var wasNone = false;
            if (combinations.Length == 0)
            {
                combinations = new[] {new Dictionary<FieldInfo, string>()};
                wasNone = true;
            }

            foreach (var combination in combinations)
            {
                if (wasNone)
                {
                    Debug.Log("[dependency injection] Testing dependencies of " + instance.GetType().FullName + "...");
                }
                else
                {
                    var withProfiles =
                        combination.Select(x => x.Key.Name + "='" + x.Value + "'").Aggregate((a, b) => a + ", " + b);
                    Debug.Log("[dependency injection] Testing dependencies of " + instance.GetType().FullName + " with profiles " + withProfiles + "...");
                }

                try
                {
                    var kernel = new KernelContainer();
                    foreach (var combo in combination)
                    {
                        combo.Key.SetValue(instance, combo.Value);
                    }
                    var method = instance.GetType().GetMethod("Inject", BindingFlags.NonPublic | BindingFlags.Instance);
                    method.Invoke(instance, new object[] {kernel.Kernel});
                }
                catch (TargetInvocationException ex)
                {
                    Debug.LogException(ex.InnerException);
                    errors++;
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                    errors++;
                }
            }
        }

        if (errors == 0)
        {
            Debug.Log("OKAY - All dependencies can be satisifed!");
        }
        else
        {
            Debug.LogError("FAIL - There were " + errors + " when testing dependencies of behaviours.");
        }
    }

    private static List<GameObject> _configurationGameObjects = null;

    public static IEnumerable<Dictionary<FieldInfo, string>> GetAllProfileCombinationsForInstance(BaseBehaviour instance)
    {
        var dict = new Dictionary<FieldInfo, List<string>>();

        foreach (var fieldOuter in instance.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
        {
            var attributes = fieldOuter.GetCustomAttributes(typeof (SettingInstanceNameSelectorAttribute), true);

            if (attributes.Length > 0)
            {
                var fieldOptions = new List<string>();
                var range = (SettingInstanceNameSelectorAttribute) attributes[0];

                if (_configurationGameObjects == null)
                {
                    _configurationGameObjects = Resources.LoadAll("Configuration").OfType<GameObject>().ToList();
                }

                foreach (var res in _configurationGameObjects)
                {
                    var components = res.GetComponents(range.BaseSettingStoreType);
                    if (components.Length > 0)
                    {
                        var settingsStoreInstanceName =
                            (from field in
                                components[0].GetType().GetFields(BindingFlags.Public | BindingFlags.Instance)
                                    .OrderBy(x => x.FieldType.Name)
                                    .ThenBy(x => x.Name)
                                let attrs = field.GetCustomAttributes(false)
                                where attrs.OfType<SettingInstanceNameAttribute>().Any()
                                select (string) field.GetValue(components[0])).FirstOrDefault();

                        fieldOptions.Add(settingsStoreInstanceName);
                    }
                }

                dict[fieldOuter] = fieldOptions;
            }
        }

        var fields = dict.Keys.ToList();
        var stack = new Stack<int>();

        while (true)
        {
            if (stack.Count < fields.Count)
            {
                stack.Push(0);
            }
            else if (stack.Count == fields.Count)
            {
                var indices = stack.ToArray().Reverse().ToArray();
                var tempDict = new Dictionary<FieldInfo, string>();
                for (var i = 0; i < indices.Length; i++)
                {
                    tempDict.Add(fields[i], dict[fields[i]][indices[i]]);
                }
                yield return tempDict;
                stack.Push(stack.Pop() + 1);
                while (stack.Peek() >= dict[fields[stack.Count - 1]].Count)
                {
                    stack.Pop();
                    if (stack.Count == 0)
                    {
                        yield break;
                    }
                    var old = stack.Peek();
                    stack.Pop();
                    stack.Push(old + 1);
                }
            }
        }
    }
}