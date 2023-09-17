using System;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;

namespace Interaction_System
{
    [Serializable]
    class DoorBehaviour : InteractableBehaviour
    {
        [SerializeField] private float duration = 1.0f;
        [SerializeField] private float angle = 90.0f;
        [SerializeField] private AudioClip openSound, closeSound;
        private bool isOpen = false;
        private Vector3 openRotation, closeRotation;
        [CanBeNull] private AudioSource audioSource;
        
        public override bool Finished { get; protected set; }

        protected override void OnInitialize()
        {
            closeRotation = Transform.localRotation.eulerAngles;
            openRotation = closeRotation + new Vector3(0, angle, 0);
            audioSource = Transform.GetComponent<AudioSource>();
        }

        public override void StartInteraction()
        {
            if (isOpen)
            {
               Close();
            }
            else
            {
                Open();
            }

            Finished = true;
        }
        
        void Open()
        {
            //Transform.DOLocalRotate(openRotation, duration).OnComplete(() => Finished = false);
            Rotate(openRotation);
            isOpen = true;
            if (audioSource != null) 
                audioSource.PlayOneShot(openSound);
        }
        
        void Close()
        {
            //Transform.DOLocalRotate(closeRotation, duration);
            Rotate(closeRotation);
            isOpen = false;
            if (audioSource != null) 
                audioSource.PlayOneShot(closeSound);
        }

        void Rotate(Vector3 newAngle)
        {
            Transform.DOLocalRotate(newAngle, duration).OnComplete(() => Finished = false);
        }
    }
}