namespace Code
{
    using System;

    /// <summary>
    /// An attribute that indicates the name to show inside the Unity editor when selecting this implementation.
    /// </summary>
    public class ImplementationFriendlyNameAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImplementationFriendlyNameAttribute"/> class.
        /// </summary>
        /// <param name="name">The name to show in the Unity editor.</param>
        public ImplementationFriendlyNameAttribute(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Gets the name to show in the Unity editor for this implementation.
        /// </summary>
        /// <value>The name to show in the Unity editor for this implementation.</value>
        public string Name { get; private set; }
    }
}
