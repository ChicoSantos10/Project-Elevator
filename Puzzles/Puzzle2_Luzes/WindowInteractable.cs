using System;
using System.Threading.Tasks;
using Interaction_System;
using UnityEngine;

namespace Puzzles
{
    [Serializable]
    internal class WindowInteractable : InteractableBehaviour
    {
        [SerializeField, Range(1,5)] float speed;
        
        public override bool Finished
        {
            get => _isVisible;
            protected set => _isVisible = value;
        }

        static readonly int Interpolator = Shader.PropertyToID("_Interpolator");

        Material mat;
        bool _isVisible;

        protected override void OnInitialize()
        {
            mat = Transform.GetComponent<MeshRenderer>().material;
            TurnOff();
        }

        public override void StartInteraction()
        {
            TurnOn();
        }

        public void TurnOff()
        {
            Interpolate(0, 1);
            _isVisible = false;
        }

        public void TurnOn()
        {
            Interpolate(1, 0);
            _isVisible = true;
        }

        async void Interpolate(float start, float end)
        {
            float t = 0;

            while (t < 1)
            {
                float value = Mathf.Lerp(start, end, t);
                mat.SetFloat(Interpolator, value);

                t += speed * Time.deltaTime;

                await Task.Yield();
            }
            
            mat.SetFloat(Interpolator, end);
        }
    }
}