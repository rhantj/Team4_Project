using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace BehaviourTree
{
    [CustomPropertyDrawer(typeof(INode), true)]
    public class INodeDrawer : PropertyDrawer
    {
        private const string m_NullPlaceHolder = "<Null>";

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            float singleLineHeight = EditorGUIUtility.singleLineHeight;
            float standardVerticalSpacing = EditorGUIUtility.standardVerticalSpacing;
            float labelWidth = EditorGUIUtility.labelWidth;

            List<Type> types = GetOrderedNodeTypeList();
            List<string> popupPaths = new List<string> { m_NullPlaceHolder };

            int previousPopupIndexWithNullOption = 0;
            string typeName = property?.managedReferenceFullTypename;
            typeName = string.IsNullOrEmpty(typeName) ? m_NullPlaceHolder : typeName.Split(' ', '.').Last();

            for (int i = 0; i < types.Count; i++)
            {
                Type t = types[i];
                string category = GetCategory(t);
                string path = string.IsNullOrEmpty(category) ? t.Name : $"{category}/{t.Name}";
                popupPaths.Add(path);
                if (typeName == t.Name) previousPopupIndexWithNullOption = i + 1;
            }

            Rect labelPosition = new Rect(position.x,              position.y, labelWidth,                  singleLineHeight);
            Rect popupPosition = new Rect(position.x + labelWidth, position.y, position.width - labelWidth, singleLineHeight);

            if (property.hasVisibleChildren) property.isExpanded = EditorGUI.Foldout(labelPosition, property.isExpanded, label, true);
            else EditorGUI.LabelField(labelPosition, label);

            int currentIndentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            int newPopupIndexWithNullOption = EditorGUI.Popup(popupPosition, previousPopupIndexWithNullOption, popupPaths.ToArray());
            EditorGUI.indentLevel = currentIndentLevel;

            if (newPopupIndexWithNullOption != previousPopupIndexWithNullOption)
            {
                if (0 == newPopupIndexWithNullOption) property.managedReferenceValue = null;
                else                                  property.managedReferenceValue = Activator.CreateInstance(types[newPopupIndexWithNullOption - 1]);
            }

            if (property.isExpanded && property.hasVisibleChildren)
            {
                EditorGUI.indentLevel++;

                SerializedProperty iterator = property.Copy();
                SerializedProperty endProperty = iterator.GetEndProperty();
                bool isEnteringChildren = true;
                float y = position.y + singleLineHeight + standardVerticalSpacing;

                while (iterator.NextVisible(isEnteringChildren) && !SerializedProperty.EqualContents(iterator, endProperty))
                {
                    isEnteringChildren = false;
                    float childHeight = EditorGUI.GetPropertyHeight(iterator, true);
                    Rect childPosition = new Rect(position.x, y, position.width, childHeight);

                    bool isDrawnAsArray = iterator.isArray && iterator.propertyType != SerializedPropertyType.String;
                    if (isDrawnAsArray)
                    {
                        Rect indentedChildPosition = EditorGUI.IndentedRect(childPosition);
                        int originalIndentLevel = EditorGUI.indentLevel;
                        EditorGUI.indentLevel = 0;
                        EditorGUI.PropertyField(indentedChildPosition, iterator, true);
                        EditorGUI.indentLevel = originalIndentLevel;
                    }
                    else
                    {
                        EditorGUI.PropertyField(childPosition, iterator, true);
                    }
                    y += childHeight + standardVerticalSpacing;
                }

                EditorGUI.indentLevel--;
            }
        }

        private static List<Type> GetOrderedNodeTypeList()
        {
            return TypeCache.GetTypesDerivedFrom<INode>()
                .Where(t => !t.IsAbstract && !t.IsInterface && t.IsSerializable)
                .OrderBy(t => GetCategory(t))
                .ThenBy(t => t.AssemblyQualifiedName)
                .ToList();
        }

        private static string GetCategory(Type t)
        {
            if (typeof(Compositor).IsAssignableFrom(t)) return "1-Compositor";
            if (typeof(Modifier).IsAssignableFrom(t)) return "2-Modifier";
            return null;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float singleLineHeight = EditorGUIUtility.singleLineHeight;
            float standardVerticalSpacing = EditorGUIUtility.standardVerticalSpacing;
            float labelWidth = EditorGUIUtility.labelWidth;
            float height = singleLineHeight;

            if (property.isExpanded && property.hasVisibleChildren)
            {
                SerializedProperty iterator = property.Copy();
                SerializedProperty endProperty = iterator.GetEndProperty();
                bool isEnteringChildren = true;

                while (iterator.NextVisible(isEnteringChildren) && !SerializedProperty.EqualContents(iterator, endProperty))
                {
                    isEnteringChildren = false;
                    float childHeight = EditorGUI.GetPropertyHeight(iterator, true);
                    height += standardVerticalSpacing + childHeight;
                }
                height += standardVerticalSpacing;

            }

            return height;
        }
    }
}
