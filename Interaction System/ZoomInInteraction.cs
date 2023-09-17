using System;
using System.Collections;
using Extras;
using SSSTools.Extensions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Interaction_System
{
    [Serializable]
    internal class ZoomInInteraction : Interaction
    {
        [SerializeField] float travelTimeSeconds = 0.5f;
        [SerializeField] QuadraticBezierCurve path; // TODO: Spline

        Vector3 _startPos;
        Quaternion _startRot;
        
        public override void Initialize(Transform camTransform, Transform interactableTransform)
        {
        }

        public override IEnumerator OnInteraction(Transform cameraTransform, Transform targetTransform, Action onFinish = null)
        {
            float t = 0;

            _startPos = cameraTransform.position;
            _startRot = cameraTransform.rotation;
            path.Start.position = _startPos;
            
            Vector3 target = path.End.position;
            Quaternion targetRotation = path.End.rotation;
            Debug.Log(targetRotation.eulerAngles);

            while (t < 1)
            {   
                cameraTransform.position = path.Evaluate(QuadraticEaseInOut.Evaluate(t));
                cameraTransform.rotation = Quaternion.Slerp(_startRot, targetRotation, QuadraticEaseInOut.Evaluate(t));
                
                yield return null;
                
                t += Time.deltaTime / travelTimeSeconds;
            }

            cameraTransform.position = target;
            cameraTransform.rotation = targetRotation;
            
            onFinish?.Invoke();
        }

        public override IEnumerator CancelInteraction(Transform cameraTransform, Transform targetTransform, Action onFinish = null)
        {
            float t = 0;

            Quaternion startRot = cameraTransform.rotation;

            while (t < 1)
            {   
                cameraTransform.position = path.Evaluate(QuadraticEaseInOut.Evaluate(1 - t));
                cameraTransform.rotation = Quaternion.Slerp(startRot, _startRot, QuadraticEaseInOut.Evaluate(t));
                
                yield return null;
                
                t += Time.deltaTime / travelTimeSeconds;
            }

            cameraTransform.position = _startPos;
            cameraTransform.rotation = _startRot;
            
            onFinish?.Invoke();
        }

#if UNITY_EDITOR
        public override void OnToolGUI(EditorWindow window, Object target)
        {
            path.OnDrawGizmos(target, new QuadraticBezierCurve.DrawOptions(){P0Lock = true});
        }

        public override void ToolEnable()
        {
            Camera main = Camera.main;
            if (main != null)
                path.P0.Position = main.transform.position;
            else
                throw new Exception("No camera in available");
        }
#endif
    }
}