using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Code
{
    using System;
    using Ninject;
    using UnityEngine;

    /// <summary>
    /// Defines the entry point for accessing the dependency injection kernel, and configures
    /// all of the service bindings within the kernel at game startup.
    /// </summary>
    public class KernelContainer
    {
        private static KernelContainer _gameKernelContainer;

        /// <summary>
        /// The static kernel container for the game at runtime. DO NOT ACCESS THIS PROPERTY!
        /// </summary>
        public static KernelContainer GameKernelContainer
        {
            get
            {
                if (_gameKernelContainer == null)
                {
                    _gameKernelContainer = new KernelContainer();
                }
                return _gameKernelContainer;
            }
        }

        /// <summary>
        /// Initializes members of the <see cref="KernelContainer"/> class.
        /// </summary>
        public KernelContainer(bool isVerifier = false)
        {
            // Initialize a new dependency injection kernel.
            Kernel = new StandardKernel(new NinjectSettings { LoadExtensions = false, UseReflectionBasedInjection = true });

            // Object which limits the lifetime of implementations to the
            // current game.  This makes it easy to discard state when the
            // game restarts (like the player picks "New Game" or whatever),
            // without having things carry over.
            CurrentGame = new object();

            // Bind the settings store accessor.
            Kernel.Bind<ISettingsStoreAccessor>()
                .ToMethod(x => new DefaultSettingsStoreAccessor(x.Kernel.Get<ISettingsStoreLoader>(), x))
                .InScope(x => x.Request);
            Kernel.Bind<ISettingsStoreLoader>().To<DefaultSettingsStoreLoader>().InSingletonScope();

            // Find all of the setting store prefabs in the Resources folder and instantiate all global settings stores.
            var globalSettingsStores = new List<Configuration>();
            var globalSettingsStoreNames = new Dictionary<string, string>();
            foreach (var settingsStorePrefab in Resources.LoadAll("Configuration").OfType<GameObject>())
            {
                var settingsStore = settingsStorePrefab.GetComponent<Configuration>();

                if (settingsStore == null)
                {
                    Debug.LogWarning("[dependency injection] The prefab '" + settingsStorePrefab.name + "' does not have a valid configuration component attached");
                    continue;
                }

                if (settingsStore.PerScene)
                {
                    // This is a per-scene settings store, so skip it when looking for prefabs.
                    continue;
                }

                if (!isVerifier)
                {
                    Debug.LogFormat("[dependency injection] Loading configuration '" + settingsStorePrefab.name + "'");
                }

                // Validate that the settings store profile name is unique.
                var settingsStoreInstanceName =
                    (from field in
                            settingsStore.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance)
                                .OrderBy(x => x.FieldType.Name)
                                .ThenBy(x => x.Name)
                            let attrs = field.GetCustomAttributes(false)
                            where attrs.OfType<SettingInstanceNameAttribute>().Any()
                            select (string)field.GetValue(settingsStore)).FirstOrDefault();
                if (settingsStoreInstanceName != null)
                {
                    if (globalSettingsStoreNames.ContainsKey(settingsStoreInstanceName))
                    {
                        throw new InvalidOperationException(
                            "The prefab '" + settingsStorePrefab.name +
                            "' has a configuration profile name of '" +
                            settingsStoreInstanceName +
                            "', but this profile name is already used by '" +
                            globalSettingsStoreNames[settingsStoreInstanceName] +
                            "'.  Configuration profile names must be unique across " +
                            "the whole application.  Change the profile name inside the '" +
                            settingsStorePrefab.name +
                            "' prefab and run the game again.");
                    }

                    globalSettingsStoreNames[settingsStoreInstanceName] = settingsStorePrefab.name;
                }

                // Set up bindings for this setting store.
                settingsStore.SetupBindings(Kernel, LookupType, x => CurrentGame);

                // Add this setting store to the list of all settings stores.
                globalSettingsStores.Add(settingsStore);
            }

            // Find all of the setting store prefabs in the scene and instantiate all per-scene settings stores.
            var perSceneSettingsStores = new List<Configuration>();
            var perSceneSettingsStoreNames = new Dictionary<string, string>();
            foreach (var settingsStorePrefab in GameObject.FindGameObjectsWithTag("Configuration"))
            {
                var settingsStore = settingsStorePrefab.GetComponent<Configuration>();

                if (!settingsStore.PerScene)
                {
                    // This is a global settings store, so skip it when looking for game objects.
                    continue;
                }

                if (!isVerifier)
                {
                    Debug.LogFormat("[dependency injection] Loading per-scene configuration '" +
                                    settingsStorePrefab.name + "'");
                }

                // Validate that the settings store profile name is unique.
                var settingsStoreInstanceName =
                    (from field in
                            settingsStore.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance)
                                .OrderBy(x => x.FieldType.Name)
                                .ThenBy(x => x.Name)
                            let attrs = field.GetCustomAttributes(false)
                            where attrs.OfType<SettingInstanceNameAttribute>().Any()
                            select (string)field.GetValue(settingsStore)).FirstOrDefault();
                if (settingsStoreInstanceName != null)
                {
                    if (perSceneSettingsStoreNames.ContainsKey(settingsStoreInstanceName))
                    {
                        throw new InvalidOperationException(
                            "The prefab '" + settingsStorePrefab.name +
                            "' has a configuration profile name of '" +
                            settingsStoreInstanceName +
                            "', but this profile name is already used by '" +
                            perSceneSettingsStoreNames[settingsStoreInstanceName] +
                            "'.  Configuration profile names must be unique across " +
                            "the whole application.  Change the profile name inside the '" +
                            settingsStorePrefab.name +
                            "' prefab and run the game again.");
                    }

                    perSceneSettingsStoreNames[settingsStoreInstanceName] = settingsStorePrefab.name;
                }

                // Set up bindings for this setting store.
                settingsStore.SetupBindings(Kernel, LookupType, x => CurrentGame);

                // Add this setting store to the list of all settings stores.
                perSceneSettingsStores.Add(settingsStore);
            }

            // Bind all the settings stores for the accessor.
            Kernel.Bind<AllConfigurations>()
                .ToMethod(
                    x =>
                    new AllConfigurations
                    {
                        GlobalConfigurations = globalSettingsStores.ToArray(),
                        PerSceneConfigurations = perSceneSettingsStores.ToArray()
                    })
                .InSingletonScope();
        }

        /// <summary>
        /// Gets the dependency injection kernel.
        /// </summary>
        /// <value>The dependency injection kernel.</value>
        public StandardKernel Kernel { get; private set; }

        /// <summary>
        /// Gets or sets an object which represents the current game.
        /// <para>
        /// This reference is used as the lifetime scope for various implementations, ensuring that
        /// their state resets when a new game starts.
        /// </para>
        /// </summary>
        /// <value>An object which represents the current game.</value>
        public object CurrentGame { get; set; }

        /// <summary>
        /// Looks up a type based on the full type name.
        /// </summary>
        /// <returns>The type.</returns>
        /// <param name="typeName">The full type name.</param>
        private Type LookupType(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                throw new InvalidOperationException(
                    "One or more interfaces are missing an implementation " +
                    "in the configuration!  Fix this by selecting an " +
                    "implementation for all interfaces in the Configuration " +
                    "prefab.");
            }

            var type = System.Reflection.Assembly.GetExecutingAssembly().GetType(typeName);
            if (type == null)
            {
                throw new InvalidOperationException("There is no type with type name " + typeName);
            }

            var obsoleteAttribute =
                type.GetCustomAttributes(typeof (ObsoleteAttribute), true)
                    .OfType<ObsoleteAttribute>()
                    .FirstOrDefault(x => x.IsError);
            if (obsoleteAttribute != null)
            {
                throw new InvalidOperationException(
                    typeName +
                    " can not be selected in dependency injection " +
                    "because it is marked with the [Obsolete] attribute, " +
                    "with 'error' set to true.  Choose another implementation " +
                    "from the settings store that this is registered in (any " +
                    "implementation with an error shown in a settings store will " +
                    "result in one of these exceptions).  The reason for this " +
                    "implementation being disabled is: " + obsoleteAttribute.Message);
            }

            return type;
        }
    }
}
