using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Represents a stored option for a dependency injection implementation.
/// </summary>
[Serializable]
public partial class ConfigurationOption
{
    /// <summary>
    /// The full type name of the implementation this option is for.
    /// </summary>
    public string ForImplementation;

    /// <summary>
    /// The full unique name of the settings store instance this option is for.
    /// </summary>
    public string ForInstance;

    /// <summary>
    /// The unique identifier of this option.
    /// </summary>
    public string OptionID;

    /// <summary>
    /// The base64-encoded, .NET serialized value of this option.
    /// </summary>
    /// <remarks>
    /// We have to store a base64-encoded string because the Unity serializer will not
    /// serialize fields if they are of type 'object', even if the underlying value itself
    /// can be serialized.
    /// </remarks>
    public string OptionValue;

    /// <summary>
    /// Whether the settings store option is actively used (e.g. the implementation
    /// is the currently selected one).
    /// </summary>
    public bool Active;

    /// <summary>
    /// The type of data being stored in this option.
    /// </summary>
    public ConfigurationOptionType OptionType;
}
