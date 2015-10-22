using System;

namespace Code
{
    /// <summary>
    /// Indicates that this field is used for instance naming in the settings store.  When you have
    /// multiple prefabs that have the same settings store component, the settings store accessor
    /// needs a unique name to distinguish between settings on one prefab from another.
    /// </summary>
    public class SettingInstanceNameAttribute : Attribute
    {
    }
}
