
// @generated

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
        
        if (type == typeof(Transform))
	    {
		    return true;
	    }
        if (type == typeof(GameObject))
	    {
		    return true;
	    }
        if (type == typeof(Image))
	    {
		    return true;
	    }
        if (type == typeof(Sprite))
	    {
		    return true;
	    }
        if (type == typeof(Material))
	    {
		    return true;
	    }
        return false;
    }

    public static object ShowEditorField(string name, object value, Type type)
    {
        
        if (type == typeof(Transform))
	    {
		    return EditorGUILayout.ObjectField(name, ConfigurationEditor.Safe<Transform>(value),
                typeof (Transform), false);
	    }
        if (type == typeof(GameObject))
	    {
		    return EditorGUILayout.ObjectField(name, ConfigurationEditor.Safe<GameObject>(value),
                typeof (GameObject), true);
	    }
        if (type == typeof(Image))
	    {
		    return EditorGUILayout.ObjectField(name, ConfigurationEditor.Safe<Image>(value),
                typeof (Image), false);
	    }
        if (type == typeof(Sprite))
	    {
		    return EditorGUILayout.ObjectField(name, ConfigurationEditor.Safe<Sprite>(value),
                typeof (Sprite), false);
	    }
        if (type == typeof(Material))
	    {
		    return EditorGUILayout.ObjectField(name, ConfigurationEditor.Safe<Material>(value),
                typeof (Material), false);
	    }
        throw new Exception("Invalid call to generated ShowEditorField method.");
    }
}
