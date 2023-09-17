using System;
using State_Machine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Interaction_System
{
    [Serializable]
    internal class InteractionState : IState
    {
        [SerializeField, Range(0.1f, 1f)] float rotationSpeed = 0.2f;
        [SerializeField] EventChannel onStartInteractionChannel;
        [SerializeField] EventChannel onStopInteractionChannel;

        InputReader _input;
        
        public Interactable Selected { get; set; }

        public event UnityAction OnFinish = delegate { };

        public float RotationSpeed
        {
            get => rotationSpeed;
            set => rotationSpeed = value;
        }

        public InputReader Input 
        { 
            get => _input;
            set => _input = value;
        }

        public void OnEnter()
        {
            Selected.Interact(_input.EnableInteraction);
            _input.DisableGameplay();
            onStartInteractionChannel.Invoke();
        }

        public void OnExit()
        {
            Selected.CancelInteraction(EnableInputs);
            onStopInteractionChannel.Invoke();

            void EnableInputs()
            {
                _input.EnableInteraction();
                _input.EnableGameplay();
            }
        }

        public void Update()
        {
            Debug.Assert(Selected != null, "An interactable should be currently selected when interacting");

            if (Selected.IsFinished())
            {
                OnFinish.Invoke();
                return;
            }
            
            Mouse mouse = Mouse.current;
            Vector2 delta = mouse.delta.ReadValue() * RotationSpeed;
            Vector2 pos = mouse.position.ReadValue();

            bool moved = delta != Vector2.zero;

            if (moved)
                Selected!.OnMouseMove(pos, delta);

            if (mouse.leftButton.isPressed)
            {
                if (mouse.leftButton.wasPressedThisFrame)
                    Selected!.OnLeftMouse(pos);

                if (moved)
                    Selected!.OnLeftMouseDrag(pos, delta);
            }
            else if (mouse.leftButton.wasReleasedThisFrame)
                Selected!.OnLeftMouseUp(pos);

            if (mouse.rightButton.isPressed)
            {
                if (mouse.rightButton.wasPressedThisFrame)
                    Selected!.OnRightMouse(pos);

                if (moved)
                    Selected!.OnRightMouseDrag(pos, delta);
            }
            else if (mouse.rightButton.wasReleasedThisFrame)
                Selected!.OnRightMouseUp(pos);
        }
    }
}