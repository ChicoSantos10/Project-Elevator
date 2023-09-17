using System;
using UnityEngine;

namespace SSSTools.FunText.AnimationTypes
{
    [Serializable]
    internal struct BySeconds : IDuration
    {
        float _timePassed;
        [SerializeField] float duration;
        
        public bool IsComplete()
        {
            return _timePassed >= duration;
        }

        public void Update(float timePassed, float speed)
        {
            _timePassed = timePassed;
        }
    }
}