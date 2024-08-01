using System.Reflection;
using UnityEditor;
using UnityEngine;

// GUI of Brush Setting.cs
[CustomEditor(typeof(BrushSettingChanger))]
public class BrushSettingEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // Add "Apply Settings" Button
        if (GUILayout.Button("Apply Settings"))
        {
            var component = target as BrushSettingChanger;
            component.ChangeBrushSetting();
        }
    }
}
