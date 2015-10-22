namespace Code
{
    using System;

    /// <summary>
    /// An attribute that indicates that the field should as a list of checkboxes for enabling
    /// and disabling various implementations of a service.
    /// </summary>
    public class EnableImplementationsOfAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Code.EnableImplementationsOfAttribute"/> class.
        /// </summary>
        /// <param name="baseType">The interface whose implementation should be enabled or disabled.</param>
        public EnableImplementationsOfAttribute(Type baseType)
        {
            this.TypeFullName = baseType.FullName;
        }

        /// <summary>
        /// Gets the full name of the interface type.
        /// </summary>
        /// <value>The full name of the interface type.</value>
        public string TypeFullName { get; private set; }
    }
}
