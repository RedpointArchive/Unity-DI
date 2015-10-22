namespace Code
{
    using System;

    /// <summary>
    /// An attribute that indicates that the field should show as a drop down in the Unity editor
    /// for selecting an implementation of the specified type.
    /// </summary>
    public class SelectImplementationOfAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Code.SelectImplementationOfAttribute"/> class.
        /// </summary>
        /// <param name="baseType">The interface whose implementation should be selected.</param>
        public SelectImplementationOfAttribute(Type baseType)
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
