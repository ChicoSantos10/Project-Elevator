using Extras;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Puzzles
{
    public class AddNumberButton : SafeButton 
    {
        [SerializeField, Range(0, 9)] int number;
        [SerializeField] AudioClip buttonSound;
        private AudioSource audioSource;

        void Awake()
        {
           audioSource = GetComponent<AudioSource>();
        }

        public override void OnButtonPressed(List<int> code)
        {
            code.Add(number);
          
            print(number);
            HighLight();
            audioSource.PlayOneShot(buttonSound);

        }

        void HighLight()
        {
            if (TryGetComponent(out MeshRenderer meshRenderer))
                meshRenderer.material.SetInt("_Active", 1);
        }

        public void StopHighLight()
        {
            if (TryGetComponent(out MeshRenderer meshRenderer))
                meshRenderer.material.SetInt("_Active", 0);
        }

        public void TurnToGreen()
        {
            if (TryGetComponent(out MeshRenderer meshRenderer))
                meshRenderer.material.SetColor("_Color", Color.green);
        }
    }
}


