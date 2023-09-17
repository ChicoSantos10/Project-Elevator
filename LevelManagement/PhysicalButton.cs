using System;
using Interaction_System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace LevelManagement
{
    [RequireComponent(typeof(Collider))]
    public class PhysicalButton : MonoBehaviour
    {
        public event UnityAction OnClick;
        public UnityEvent onClickActions;

        [SerializeField] LayerMask layer;

        Collider _collider;

        void Awake()
        {
            _collider = GetComponent<Collider>();
        }

        public void Update()
        {
            if (!Mouse.current.leftButton.wasPressedThisFrame) 
                return;

            Collider c = SelectionManager.GetObjectScreenPoint(Mouse.current.position.ReadValue());
            
            if (c == null || c != _collider || c.gameObject.layer.Equals(layer))
                return;
            
            print("Clicked");
            OnClick?.Invoke();
            onClickActions.Invoke();
        }
    }
}