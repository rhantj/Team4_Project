using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ReadOnlyAttribute), true)]
public class ReadOnlyAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        bool isRuntimeOnlyReadOnly = (attribute as ReadOnlyAttribute)?.IsRuntimeOnlyReadOnly ?? false;
        GUI.enabled = isRuntimeOnlyReadOnly && !Application.isPlaying;
        EditorGUI.PropertyField(position, property, label);
        GUI.enabled = true;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
}
