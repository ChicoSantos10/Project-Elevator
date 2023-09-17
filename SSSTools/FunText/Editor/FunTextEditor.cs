using System;
using SSSTools.FunText.Core;
using UnityEditor;
using UnityEngine;

namespace SSSTools.FunText.Editor
{
    [CustomEditor(typeof(FunTextAnimator))]
    public class FunTextEditor : UnityEditor.Editor
    {
        Texture _logo;

        void OnEnable()
        {
            _logo = (Texture) EditorGUIUtility.Load("SSSTools/FunText/SSSTools_FunTextLogo.png");
        }

        public override void OnInspectorGUI()
        {
            //GUILayout.Box(_logo);
            
            base.OnInspectorGUI();
        }
    }
}
