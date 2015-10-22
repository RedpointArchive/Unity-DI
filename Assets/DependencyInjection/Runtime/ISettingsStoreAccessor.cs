namespace Code
{
    /// <summary>
    /// The settings store accessor interface, which provides access to retrieve
    /// saved options for dependency injection implementations.
    /// </summary>
    public interface ISettingsStoreAccessor
    {
        /// <summary>
        /// Gets the specified option.
        /// </summary>
        /// <param name="name">The unique ID of the option.</param>
        /// <typeparam name="TImplementor">The implementor type which this option is defined against.</typeparam>
        /// <typeparam name="TReturn">The type of the option value.</typeparam>
        /// <returns>The value of the option.</returns>
        TReturn Get<TImplementor, TReturn>(string name);
    }
}
