using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Interaction_System
{
    abstract class Interaction
    {
        public abstract void Initialize(Transform camTransform, Transform interactableTransform);
        public abstract IEnumerator OnInteraction(Transform cameraTransform, Transform targetTransform, Action onFinish = null);
        public abstract IEnumerator CancelInteraction(Transform cameraTransform, Transform targetTransform, Action onFinish = null);

        #if UNITY_EDITOR
        
        public virtual void OnToolGUI(UnityEditor.EditorWindow window, Object target) { }
        
        public virtual void ToolEnable() { }
        
        public virtual void ToolDisable() { }
        
        #endif
    }
}