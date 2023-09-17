using System;
using UnityEngine;

namespace Extras
{
    [Serializable]
    public class ControlPoint
    {
        // TODO: Use only position and rotation instead of transform??
        [SerializeField] Transform transform;

        public ControlPoint(Transform transform)
        {
            this.transform = transform;
        }

        public Transform Transform => transform;

        public Vector3 Position
        {
            get => transform.position;
            set => transform.position = value;
        }

        public Quaternion Rotation
        {
            get => transform.rotation;
            set => transform.rotation = value;
        }

        public static implicit operator Transform(ControlPoint cp) => cp.Transform;
    }
}