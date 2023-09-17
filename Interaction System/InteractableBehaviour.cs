using System;
using System.Collections;
using Robot;
using UnityEngine;

namespace Interaction_System
{
    [Serializable]
    public abstract class InteractableBehaviour
    {
        /// <summary>
        /// Interactable transform
        /// </summary>
        protected Transform Transform { get; private set; }
        protected Transform CameraTransform { get; private set; }

        public abstract bool Finished { get; protected set; }
        
        public void Initialize(Transform transform, Transform cameraTransform)
        {
            Transform = transform;
            CameraTransform = cameraTransform;
            
            OnInitialize();
        }
        
        protected virtual void OnInitialize() { }

        public virtual IEnumerator StartInteractionCoroutine()
        {
            yield return null;
        }

        public virtual IEnumerator StopInteractionCoroutine()
        {
            yield return null;
        }

        public virtual void StartInteraction() { }

        public virtual void StopInteraction() { }

        public virtual void OnLeftMouse(Vector2 pos) { } 

        public virtual void OnRightMouse(Vector2 pos) { }

        public virtual void OnMouseMove(Vector2 pos, Vector2 delta) { }

        public virtual void OnLeftMouseDrag(Vector2 pos, Vector2 delta) { }
        
        public virtual void OnLeftMouseUp(Vector2 pos) { }
        
        public virtual void OnRightMouseDrag(Vector2 pos, Vector2 delta) { }

        public virtual void OnRightMouseUp(Vector2 pos) { }

        protected void OnFail()
        {
            RobotMovement.ClosestRobotToTarget(Transform.gameObject);
        }

        #if UNITY_EDITOR
        public virtual void OnToolGUI(UnityEditor.EditorWindow window) { }
        #endif
    }
}