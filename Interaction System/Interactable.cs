using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SSSTools.FunText.Attributes;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Interaction_System
{
    public class Interactable : MonoBehaviour
    {
        [SerializeReference, ChooseReference] List<InteractableBehaviour> behaviours =  new List<InteractableBehaviour>()
        {
            new MouseMover(),
        };

        [SerializeReference, ChooseReference] Interaction _interaction = new MoveTowardsInteraction();

        Transform _transform;
        Transform _cameraTransform;
        
        void Awake()
        {
            _transform = transform;
            _cameraTransform = Camera.main!.transform;
        }

        void Start()
        {
            behaviours.ForEach(b => b.Initialize(_transform, _cameraTransform));
            _interaction.Initialize(_cameraTransform, _transform);
        }

        public void Interact(Action onFinish = null)
        {
            StopAllCoroutines();
            StartCoroutine(_interaction.OnInteraction(_cameraTransform, _transform, () =>
            {
                onFinish?.Invoke();
                behaviours.ForEach(b => b.StartInteraction());
            }));
        }
        
        public void CancelInteraction(Action onFinish = null)
        {
            StopAllCoroutines();
            StartCoroutine(_interaction.CancelInteraction(_cameraTransform, _transform, () =>
            {
                onFinish?.Invoke();
                behaviours.ForEach(b => b.StopInteraction());
            }));
        }

        public bool IsFinished() => behaviours.All(b => b.Finished);

        #region Interaction Behaviour

        public void OnLeftMouse(Vector2 pos) => behaviours.ForEach(b => b.OnLeftMouse(pos));
        public void OnRightMouse(Vector2 pos) => behaviours.ForEach(b => b.OnRightMouse(pos));
        public void OnMouseMove(Vector2 pos, Vector2 delta) => behaviours.ForEach(b => b.OnMouseMove(pos, delta));
        public void OnLeftMouseDrag(Vector2 pos, Vector2 delta) => behaviours.ForEach(b => b.OnLeftMouseDrag(pos, delta));
        public void OnLeftMouseUp(Vector2 pos) => behaviours.ForEach(b => b.OnLeftMouseUp(pos));
        public void OnRightMouseDrag(Vector2 pos, Vector2 delta) => behaviours.ForEach(b => b.OnRightMouseDrag(pos, delta));
        public void OnRightMouseUp(Vector2 pos) => behaviours.ForEach(b => b.OnRightMouseUp(pos));

        public bool TryGetBehaviour<T>(out T behaviour) where T : InteractableBehaviour
        {
            foreach (InteractableBehaviour interactableBehaviour in behaviours.Where(interactableBehaviour => interactableBehaviour.GetType() == typeof(T)))
            {
                behaviour = (T) interactableBehaviour;
                return true;
            }
            
            behaviour = default;
            return false;
        }
        
        #endregion

        #if UNITY_EDITOR
        
        public void OnToolGUI(UnityEditor.EditorWindow window, Object target)
        {
            behaviours.ForEach(b => b.OnToolGUI(window));
            _interaction.OnToolGUI(window, target);
        }

        public void ToolEnable()
        {
            _interaction.ToolEnable();
        }

        public void ToolDisable()
        {
            _interaction.ToolDisable();
        }
        
        #endif
    }
}