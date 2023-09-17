using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using SSSTools.FunText.Attributes;
using SSSTools.FunText.Core;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SSSTools.Editor
{
    [CustomPropertyDrawer(typeof(ChooseReferenceAttribute), true)]
    public class ChooseReferenceDrawer : PropertyDrawer
    {
        bool _isExpanded;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Type fieldType = typeof(IEnumerable).IsAssignableFrom(fieldInfo.FieldType)
                ? fieldInfo.FieldType.GetGenericArguments()[0]
                : fieldInfo.FieldType;

            Type[] types = TypeCache.GetTypesDerivedFrom(fieldType).Where(t => !(t.IsAbstract || t.IsInterface || t == typeof(Object))).ToArray();
            
            GenericMenu menu = new GenericMenu();
            foreach (Type type in types)
            {
                menu.AddSeparator(type.Namespace + "/");
                menu.AddItem(new GUIContent($"{type.Namespace}/{ObjectNames.NicifyVariableName(type.Name)}"), false, o => UpdateReference(o, property), type);
            }
            
            Rect pos = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            _isExpanded = EditorGUI.Foldout(pos, _isExpanded, GUIContent.none);

            pos = EditorGUI.PrefixLabel(pos, label);

            string fullName = property.managedReferenceFullTypename.Substring(property.managedReferenceFullTypename.IndexOf('.') + 1);
            
            string typeName = ObjectNames.NicifyVariableName(fullName);
            if(EditorGUI.DropdownButton(pos, new GUIContent($"Chosen Class: {typeName}"), FocusType.Keyboard))
                menu.DropDown(pos);

            if (!_isExpanded) 
                return;
            
            EditorGUI.indentLevel++;

            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            Color color = GUI.color;
            GUI.color = Color.green;
            
            EditorGUI.PropertyField(position, property, new GUIContent(typeName), true);
            
            GUI.color = color;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!_isExpanded)
                return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            
            return EditorGUI.GetPropertyHeight(property, true) + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        static void UpdateReference(object o, SerializedProperty prop)
        {
            prop.serializedObject.Update();
            Type chosen = (Type) o;
            try
            {
                prop.managedReferenceValue = Activator.CreateInstance(chosen);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
                prop.managedReferenceValue = FormatterServices.GetUninitializedObject(chosen);
            }

            prop.serializedObject.ApplyModifiedProperties();
        }
    }
}
