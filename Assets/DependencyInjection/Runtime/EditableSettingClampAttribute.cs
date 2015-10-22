namespace Code
{
    using System;

    /// <summary>
    /// An attribute which clamps the range of values that may be accepted for an editable setting.
    /// </summary>
    public class EditableSettingClampAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Code.EditableSettingClampAttribute"/> class.
        /// </summary>
        /// <param name="min">The minimum inclusive value allowed for the field.</param>
        /// <param name="max">The maximum inclusive value allowed for the field.</param>
        public EditableSettingClampAttribute(float min, float max)
        {
            Min = min;
            Max = max;
        }

        /// <summary>
        /// Gets the minimum inclusive value allowed for the field.
        /// </summary>
        public float Min { get; private set; }

        /// <summary>
        /// Gets the maximum inclusive value allowed for the field.
        /// </summary>
        public float Max { get; private set; }
    }
}
