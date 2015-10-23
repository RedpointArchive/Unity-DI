using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ConfigurationVerifier), true)]
public class ConfigurationVerifierInfo : Editor
{
    public override void OnInspectorGUI()
    {
        GUILayout.Label(@"
NEWLINEThis game object is used to verify that all code dependencies
are valid on game start.  This verification only applies when
running the game inside the editor.
NEWLINE
NEWLINEYou can delete this game object to remove verification when
this scene starts.  You can use 'Verify Configuration on Game Start'
from the 'Dependency Injection' menu item to create this
verification object again.NEWLINE
".Trim().Replace("\n", " ").Replace("\r", "").Replace("NEWLINE", "\n"), new GUIStyle { wordWrap = true });
    }
}