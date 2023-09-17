using System;
using UnityEngine;

namespace Puzzles
{
    [ExecuteAlways]
    public class Board : MonoBehaviour
    {
        const int size = 8;

        Bounds _bounds;
        
        void Start()
        {
            _bounds = new Bounds(transform.position, ComputeBounds());
        }

        Vector3 ComputeBounds() => new Vector3(1.1f,0.1f,1.1f) * size * Piece.CubeSize;

        public Vector3 SnapPosition(Vector3 pos)
        {
            if (!_bounds.Contains(pos))
                return pos;

            Vector3 offset = transform.position + new Vector3(Piece.CubeSize, 0, Piece.CubeSize) * 0.5f;
            return SnapPosition(pos, offset);
        }

        public static Vector3 SnapPosition(Vector3 pos, Vector3 offset)
        {
            pos -= offset;
            
            pos.x = Mathf.Round(pos.x / Piece.CubeSize) * Piece.CubeSize;
            pos.z = Mathf.Round(pos.z / Piece.CubeSize) * Piece.CubeSize;

            return pos + offset;
        }

        public bool ContainsPoint(Vector3 pos) => _bounds.Contains(pos);
        public bool Intersects(Bounds bounds) => _bounds.Intersects(bounds);
        public bool ContainsBounds(Bounds bounds) => _bounds.min.x <= bounds.min.x && _bounds.min.z <= bounds.min.z &&
                                                    _bounds.max.x >= bounds.max.x && _bounds.max.z >= bounds.max.z;

        /*
         * Vector3 topLeft = transform.position + new Vector3(size * 0.5f, 0, -size * 0.5f) * Piece.CubeSize;
            topLeft.x -= Piece.CubeSize * 0.5f;
            topLeft.z += Piece.CubeSize * 0.5f;
            //Vector3 offset = transform.position + new Vector3(Piece.CubeSize, 0, Piece.CubeSize) * 0.5f;
            pos -= topLeft;// + new Vector3(Piece.CubeSize, 0, Piece.CubeSize) * 0.5f;
            
            float difference = pos.x % Piece.CubeSize;
            pos.x -= difference;
            
            if (difference > Piece.CubeSize * 0.5f)
            {
                pos.x += Piece.CubeSize;
            }
            
            difference = pos.z % Piece.CubeSize;
            pos.z -= difference;
            
            if (difference > Piece.CubeSize * 0.5f)
            {
                pos.z += Piece.CubeSize;
            }

            return pos + topLeft;// - new Vector3(Piece.CubeSize, 0, Piece.CubeSize) * 0.5f;
         */

#if UNITY_EDITOR

        [SerializeField] Vector3 position = new Vector3(0.12f, 0f, 0.056f);
        
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, ComputeBounds());
            
            /*
            Gizmos.color = Color.blue;
            Vector3 topLeft = transform.position + new Vector3(size * 0.5f, 0, -size * 0.5f) * Piece.CubeSize;
            topLeft.x -= Piece.CubeSize * 0.5f;
            topLeft.z += Piece.CubeSize * 0.5f;
            Gizmos.DrawSphere(topLeft, 0.01f);

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    float xOff = i * Piece.CubeSize; 
                    float zOff = j * Piece.CubeSize;

                    Vector3 pos = topLeft + new Vector3(-xOff, 0, zOff);
                    
                    Gizmos.DrawWireCube(pos, new Vector3(Piece.CubeSize, Piece.CubeSize, Piece.CubeSize));
                }
            }
            */

            Gizmos.color = Color.magenta;
            //Vector3 position = new Vector3(0.12f, 0f, 0.056f);
            Gizmos.DrawSphere(transform.position - position, 0.01f);
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(SnapPosition(transform.position - position), 0.01f);
        }
#endif
    }
}