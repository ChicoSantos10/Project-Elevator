using System;
using Interaction_System;
using UnityEngine;
using JetBrains.Annotations;
using Object = UnityEngine.Object;

namespace Puzzles
{
    [Serializable]
    public class PickupObject : InteractableBehaviour
    {
        [SerializeField] GameObject enableObject;
        [SerializeField] private AudioClip catchPiece;
        [SerializeField] AudioSource audioSource;
        public override bool Finished { get; protected set; }

        protected override void OnInitialize()
        {
            enableObject.SetActive(false);
            
        }

        public override void StopInteraction()
        {      
            enableObject.SetActive(true);
            audioSource.PlayOneShot(catchPiece);
            Object.Destroy(Transform.gameObject);
        }
    }
}