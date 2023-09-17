using System;
using UnityEngine;
using UnityEngine.Events;

namespace Custom_Cursor
{
    [CreateAssetMenu(fileName = "Cursor Manager", menuName = "Cursor/Manager", order = 0)]
    public class CursorManager : ScriptableObject
    {
        [SerializeField] CursorAnimation defaultAnimation;
        
        CursorAnimation _animation;

        public Vector2 Offset => _animation.Offset;
        public float Speed => _animation.Speed;
        
        public CursorAnimation CursorAnimation
        {
            set => SetAnimation(value);
        }

        void OnEnable()
        {
            SetDefaultCursor();
        }

        public void SetAnimation(CursorAnimation animation)
        {
            if (_animation == animation)
                return;
            
            animation.Reset();
            _animation = animation;
            
            OnAnimationChange.Invoke();
        }

        public void SetDefaultCursor() => SetAnimation(defaultAnimation);

        public Texture2D NextFrame()
        {
            if (_animation.MoveNext())
                return _animation.Current;
            
            _animation.Reset();
            _animation.MoveNext();
            return _animation.Current;
        }

        public static void EnableCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        public static void DisableCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
        public event UnityAction OnAnimationChange = delegate { };
    }
}