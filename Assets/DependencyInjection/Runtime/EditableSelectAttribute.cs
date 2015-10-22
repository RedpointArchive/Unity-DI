namespace Code
{
    using System;

    /// <summary>
    /// An attribute which indicates the field on the implementation should appear
    /// as a selectable dropdown in the Unity editor.
    /// </summary>
    public class EditableSelectAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EditableSelectAttribute"/> class.
        /// </summary>
        /// <param name="id">The unique ID for this implementation option.</param>
        public EditableSelectAttribute(string id)
        {
            this.Name = id;
        }

        /// <summary>
        /// Gets the name to show in the Unity editor for this option.
        /// </summary>
        /// <value>The name to show in the Unity editor for this option.</value>
        public string Name { get; private set; }
    }
}
