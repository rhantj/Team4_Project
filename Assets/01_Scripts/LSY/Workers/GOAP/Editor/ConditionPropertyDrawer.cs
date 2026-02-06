using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(GOAP.ICondition), true)]
public class ConditionPropertyDrawer : PropertyDrawer
{
    private static readonly string[] typeNames = GOAP.ICondition.allowedTypes.Select(t => t.Name).ToArray();

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var managedReferenceValue = property.managedReferenceValue;
        int previousIndex = -1;

        if (null != managedReferenceValue)
        {
            Type conditionType = managedReferenceValue.GetType();
            if (conditionType.IsGenericType && typeof(GOAP.Condition<>) == conditionType.GetGenericTypeDefinition())
            {
                Type comparandType = conditionType.GetGenericArguments()[0];
                previousIndex = Array.IndexOf(typeNames, comparandType);
            }
        }

        Rect rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        int newIndex = EditorGUI.Popup(rect, label.text, previousIndex, typeNames);

        if (0 <= newIndex && newIndex != previousIndex)
        {
            Type selectedType = GOAP.ICondition.allowedTypes[newIndex];
            Type conditionType = typeof(GOAP.Condition<>).MakeGenericType(selectedType);
            property.managedReferenceValue = Activator.CreateInstance(conditionType);
        }

        if (null != property.managedReferenceValue)
        {
            SerializedProperty childProperty = property.Copy();
            SerializedProperty endProperty = childProperty.GetEndProperty();
            childProperty.NextVisible(true);
            float y = position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            while (childProperty.propertyPath != endProperty.propertyPath)
            {
                float childHeight = EditorGUI.GetPropertyHeight(childProperty, true);
                EditorGUI.PropertyField(new Rect(position.x, y, position.width, childHeight), childProperty, true);
                y += childHeight + EditorGUIUtility.standardVerticalSpacing;
                if (!childProperty.NextVisible(false)) break;
            }
        }
    }
}
