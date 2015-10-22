using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ConfigurationCodeGenerator
{
    // WARNING: Do not remove or re-order items in this list!
    // You can **only add** new items to this list!
    public static string[] SupportedTypes = new[]
        {
            "Transform",
            "GameObject",
            "Image",
            "Sprite",
            "Material",
        };

    public static string[] AllowSceneSelect = new[]
        {
            "GameObject",
        };

    [MenuItem("Code Generation/Update Types supported by Settings Store")]
    public static void UpdateTypesSupportedBySettingsStore()
    {
        var classDefinition = GenerateSettingsStoreRuntimeCode();
        var outputPath = Path.Combine(Application.dataPath, "DependencyInjection/Runtime/Configuration.Generated.cs");
        try
        {
            File.WriteAllText(outputPath, classDefinition);
            Debug.LogFormat("Generated runtime code to: " + outputPath);
        }
        catch (Exception ex)
        {
            Debug.LogError("An error occurred while saving the generated runtime code: " + ex);
        }

        classDefinition = GenerateSettingsStoreEditorCode();
        outputPath = Path.Combine(Application.dataPath, "DependencyInjection/Editor/ConfigurationEditor.Generated.cs");
        try
        {
            File.WriteAllText(outputPath, classDefinition);
            Debug.LogFormat("Generated editor code to: " + outputPath);
        }
        catch (Exception ex)
        {
            Debug.LogError("An error occurred while saving the generated editor code: " + ex);
        }

        AssetDatabase.Refresh();
    }

    public static string GenerateSettingsStoreEditorCode()
    {
        var code = @"
// @genera" + @"ted

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class ConfigurationEditorHelper
{
    public static bool IsTypeSupportedByEditorHelper(Type type)
    {
        ";

        foreach (var type in SupportedTypes)
        {
            code += @"
        if (type == typeof(" + type + @"))
	    {
		    return true;
	    }";
        }

        code += @"
        return false;
    }

    public static object ShowEditorField(string name, object value, Type type)
    {
        ";

        foreach (var type in SupportedTypes)
        {
            var supportsSceneSelect = AllowSceneSelect.Contains(type) ? "true" : "false";
            code += @"
        if (type == typeof(" + type + @"))
	    {
		    return EditorGUILayout.ObjectField(name, ConfigurationEditor.Safe<" + type + @">(value),
                typeof (" + type + @"), " + supportsSceneSelect + @");
	    }";
        }

        code += @"
        throw new Exception(""Invalid call to generated ShowEditorField method."");
    }
}
";
        return code;
    }

    public static string GenerateSettingsStoreRuntimeCode()
    {
        var code = @"
// @genera" + @"ted

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Represents the type of value stored in the settings store option.
/// </summary>
public enum ConfigurationOptionType
{
	/// <summary>
	/// A serialized .NET value.
	/// </summary>
	NETSerializedValue = 0,
	";
        
        var i = 1;
        foreach (var type in SupportedTypes)
        {
            code += @"
	/// <summary>
	/// A Unity " + type + @".
	/// </summary>
	" + type + @" = " + (i++) + @",
	";
        }

        code += @"
}

public partial class ConfigurationOption
{
	";
        
        foreach (var type in SupportedTypes)
        {
            code += @"
    /// <summary>
    /// The " + type + @" value, if the field type is " + type + @".
    /// </summary>
    public " + type + @" Option" + type + @";
	";
        }

        code += @"
}

public class ConfigurationSerializationHelper
{
	private static BinaryFormatter _formatter = new BinaryFormatter();

	public static object Get(Configuration settingsStore, Type implementor, string settingId, Type valueType, object @default)
	{
		var existingValueSet = settingsStore.StoredOptions.FirstOrDefault(
					x => x.ForImplementation == implementor.FullName && x.OptionID == settingId);
		object value;
		if (existingValueSet == null)
		{
			value = @default;
		}";

        foreach (var type in SupportedTypes)
        {
            code += @"
        else if (valueType == typeof(" + type + @"))
	    {
		    value = existingValueSet.Option" + type + @";
	    }";
        }

        code += @"
		else if (string.IsNullOrEmpty(existingValueSet.OptionValue))
		{
			value = @default;
		}
		else
		{
			var decodedBytes = Convert.FromBase64String(existingValueSet.OptionValue);
			using (var memory = new MemoryStream(decodedBytes))
			{
				value = _formatter.Deserialize(memory);
			}
		}

		return value;
	}

    public static void RuntimeGet(Dictionary<string, object> dict, ConfigurationOption opt)
    {
        switch (opt.OptionType)
        {
            ";

        foreach (var type in SupportedTypes)
        {
            code += @"
            case ConfigurationOptionType." + type + @":
                dict[opt.OptionID] = opt.Option" + type + @";
                break;
";
        }

        code += @"
            case ConfigurationOptionType.NETSerializedValue:
            default:
                var decodedBytes = Convert.FromBase64String(opt.OptionValue);
                if (decodedBytes.Length != 0)
                {
                    using (var memory = new MemoryStream(decodedBytes))
                    {
                        dict[opt.OptionID] = _formatter.Deserialize(memory);
                    }
                }
                else
                {
                    throw new InvalidOperationException(""Settings store option '"" + opt.OptionID +
                        ""' has an unknown type."");
                }
                break;
        }
    }

	public static void Set(Configuration settingsStore, Type implementor, string instanceId, string settingId, Type valueType, object value)
	{
		var existingValueSet = settingsStore.StoredOptions.FirstOrDefault(
			x => x.ForImplementation == implementor.FullName && x.OptionID == settingId);

		if (existingValueSet == null)
		{
			existingValueSet = new ConfigurationOption
			{
				ForImplementation = implementor.FullName,
				OptionID = settingId,
			};
			settingsStore.StoredOptions.Add(existingValueSet);
		}

		var settingsStoreType = ConfigurationOptionType.NETSerializedValue;
";
        var first = true;
        foreach (var type in SupportedTypes)
        {
            var @else = first ? string.Empty : "else ";
            first = false;

            code += "		";
            code += @else;
            code += @"if (valueType == typeof(" + type + @"))
		{
            settingsStoreType = ConfigurationOptionType." + type + @";
		}
";
        }

        code += @"

        existingValueSet.OptionType = settingsStoreType;
		existingValueSet.OptionValue = null;
		existingValueSet.OptionTransform = null;
		existingValueSet.OptionGameObject = null;
		existingValueSet.OptionImage = null;
		existingValueSet.ForInstance = instanceId;

		switch (settingsStoreType)
		{
            ";
        
        foreach (var type in SupportedTypes)
        {
            code += @"
			case ConfigurationOptionType." + type + @":
				existingValueSet.Option" + type + @" = value as " + type + @";
				break;
";
        }

        code += @"
			case ConfigurationOptionType.NETSerializedValue:
			default:
				string newValue;
				using (var memory = new MemoryStream())
				{
					_formatter.Serialize(memory, value);
					var bytes = new byte[memory.Position];
                    memory.Seek(0, SeekOrigin.Begin);
					memory.Read(bytes, 0, bytes.Length);
					newValue = Convert.ToBase64String(bytes);
				}

				existingValueSet.OptionValue = newValue;
				break;
		}
	}
}

";
        return code;
    }
}
