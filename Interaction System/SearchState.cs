using System;
using State_Machine;
using UnityEngine;

namespace Interaction_System
{
    [Serializable]
    internal class SearchState : IState
    {
        static readonly int HighlightIntensity = Shader.PropertyToID("_HighlightIntensity");
        
        [SerializeField, Range(0.5f, 5)] float maxPickupDistance = 1f;

#nullable enable

        public Interactable? Selected { get; private set; }

        public Material? CurrentMaterial { get; private set; }

#nullable disable
        
        public void OnEnter()
        {
                     
        }

        public void OnExit()
        {
            Highlight(0);
        }

        public void Update()
        {
            Collider interactableObj = SelectionManager.GetObject(maxPickupDistance);
            if (interactableObj != null &&
                interactableObj.gameObject.TryGetComponent(out Interactable interactable))
            {
                if (Selected == interactable) 
                    return;
                
                Selected = interactable;
                // TODO: Optimization => Interactable could have the material as a property
                CurrentMaterial = interactableObj.gameObject.GetComponentInChildren<MeshRenderer>().material; 
                Highlight(1);
            }
            else if (Selected != null)
            {
                Highlight(0);
                Selected = null;
                CurrentMaterial = null;
            }
        }
        
        void Highlight(float amount)
        {
            Debug.Assert(CurrentMaterial != null, "Material should not be null");

            CurrentMaterial!.SetFloat(HighlightIntensity, amount);
        }
    }
}