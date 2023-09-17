using System;
using System.Collections.Generic;
using System.Linq;
using Interaction_System;
using UnityEngine;

namespace Puzzles
{
    [Serializable]
    internal class CubePuzzleBehaviour : InteractableBehaviour
    {
        [SerializeField] LayerMask puzzlePieceLayer;
        [SerializeField] Board board;
        [SerializeField] List<Piece> pieces;
        [SerializeField] AudioClip puzzleCompleted;
        AudioSource audioSource;
        
        Collider _pieceCollider;
        Piece _piece;
        Camera _camera;
        int lockTest = 0;
        
        public override bool Finished { get; protected set; }

        protected override void OnInitialize()
        {
            _camera = Camera.main;
            audioSource = Transform.GetComponent<AudioSource>();
        }

        public override void OnLeftMouse(Vector2 pos)
        {
            SetActivePiece(pos);
        }

        public override void OnRightMouse(Vector2 pos)
        {
            SetActivePiece(pos);
            
            if (_pieceCollider == null)
                return;
            
            if (board.ContainsPoint(_piece.transform.position) && !board.ContainsBounds(_piece.RotatedBounds))
                return;
            
            _piece.Rotate();
            MovePiece(_piece.transform.position);
        }

        void SetActivePiece(Vector2 pos)
        {
            _pieceCollider = SelectionManager.GetObjectScreenPoint(pos, layer: puzzlePieceLayer);
            _piece = _pieceCollider != null ? _pieceCollider.GetComponent<Piece>() : null;
        }

        public override void OnLeftMouseDrag(Vector2 pos, Vector2 delta)
        {
            if (_pieceCollider == null) return;
            
            Transform transform = _pieceCollider.transform;

            Vector3 piecePosition = transform.position;
            Vector3 worldPoint = _camera.ScreenToWorldPoint((Vector3)pos + Vector3.forward * piecePosition.y);
            
            worldPoint.y = piecePosition.y;
            MovePiece(worldPoint);
        }

        public override void OnLeftMouseUp(Vector2 pos)
        {
            if (_piece == null)
                return;
            
            if (_piece.Collided) 
                _piece.Move(_piece.InitialPosition);

            _piece.Placed = board.ContainsBounds(_piece.Bounds);
            Debug.Log(_piece.Placed);
            Finished = CheckFinish();
;
            if (Finished && lockTest == 0)
            {
                Debug.Log("entra aqui1");
                if(GameObject.Find("CameraObjectController"))
                { 
                    
                    GameObject.Find("CameraObjectController").GetComponent<Camera_Controller.CameraModesController>().puzzle1Completed = true;
                    audioSource.PlayOneShot(puzzleCompleted);
                    lockTest = 1;
                }
            }
            else
            {
                Debug.Log($"Finished? {Finished}");
            }
        }

        bool CheckFinish()
        {
            return pieces.All(piece => piece.Placed);
        }

        void MovePiece(Vector3 pos) => _piece.Move(board.SnapPosition(pos) + _piece.Offset);
    }
}