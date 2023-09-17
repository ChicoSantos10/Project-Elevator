using Attributes;
using UnityEditor;
using UnityEngine;

namespace Drawers
{
    [CustomPropertyDrawer(typeof(ExposeScriptableObjectAttribute), true)]
    public class ExposeScriptableObjectAttributeDrawer : PropertyDrawer
    {
        Editor _editor = null;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position,property, label, true);

            if (property.objectReferenceValue != null)
                property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, GUIContent.none);
            else
                return;

            if (!property.isExpanded) 
                return;
            
            EditorGUI.indentLevel++;
                
            if (!_editor)
                Editor.CreateCachedEditor(property.objectReferenceValue, null, ref _editor);
                
            _editor.OnInspectorGUI();

            EditorGUI.indentLevel--;
        }
    }
}