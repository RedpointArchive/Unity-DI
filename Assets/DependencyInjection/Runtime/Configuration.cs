using System;
using System.Collections.Generic;
using Code;
using Ninject;
using Ninject.Activation;
using UnityEngine;

/// <summary>
/// Base class for dependency injection based settings stores.
/// </summary>
public abstract class Configuration : MonoBehaviour
{
    /// <summary>
    /// Indicates whether or not this settings store is per-scene.  If it is, then the settings
    /// store will be searched via GameObject.Find instead of looking at the prefab version.  In
    /// addition, you will be able to leave things as default implementations.  Per-scene settings
    /// store are always loaded after all non-scene settings stores.
    /// </summary>
    public virtual bool PerScene { get { return false; } }

    /// <summary>
    /// The serialized, stored options for implementations of dependency injected services.
    /// </summary>
    [SerializeField]
    public List<ConfigurationOption> StoredOptions;

    /// <summary>
    /// Sets up the bindings in the kernel based on this settings store configuration.
    /// </summary>
    /// <param name="kernel">The kernel to bind into.</param>
    /// <param name="lookupType">A lambda for looking up types based on names.</param>
    /// <param name="currentGameScope">A lambda which returns the current game object.</param>
    public abstract void SetupBindings(IKernel kernel, Func<string, Type> lookupType, Func<IContext, object> currentGameScope);
}
