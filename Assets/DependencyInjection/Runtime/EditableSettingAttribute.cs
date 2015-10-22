namespace Code
{
    using System;

    /// <summary>
    /// An attribute which indicates the field on the implementation should appear
    /// as editable in the Unity editor.
    /// </summary>
    public class EditableSettingAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Code.EditableSettingAttribute"/> class.
        /// </summary>
        /// <param name="id">The unique ID for this implementation option.</param>
        /// <param name="name">The name to show in the Unity editor for this option.</param>
        /// <param name="default">The default value for this option.</param>
        /// <param name="hideInDefaultFields">Whether this option should be hidden from the editable fields (for example if there's a custom editor to edit it).</param>
        public EditableSettingAttribute(string id, string name, object @default, bool hideInDefaultFields = false)
        {
            this.ID = id;
            this.Name = name;
            this.Default = @default;
            this.HideInDefaultFields = hideInDefaultFields;
        }

        /// <summary>
        /// Gets the unique ID for this implementation option.
        /// </summary>
        /// <value>The ID.</value>
        public string ID { get; private set; }

        /// <summary>
        /// Gets the name to show in the Unity editor for this option.
        /// </summary>
        /// <value>The name to show in the Unity editor for this option.</value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the default value for this option.
        /// </summary>
        /// <value>The default value for this option.</value>
        public object Default { get; private set; }

        public bool HideInDefaultFields { get; private set; }
    }
}
