
// @generated

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
	
	/// <summary>
	/// A Unity Transform.
	/// </summary>
	Transform = 1,
	
	/// <summary>
	/// A Unity GameObject.
	/// </summary>
	GameObject = 2,
	
	/// <summary>
	/// A Unity Image.
	/// </summary>
	Image = 3,
	
	/// <summary>
	/// A Unity Sprite.
	/// </summary>
	Sprite = 4,
	
	/// <summary>
	/// A Unity Material.
	/// </summary>
	Material = 5,
	
}

public partial class ConfigurationOption
{
	
    /// <summary>
    /// The Transform value, if the field type is Transform.
    /// </summary>
    public Transform OptionTransform;
	
    /// <summary>
    /// The GameObject value, if the field type is GameObject.
    /// </summary>
    public GameObject OptionGameObject;
	
    /// <summary>
    /// The Image value, if the field type is Image.
    /// </summary>
    public Image OptionImage;
	
    /// <summary>
    /// The Sprite value, if the field type is Sprite.
    /// </summary>
    public Sprite OptionSprite;
	
    /// <summary>
    /// The Material value, if the field type is Material.
    /// </summary>
    public Material OptionMaterial;
	
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
		}
        else if (valueType == typeof(Transform))
	    {
		    value = existingValueSet.OptionTransform;
	    }
        else if (valueType == typeof(GameObject))
	    {
		    value = existingValueSet.OptionGameObject;
	    }
        else if (valueType == typeof(Image))
	    {
		    value = existingValueSet.OptionImage;
	    }
        else if (valueType == typeof(Sprite))
	    {
		    value = existingValueSet.OptionSprite;
	    }
        else if (valueType == typeof(Material))
	    {
		    value = existingValueSet.OptionMaterial;
	    }
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
            
            case ConfigurationOptionType.Transform:
                dict[opt.OptionID] = opt.OptionTransform;
                break;

            case ConfigurationOptionType.GameObject:
                dict[opt.OptionID] = opt.OptionGameObject;
                break;

            case ConfigurationOptionType.Image:
                dict[opt.OptionID] = opt.OptionImage;
                break;

            case ConfigurationOptionType.Sprite:
                dict[opt.OptionID] = opt.OptionSprite;
                break;

            case ConfigurationOptionType.Material:
                dict[opt.OptionID] = opt.OptionMaterial;
                break;

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
                    throw new InvalidOperationException("Settings store option '" + opt.OptionID +
                        "' has an unknown type.");
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
		if (valueType == typeof(Transform))
		{
            settingsStoreType = ConfigurationOptionType.Transform;
		}
		else if (valueType == typeof(GameObject))
		{
            settingsStoreType = ConfigurationOptionType.GameObject;
		}
		else if (valueType == typeof(Image))
		{
            settingsStoreType = ConfigurationOptionType.Image;
		}
		else if (valueType == typeof(Sprite))
		{
            settingsStoreType = ConfigurationOptionType.Sprite;
		}
		else if (valueType == typeof(Material))
		{
            settingsStoreType = ConfigurationOptionType.Material;
		}


        existingValueSet.OptionType = settingsStoreType;
		existingValueSet.OptionValue = null;
		existingValueSet.OptionTransform = null;
		existingValueSet.OptionGameObject = null;
		existingValueSet.OptionImage = null;
		existingValueSet.ForInstance = instanceId;

		switch (settingsStoreType)
		{
            
			case ConfigurationOptionType.Transform:
				existingValueSet.OptionTransform = value as Transform;
				break;

			case ConfigurationOptionType.GameObject:
				existingValueSet.OptionGameObject = value as GameObject;
				break;

			case ConfigurationOptionType.Image:
				existingValueSet.OptionImage = value as Image;
				break;

			case ConfigurationOptionType.Sprite:
				existingValueSet.OptionSprite = value as Sprite;
				break;

			case ConfigurationOptionType.Material:
				existingValueSet.OptionMaterial = value as Material;
				break;

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

