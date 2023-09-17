using System;
using UnityEngine;

namespace Interaction_System
{
    [Serializable]
    internal class MouseMover : InteractableBehaviour
    {
        //[SerializeField] RectTransform crosshair; 
        [SerializeField] CursorLockMode lockMode;
        CursorLockMode _defaultCursorMode;
        bool _defaultVisibility;

        public override bool Finished { get; protected set; } = true;

        public override void StartInteraction()
        {
            _defaultCursorMode = Cursor.lockState;
            Cursor.lockState = lockMode;

            _defaultVisibility = Cursor.visible;
            Cursor.visible = true;
        }

        public override void StopInteraction()
        {
            Cursor.lockState = _defaultCursorMode;
            Cursor.visible = _defaultVisibility;
            //crosshair.anchoredPosition = Vector2.zero;
        }

        public override void OnMouseMove(Vector2 pos, Vector2 delta)
        {
            //crosshair.position = pos;
        }
    }
}