using UnityEngine;

namespace Interaction_System
{
    public class RotationBehaviour : InteractableBehaviour
    {
        Vector3 _startPos;
        Quaternion _startRot;

        public override bool Finished { get; protected set; } = false;

        protected override void OnInitialize()
        {
            _startPos = Transform.position;
            _startRot = Transform.rotation;
        }

        public override void OnLeftMouseDrag(Vector2 pos, Vector2 delta)
        {
            Transform.Rotate(CameraTransform.up * -delta.x,  Space.World);
            Transform.Rotate(CameraTransform.right * delta.y,  Space.World);
        }

        public override void StopInteraction()
        {
            Transform.position = _startPos;
            Transform.rotation = _startRot;
        }
    }
}