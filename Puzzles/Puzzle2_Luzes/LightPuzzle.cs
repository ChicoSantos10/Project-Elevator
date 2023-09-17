using System;
using System.Collections.Generic;
using System.Linq;
using Interaction_System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Puzzles
{
    [Serializable]
    class LightPuzzle : InteractableBehaviour
    {
        public override bool Finished { get => _windowInteractable.Finished || _finished; protected set => _finished = value; }

        [SerializeField] private LightController[] lights;
        [SerializeField] private Collider[] buttons;
        [SerializeField] private LayerMask layer;
        [SerializeField] private AudioClip buttonSound;
        [SerializeField] Interactable window;

        WindowInteractable _windowInteractable;
        
        private AudioSource audioSource;
        bool _finished;

        private Dictionary<Collider, LightController> _lightCombination;

        void ShuffleLights()
        {
            _lightCombination = new Dictionary<Collider, LightController>();
            
            List<LightController> temp = lights.OrderBy(l => Random.value).ToList();

            int i = 0;
            
            foreach (Collider collider in buttons)
            {
                _lightCombination.Add(collider, temp[i++]);
            }
        }

        public override void OnLeftMouse(Vector2 pos)
        {
            Collider? c = SelectionManager.GetObjectScreenPoint(pos, layer: layer);
            
            if (c == null)
            {
                return;
            }

            _lightCombination[c].Swap();
            audioSource.PlayOneShot(buttonSound);
        }

        protected override void OnInitialize()
        {
            ShuffleLights();
            audioSource = Transform.GetComponent<AudioSource>();

            window.TryGetBehaviour(out _windowInteractable);
        }

        public override void StartInteraction()
        {
            _windowInteractable.TurnOff();
        }

        public bool CheckSolution(List<Collider> clicks)
        {
            int i = 0;
            if (clicks.All(c => _lightCombination[c] == lights[i++]))
            {
                _finished = true;
                return true;
            }

            _windowInteractable.TurnOff();
            return false;
        }
    }
}