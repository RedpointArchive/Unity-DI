using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(InjectionSource))]
public class InjectionSourcePropertyDrawer : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        Rect headerRect = new Rect(position.x, position.y, position.width, 20);
        EditorGUI.LabelField(headerRect, "Dependency Injection", EditorStyles.boldLabel);
        var y = 16;

        var injectionSource = ((InjectedBehaviour)property.serializedObject.targetObject).InjectionSource;
        if (injectionSource != null)
        {
            if (injectionSource.ImpliedComponents != null)
            {
                for (var i = 0; i < injectionSource.ImpliedComponents.Length; i++)
                {
                    GUI.enabled = false;
                    Rect fieldRect = new Rect(position.x, position.y + y, position.width, 16);
                    EditorGUI.ObjectField(fieldRect, injectionSource.ImpliedComponents[i], typeof(Component));
                    y += 16;
                    GUI.enabled = true;
                }
            }
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var y = 16;

        var injectionSource = ((InjectedBehaviour)property.serializedObject.targetObject).InjectionSource;
        if (injectionSource != null)
        {
            if (injectionSource.ImpliedComponents != null)
            {
                for (var i = 0; i < injectionSource.ImpliedComponents.Length; i++)
                {
                    y += 16;
                }
            }
        }

        return y;
    }
}