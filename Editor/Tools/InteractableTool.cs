using System;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

namespace Tools
{
    [EditorTool("Move Control Points", typeof(Interaction_System.Interactable))]
    public class InteractableTool : EditorTool
    {
        [SerializeField] Texture2D toolIcon;

        GUIContent icon;

        public override GUIContent toolbarIcon =>
            icon ??= new GUIContent("Moves the control points", toolIcon, "Control Point");

        void OnEnable()
        {
            (target as Interaction_System.Interactable)?.ToolEnable();
        }

        void OnDisable()
        {
            (target as Interaction_System.Interactable)?.ToolDisable();
        }

        public override void OnToolGUI(EditorWindow window)
        {
            Interaction_System.Interactable interactable = target as Interaction_System.Interactable;
            if (interactable == null)
                return;

            interactable.OnToolGUI(window, interactable);
        }
    }
}