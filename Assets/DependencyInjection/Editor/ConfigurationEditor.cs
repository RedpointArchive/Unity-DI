using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using Code;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The custom Unity editor used to edit the <see cref="Configuration"/> component.
/// </summary>
[CustomEditor(typeof(Configuration), true)]
public class ConfigurationEditor : Editor
{
    /// <summary>
    /// The assembly that types should be loaded from.
    /// </summary>
    private static Assembly assembly = typeof(Configuration).Assembly;

    private static Dictionary<Type, IDependencyTargetCustomEditor> editorInstances = new Dictionary<Type, IDependencyTargetCustomEditor>();

    /// <summary>
    /// Called when the GUI needs to render the inspector.
    /// </summary>
    public override void OnInspectorGUI()
    {
        var formatter = new BinaryFormatter();
        var settings = (Configuration)target;
        string settingsStoreInstanceName = null;

        if (settings.StoredOptions == null)
        {
            settings.StoredOptions = new List<ConfigurationOption>();
        }

        var type = target.GetType();

        // Handle any settings store instance name field, and update all of the settings
        // store options so that they have the instance name set in them correctly.
        foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance).OrderBy(x => x.FieldType.Name).ThenBy(x => x.Name))
        {
            var attrs = field.GetCustomAttributes(false);

            foreach (var attr in attrs.OfType<SettingInstanceNameAttribute>())
            {
                field.SetValue(this.target,
                    EditorGUILayout.TextField(this.GetFancyName(field.Name), (string)field.GetValue(this.target)));
                settingsStoreInstanceName = (string)field.GetValue(this.target);
                break;
            }

            if (settingsStoreInstanceName != null)
            {
                break;
            }
        }
        foreach (var opt in settings.StoredOptions)
        {
            opt.ForInstance = settingsStoreInstanceName;
        }

        var defaultOpts = new string[0];
        var inherit = "(inherit from global settings)";
        if (settings.PerScene)
        {
            defaultOpts = new[] { inherit };
        }

        // For all public fields that have a "SelectImplementationOf" attribute,
        // show an option list with all of the available implementations.
        var implementorOptions = new List<Type>();
        var activeImplementations = new List<string>();
        foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance).OrderBy(x => x.FieldType.Name).ThenBy(x => x.Name))
        {
            var attrs = field.GetCustomAttributes(false);
            var fieldOptions = new List<string>();
            var enableOptions = new List<string>();
            var isSettingInstanceNameField = false;

            foreach (var attr in attrs.OfType<SettingInstanceNameAttribute>())
            {
                isSettingInstanceNameField = true;
            }

            foreach (var attr in attrs.OfType<SelectImplementationOfAttribute>())
            {
                var referencedType = assembly.GetType(attr.TypeFullName);
                var implementors = from x in assembly.GetTypes()
                    where referencedType.IsAssignableFrom(x)
                    where !x.IsAbstract && !x.IsInterface
                    select x.FullName;
                fieldOptions.AddRange(implementors);
            }

            if (fieldOptions.Count > 0)
            {
                var existingIdx = fieldOptions.IndexOf((string) field.GetValue(target));
                if (settings.PerScene)
                {
                    existingIdx++;
                }

                var guiFieldIdx = EditorGUILayout.Popup(
                    this.GetFancyName(field.Name),
                    existingIdx,
                    defaultOpts.Concat(fieldOptions.Select(x =>
                                {
                                    var fancyName = this.GetFancyName(x);
                                    var y = assembly.GetType(x);
                                    var isObsolete = y.GetCustomAttributes(typeof (ObsoleteAttribute), true).FirstOrDefault();
                                    return (isObsolete != null ? (((ObsoleteAttribute)isObsolete).IsError ? "(Disabled) " : "(Deprecated) ") : string.Empty) + fancyName;
                                })).ToArray());

                if (guiFieldIdx >= 0 && guiFieldIdx < (settings.PerScene ? (fieldOptions.Count + 1) : fieldOptions.Count))
                {
                    if (settings.PerScene)
                    {
                        guiFieldIdx--;
                        if (guiFieldIdx == -1)
                        {
                            field.SetValue(this.target, string.Empty);
                        }
                        else
                        {
                            field.SetValue(this.target, fieldOptions[guiFieldIdx]);
                        }
                    }
                    else
                    {
                        field.SetValue(this.target, fieldOptions[guiFieldIdx]);
                    }

                    activeImplementations.Add((string)field.GetValue(this.target));

                    if (guiFieldIdx >= 0)
                    {
                        var implementor = assembly.GetType(fieldOptions[guiFieldIdx]);
                        if (implementor != null)
                        {
                            implementorOptions.Add(implementor);

                            var obsoleteAttribute =
                                implementor.GetCustomAttributes(typeof (ObsoleteAttribute), true).FirstOrDefault();

                            if (obsoleteAttribute != null)
                            {
                                EditorGUI.indentLevel++;

                                EditorGUILayout.HelpBox(
                                    this.GetObsoleteMessage(implementor, obsoleteAttribute),
                                    this.GetObsoleteMessageType(implementor, obsoleteAttribute));

                                EditorGUI.indentLevel--;
                            }
                        }
                    }
                }
            }

            foreach (var attr in attrs.OfType<EnableImplementationsOfAttribute>())
            {
                var referencedType = assembly.GetType(attr.TypeFullName);
                var implementors = from x in assembly.GetTypes()
                    where referencedType.IsAssignableFrom(x)
                    where !x.IsAbstract && !x.IsInterface
                    select x.FullName;
                enableOptions.AddRange(implementors);
            }

            if (enableOptions.Count > 0)
            {
                var currentValues = field.GetValue(target) as string[];
                var newValues = new List<string>();

                if (currentValues == null)
                {
                    currentValues = new string[0];
                }

                EditorGUILayout.LabelField(this.GetFancyName(field.Name) + ":");

                EditorGUI.indentLevel++;

                foreach (var opt in enableOptions)
                {
                    if (EditorGUILayout.ToggleLeft(this.GetFancyName(opt), currentValues.Contains(opt)))
                    {
                        newValues.Add(opt);

                        var implementor = assembly.GetType(opt);
                        if (implementor != null)
                        {
                            implementorOptions.Add(implementor);

                            activeImplementations.Add(implementor.FullName);
                        }
                    }
                }

                EditorGUI.indentLevel--;

                field.SetValue(this.target, newValues.ToArray());
            }

            if (fieldOptions.Count == 0 && enableOptions.Count == 0 && !isSettingInstanceNameField)
            {
                // Show a standard input field if this is a string type.
                if (field.FieldType == typeof (string))
                {
                    field.SetValue(this.target,
                        EditorGUILayout.TextField(this.GetFancyName(field.Name), (string)field.GetValue(this.target)));
                }
            }
        }

        foreach (var implementor in implementorOptions.OrderBy(x => x.Name))
        {
            // Check if there is a custom editor for this implementation or the
            // interface it's implementing.
            if (!editorInstances.ContainsKey(implementor))
            {
                foreach (var editorType in Assembly.GetExecutingAssembly().GetTypes())
                {
                    if (!typeof (IDependencyTargetCustomEditor).IsAssignableFrom(editorType))
                    {
                        continue;
                    }

                    if (!editorType.IsAbstract && !editorType.IsInterface)
                    {
                        var editorInfo = (IDependencyTargetCustomEditor) Activator.CreateInstance(editorType);
                        if (editorInfo.GetTargetType().IsAssignableFrom(implementor))
                        {
                            editorInstances[implementor] = editorInfo;
                            break;
                        }
                    }
                }
            }

            var editorInstance = editorInstances.ContainsKey(implementor) ? editorInstances[implementor] : null;

            var hasFields = false;
            foreach (var field in implementor.GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
            {
                var settingAttrs = field.GetCustomAttributes(typeof(EditableSettingAttribute), false);
                var setting = settingAttrs.OfType<EditableSettingAttribute>().FirstOrDefault();

                if (setting != null)
                {
                    hasFields = true;
                    break;
                }

                var selectAttrs = field.GetCustomAttributes(typeof(EditableSelectAttribute), false);
                var select = selectAttrs.OfType<EditableSelectAttribute>().FirstOrDefault();

                if (select != null)
                {
                    hasFields = true;
                    break;
                }
            }

            var obsoleteAttribute = implementor.GetCustomAttributes(typeof (ObsoleteAttribute), true).FirstOrDefault();

            if (!hasFields && editorInstance == null && obsoleteAttribute == null)
            {
                continue;
            }

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("<b>" + this.GetFancyName(implementor.Name) + " Settings:</b>",
                new GUIStyle { richText = true });

            EditorGUI.indentLevel++;

            if (obsoleteAttribute != null)
            {
                EditorGUILayout.HelpBox(
                    this.GetObsoleteMessage(implementor, obsoleteAttribute), 
                    this.GetObsoleteMessageType(implementor, obsoleteAttribute));
            }

            if (hasFields)
            {
                foreach (var field in implementor.GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    var settingAttrs = field.GetCustomAttributes(typeof (EditableSettingAttribute), false);
                    var setting = settingAttrs.OfType<EditableSettingAttribute>().FirstOrDefault();
                    var settingClamp = field.GetCustomAttributes(typeof(EditableSettingClampAttribute), false).FirstOrDefault() as EditableSettingClampAttribute;

                    if (setting != null && !setting.HideInDefaultFields)
                    {
                        var value = ConfigurationSerializationHelper.Get(
                            settings,
                            implementor,
                            setting.ID,
                            field.FieldType,
                            setting.Default);

                        var settingsStoreType = ConfigurationOptionType.NETSerializedValue;
                        if (field.FieldType == typeof (int))
                        {
                            if (settingClamp != null)
                            {
                                value = EditorGUILayout.IntSlider(
                                    setting.Name,
                                    Safe<int>(value),
                                    (int) settingClamp.Min,
                                    (int) settingClamp.Max);
                            }
                            else
                            {
                                value = EditorGUILayout.IntField(setting.Name, Safe<int>(value));
                            }
                        }
                        else if (field.FieldType == typeof (float))
                        {
                            if (settingClamp != null)
                            {
                                value = EditorGUILayout.Slider(
                                    setting.Name,
                                    Safe<float>(value),
                                    settingClamp.Min,
                                    settingClamp.Max);
                            }
                            else
                            {
                                value = EditorGUILayout.FloatField(setting.Name, Safe<float>(value));
                            }
                        }
                        else if (field.FieldType == typeof (bool))
                        {
                            value = EditorGUILayout.Toggle(setting.Name, Safe<bool>(value));
                        }
                        else if (ConfigurationEditorHelper.IsTypeSupportedByEditorHelper(field.FieldType))
                        {
                            value = ConfigurationEditorHelper.ShowEditorField(setting.Name, value, field.FieldType);
                        }
                        else
                        {
                            EditorGUILayout.LabelField("type not handled for '" + setting.Name + "'");
                        }

                        ConfigurationSerializationHelper.Set(
                            settings,
                            implementor,
                            settingsStoreInstanceName,
                            setting.ID,
                            field.FieldType,
                            value);
                    }

                    var selectAttrs = field.GetCustomAttributes(typeof (EditableSelectAttribute), false);
                    var select = selectAttrs.OfType<EditableSelectAttribute>().FirstOrDefault();

                    if (select != null)
                    {
                        // TODO: have a select for within a controller/implementation
                    }
                }
            }

            if (editorInstance != null)
            {
                editorInstance.OnInspectorGUI(new InternalEditableSettingsStoreAccessor(settings, settingsStoreInstanceName, implementor));
            }

            EditorGUI.indentLevel--;
        }

        foreach (var opt in settings.StoredOptions)
        {
            opt.Active = activeImplementations.Contains(opt.ForImplementation);
        }
    }

    private MessageType GetObsoleteMessageType(Type implementor, object obsoleteAttribute)
    {
        return ((ObsoleteAttribute)obsoleteAttribute).IsError ? MessageType.Error : MessageType.Warning;
    }

    private string GetObsoleteMessage(Type type, object obsoleteAttribute)
    {
        var message = ((ObsoleteAttribute) obsoleteAttribute).Message;

        if (string.IsNullOrEmpty(message))
        {
            message = this.GetFancyName(type.Name) + " is obsolete.";
        }

        return message;
    }

    private class InternalEditableSettingsStoreAccessor : IEditableSettingsStoreAccessor
    {
        private readonly Configuration _configuration;
        private readonly string _settingsStoreInstanceName;
        private readonly Type _implementor;

        public InternalEditableSettingsStoreAccessor(Configuration configuration, string settingsStoreInstanceName, Type implementor)
        {
            _configuration = configuration;
            _settingsStoreInstanceName = settingsStoreInstanceName;
            _implementor = implementor;
        }

        public IEnumerable<string> GetNames()
        {
            return _configuration.StoredOptions.Where(
                    x => x.ForImplementation == _implementor.FullName).Select(x => x.OptionID);
        }

        public T Get<T>(string id, T @default = default(T))
        {
            return (T)ConfigurationSerializationHelper.Get(
                _configuration,
                _implementor,
                id,
                typeof (T),
                @default);
        }

        public void Set<T>(string id, T value)
        {
            ConfigurationSerializationHelper.Set(
                _configuration,
                _implementor,
                _settingsStoreInstanceName,
                id,
                typeof(T),
                value);
        }

        public void Unset(string id)
        {
            var existingValueSet = _configuration.StoredOptions.FirstOrDefault(
                x => x.ForImplementation == _implementor.FullName && x.OptionID == id);

            if (existingValueSet != null)
            {
                _configuration.StoredOptions.Remove(existingValueSet);
            }
        }
    }

    /// <summary>
    /// Gets the fancy (user friendly) name of the type.
    /// </summary>
    /// <returns>The fancy name.</returns>
    /// <param name="name">The original full name of the type.</param>
    private string GetFancyName(string name)
    {
        return NameUtility.GetFancyName(assembly, name);
    }

    /// <summary>
    /// Safely casts the value to the specified type, defaulting to
    /// the default value of the type if the cast fails.
    /// </summary>
    /// <param name="val">The value to cast.</param>
    /// <typeparam name="T">The type to cast to.</typeparam>
    /// <returns>The casted value.</returns>
    public static T Safe<T>(object val)
    {
        try
        {
            return (T)val;
        }
        catch (InvalidCastException)
        {
            return default(T);
        }
        catch (NullReferenceException)
        {
            return default(T);
        }
    }
}
