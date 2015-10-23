namespace Code
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization.Formatters.Binary;
    using UnityEngine;

    /// <summary>
    /// The default implementation for loading stored settings for dependency injection implementations.
    /// </summary>
    public class DefaultSettingsStoreLoader : ISettingsStoreLoader
    {
        /// <summary>
        /// The loaded settings.
        /// </summary>
        private Dictionary<Type, Dictionary<string, object>> loadedSettings;

        private Dictionary<string, Dictionary<Type, Dictionary<string, object>>> loadedInstanceSettings;

        private Configuration[] _globalConfigurations;

        private Configuration[] _perSceneConfigurations;

        public DefaultSettingsStoreLoader(AllConfigurations allConfigurations)
        {
            this._globalConfigurations = allConfigurations.GlobalConfigurations;
            this._perSceneConfigurations = allConfigurations.PerSceneConfigurations;
        }

        /// <summary>
        /// Gets the specified option.
        /// </summary>
        /// <param name="instanceName">The instance name if applicable, or null.</param>
        /// <param name="name">The unique ID of the option.</param>
        /// <typeparam name="TImplementor">The implementor type which this option is defined against.</typeparam>
        /// <typeparam name="TReturn">The type of the option value.</typeparam>
        /// <returns>The value of the option.</returns>
        public TReturn Get<TImplementor, TReturn>(string instanceName, string name)
        {
            this.LoadSettingsIfNeeded();

            var loadedSettingsRef = this.loadedSettings;
            if (instanceName != null)
            {
                if (!this.loadedInstanceSettings.ContainsKey(instanceName))
                {
                    Debug.LogWarning(
                        "The instance name '" + instanceName + "' has no settings, yet " +
                        "an attempt to retrieve the '" + name +
                        "' setting on the instance was made.  Returning " +
                        "the default value for the requested type.");
                    return default(TReturn);
                }

                loadedSettingsRef = this.loadedInstanceSettings[instanceName];
            }

            if (!loadedSettingsRef.ContainsKey(typeof (TImplementor)))
            {
                Debug.LogWarning(
                    "The type '" + typeof (TImplementor).FullName + "' has no settings, yet " +
                    "an attempt to retrieve the '" + name +
                    "' setting was made.  Returning " + "the default value for the requested type.");
                return default(TReturn);
            }

            if (!loadedSettingsRef[typeof (TImplementor)].ContainsKey(name))
            {
                Debug.LogWarning(
                    "A request was made for the '" + name + "' setting on type " + "'" +
                    typeof (TImplementor).FullName + "', but the setting " +
                    "does not exist.  Returning the default value for the " + "requested type.");
                return default(TReturn);
            }

            return (TReturn) loadedSettingsRef[typeof (TImplementor)][name];
        }

        public IEnumerable<string> InstanceNames
        {
            get
            {
                LoadSettingsIfNeeded();
                return this.loadedInstanceSettings.Keys;
            }
        }

        /// <summary>
        /// Loads the settings if needed (they are only loaded once per game).
        /// </summary>
        private void LoadSettingsIfNeeded()
        {
            if (this.loadedSettings != null && this.loadedInstanceSettings != null)
            {
                return;
            }

            this.loadedSettings = new Dictionary<Type, Dictionary<string, object>>();
            this.loadedInstanceSettings = new Dictionary<string, Dictionary<Type, Dictionary<string, object>>>();
            foreach (var store in this._globalConfigurations)
            {
                LoadSettingsStore(store);
            }
            foreach (var store in this._perSceneConfigurations)
            {
                LoadSettingsStore(store);
            }
        }

        private void LoadSettingsStore(Configuration store)
        {
            // Find the instance name field if applicable.
            var settingsStoreInstanceName =
                (from field in
                        store.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance)
                            .OrderBy(x => x.FieldType.Name)
                            .ThenBy(x => x.Name)
                        let attrs = field.GetCustomAttributes(false)
                        where attrs.OfType<SettingInstanceNameAttribute>().Any()
                        select (string) field.GetValue(store)).FirstOrDefault();

            if (settingsStoreInstanceName != null)
            {
                if (!this.loadedInstanceSettings.ContainsKey(settingsStoreInstanceName))
                {
                    this.loadedInstanceSettings.Add(settingsStoreInstanceName,
                        new Dictionary<Type, Dictionary<string, object>>());
                }
            }

            var assembly = Assembly.GetExecutingAssembly();
            var formatter = new BinaryFormatter();
            foreach (var opt in store.StoredOptions)
            {
                if (!opt.Active)
                {
                    // The option is for an implementation that is not active.
                    continue;
                }

                var type = assembly.GetType(opt.ForImplementation);
                if (type == null)
                {
                    // The type does not exist, so this might be a setting for a class that has
                    // since been renamed.
                    continue;
                }

                Dictionary<string, object> dict;
                if (settingsStoreInstanceName != null)
                {
                    if (!this.loadedInstanceSettings[settingsStoreInstanceName].ContainsKey(type))
                    {
                        this.loadedInstanceSettings[settingsStoreInstanceName].Add(type, new Dictionary<string, object>());
                    }

                    dict = this.loadedInstanceSettings[settingsStoreInstanceName][type];
                }
                else
                {
                    if (!this.loadedSettings.ContainsKey(type))
                    {
                        this.loadedSettings.Add(type, new Dictionary<string, object>());
                    }

                    dict = this.loadedSettings[type];
                }
                
                ConfigurationSerializationHelper.RuntimeGet(dict, opt);
            }
        }
    }
}
