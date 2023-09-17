using System;
using System.Collections;
using UnityEngine;

namespace Interaction_System
{
    [Serializable]
    internal class MoveTowardsInteraction : Interaction
    {
        [SerializeField] float travelTimeSeconds = 0.5f;
        [SerializeField] float stopDistance = 2.5f;
        [SerializeField] Quaternion targetRotation = Quaternion.identity;

        Vector3 _startPos;
        Quaternion _startRot;
        
        public override void Initialize(Transform camTransform, Transform interactableTransform)
        {
            _startPos = interactableTransform.position;
            _startRot = interactableTransform.rotation;
        }

        public override IEnumerator OnInteraction(Transform cameraTransform, Transform targetTransform, Action onFinish = null)
        {
            float t = 0;
            Vector3 startPos = targetTransform.position;
            Quaternion startRot = targetTransform.rotation;

            Vector3 targetPos = cameraTransform.position + cameraTransform.forward * stopDistance;
            
            while (t < 1)
            {   
                // TODO: Consider using a smoother interpolation
                targetTransform.position = Vector3.Lerp(startPos, targetPos, t);
                targetTransform.rotation = Quaternion.Slerp(startRot, targetRotation, t);
                
                yield return null;
                
                t += Time.deltaTime / travelTimeSeconds;
            }

            targetTransform.position = targetPos;
            targetTransform.rotation = targetRotation;
            
            onFinish?.Invoke();
        }

        public override IEnumerator CancelInteraction(Transform cameraTransform, Transform targetTransform, Action onFinish = null)
        {
            float t = 0;
            Vector3 startPos = targetTransform.position;
            Quaternion startRot = targetTransform.rotation;
            
            while (t < 1)
            {   
                // TODO: Consider using a smoother interpolation
                targetTransform.position = Vector3.Lerp(startPos, _startPos, t);
                targetTransform.rotation = Quaternion.Slerp(startRot, _startRot, t);
                
                yield return null;
                
                t += Time.deltaTime / travelTimeSeconds;
            }

            targetTransform.position = _startPos;
            targetTransform.rotation = _startRot;
            
            onFinish?.Invoke();
        }
    }
}