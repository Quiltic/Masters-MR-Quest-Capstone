using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CreateButtons))]
public class EditorUtilities : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        CreateButtons createButtons = (CreateButtons)target;

        if (GUILayout.Button("Apply Default Rocks to List"))
            createButtons.GatherDefaults();
    }
}
