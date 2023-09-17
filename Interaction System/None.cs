using System;
using System.Collections;
using UnityEngine;

namespace Interaction_System
{
    class None : Interaction
    {
        public override void Initialize(Transform camTransform, Transform interactableTransform)
        {
            
        }

        public override IEnumerator OnInteraction(Transform cameraTransform, Transform targetTransform, Action onFinish = null)
        {
            onFinish?.Invoke();
            yield return null;
        }

        public override IEnumerator CancelInteraction(Transform cameraTransform, Transform targetTransform, Action onFinish = null)
        {
            onFinish?.Invoke();
            yield return null;
        }
    }
}