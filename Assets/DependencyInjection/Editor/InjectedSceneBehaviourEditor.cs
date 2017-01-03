using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InjectedSceneBehaviour))]
public class InjectedSceneBehaviourEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Validate"))
        {
            ((InjectedSceneBehaviour)serializedObject.targetObject).Update();
        }
    }
}
