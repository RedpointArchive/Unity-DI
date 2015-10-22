using System;
using UnityEngine;

namespace Code
{
    public class SettingInstanceNameSelectorAttribute : PropertyAttribute
    {
        public Type BaseSettingStoreType { get; set; }

        public SettingInstanceNameSelectorAttribute(Type baseSettingStoreType)
        {
            BaseSettingStoreType = baseSettingStoreType;
        }
    }
}
