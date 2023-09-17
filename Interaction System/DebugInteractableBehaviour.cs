using UnityEngine;

namespace Interaction_System
{
    public class DebugInteractableBehaviour : InteractableBehaviour
    {
        public override bool Finished { get; protected set; } = false;

        protected override void OnInitialize()
        {
            Debug.Log("Initialized");
        }

        public override void StartInteraction()
        {
            Debug.Log("Started Interacting");
        }

        public override void StopInteraction()
        {
            Debug.Log("On Stop Interaction");
        }

        public override void OnLeftMouse(Vector2 pos)
        {
            Debug.Log($"On Left Mouse at: {pos}");
        }

        public override void OnRightMouse(Vector2 pos)
        {
            Debug.Log($"On Right Mouse at: {pos}");
        }

        public override void OnMouseMove(Vector2 pos, Vector2 delta)
        {
            Debug.Log($"On Mouse Move from: {pos - delta} to {pos}");
        }

        public override void OnLeftMouseDrag(Vector2 pos, Vector2 delta)
        {
            Debug.Log($"On Left Mouse Drag from: {pos - delta} to {pos}");
        }

        public override void OnRightMouseDrag(Vector2 pos, Vector2 delta)
        {
            Debug.Log($"On Right Mouse Drag from: {pos - delta} to {pos}");
        }
    }
}