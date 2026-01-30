using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SerializableNullable<>), true)]
public class SerializableNullableDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty hasValueProperty = property.FindPropertyRelative("m_HasValue");
        SerializedProperty valueProperty = property.FindPropertyRelative("m_Value");

        float checkBoxSize = EditorGUIUtility.singleLineHeight;

        Rect contentPosition = EditorGUI.PrefixLabel(position, label);

        Rect checkBoxPosition = new Rect(contentPosition.x,
                                         contentPosition.y,
                                         checkBoxSize,
                                         contentPosition.height);
        Rect fieldPosition = new Rect(contentPosition.x + checkBoxSize,
                                      contentPosition.y,
                                      contentPosition.width - checkBoxSize,
                                      contentPosition.height);

        int originalIndentLevel = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;
        hasValueProperty.boolValue = EditorGUI.Toggle(checkBoxPosition, hasValueProperty.boolValue);
        if (hasValueProperty.boolValue) EditorGUI.PropertyField(fieldPosition, valueProperty, GUIContent.none, true);
        else EditorGUI.LabelField(fieldPosition, "<null>");
        EditorGUI.indentLevel = originalIndentLevel;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var hasValueProperty = property.FindPropertyRelative("m_HasValue");
        return
            hasValueProperty.boolValue
                ? EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_Value"), label, true)
                : EditorGUIUtility.singleLineHeight;
    }
}
