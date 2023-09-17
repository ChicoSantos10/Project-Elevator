using System;
using System.Collections.Generic;
using SSSTools.Extensions;
using UnityEngine;

namespace Puzzles
{
    public class Piece : MonoBehaviour
    {
        public const float CubeSize = 0.045f;
        
        [Serializable]
        struct Structure
        {
            [SerializeField, Tooltip("How many cubes")] int width, height;

            public float Width => width * CubeSize;
            public float Height => height * CubeSize;
        }
        
        [SerializeField] Structure structure;
        [SerializeField] Vector3 offset;
        [SerializeField, HideInInspector] Bounds bounds;

        readonly HashSet<Collider> _collisions = new HashSet<Collider>();

        public Vector3 InitialPosition { get; private set; }
        
        public Vector3 Offset => offset * CubeSize;

        public Bounds Bounds => bounds;

        public Bounds RotatedBounds => bounds.Rotate();

        public bool Collided => _collisions.Count > 1; // We always collider with the puzzle itself
        
        public bool Placed { get; set; }

        [ContextMenu("Initialize")]
        void Start()
        {
            bounds = new Bounds(transform.position, ComputeBoundsSize());
            InitialPosition = transform.position;
        }

        [ContextMenu("Rotate")]
        public void Rotate()
        {
            (offset.x, offset.z) = (offset.z, offset.x);

            bounds = bounds.Rotate();
            
            transform.Rotate(0, 90f, 0, Space.World);
        }

        public void Move(Vector3 position)
        {
            bounds.center = position;
            transform.position = position;
        }

        void OnTriggerEnter(Collider other)
        {
            if (!_collisions.Contains(other))
                _collisions.Add(other);
        }

        void OnTriggerExit(Collider other)
        {
            _collisions.Remove(other);
        }

        Vector3 ComputeBoundsSize() => new Vector3(structure.Width, CubeSize, structure.Height);
        
        #if UNITY_EDITOR

        public Vector3 testOffset = new Vector3(0.01f, 0, 0.03f);
        
        void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireCube(Bounds.center, Bounds.size);
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(RotatedBounds.center, RotatedBounds.size);
            Gizmos.DrawSphere(transform.position + Offset, 0.025f);
            //Gizmos.DrawSphere(Board.SnapPosition(transform.position + testOffset, transform.position + new Vector3(Piece.CubeSize, 0, Piece.CubeSize) * 0.5f), 0.025f);
        }
#endif
    }
}