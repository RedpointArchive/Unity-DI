using System.Collections.Generic;

namespace Code
{
    /// <summary>
    /// The settings store loader interface, which loads and caches all of the settings
    /// in the dependency injection system.
    /// </summary>
    public interface ISettingsStoreLoader
    {
        /// <summary>
        /// Gets the specified option.
        /// </summary>
        /// <param name="instanceName">The instance name if applicable, or null.</param>
        /// <param name="name">The unique ID of the option.</param>
        /// <typeparam name="TImplementor">The implementor type which this option is defined against.</typeparam>
        /// <typeparam name="TReturn">The type of the option value.</typeparam>
        /// <returns>The value of the option.</returns>
        TReturn Get<TImplementor, TReturn>(string instanceName, string name);

        /// <summary>
        /// A list of all of the loaded instance names.
        /// </summary>
        IEnumerable<string> InstanceNames { get; }
    }
}
