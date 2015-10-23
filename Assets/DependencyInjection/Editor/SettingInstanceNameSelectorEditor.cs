using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Code;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SettingInstanceNameSelectorAttribute))]
public class SettingInstanceNameSelectorEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var fieldOptions = new List<string>();
        var range = attribute as SettingInstanceNameSelectorAttribute;

        foreach (var res in Resources.LoadAll("Configuration").OfType<GameObject>())
        {
            var components = res.GetComponents(range.BaseSettingStoreType);
            if (components.Length > 0)
            {
                var settingsStoreInstanceName =
                    (from field in
                            components[0].GetType().GetFields(BindingFlags.Public | BindingFlags.Instance)
                                .OrderBy(x => x.FieldType.Name)
                                .ThenBy(x => x.Name)
                            let attrs = field.GetCustomAttributes(false)
                            where attrs.OfType<SettingInstanceNameAttribute>().Any()
                            select (string)field.GetValue(components[0])).FirstOrDefault();

                fieldOptions.Add(settingsStoreInstanceName);
            }
        }

        if (fieldOptions.Count > 0)
        {
            if (fieldOptions.IndexOf(property.stringValue) != -1)
            {
                property.stringValue = fieldOptions[EditorGUI.Popup(position, label,
                    fieldOptions.IndexOf(property.stringValue),
                    fieldOptions.Select(x => new GUIContent(x)).ToArray())];
            }
            else
            {
                property.stringValue = fieldOptions[EditorGUI.Popup(position, label,
                    0,
                    fieldOptions.Select(x => new GUIContent(x)).ToArray())];
            }
        }
        else
        {
            property.stringValue = null;
        }
    }
}
