using System;
using UnityEngine;

namespace SSSTools.FunText.AnimationTypes
{
    [Serializable]
    internal struct ByLoops : IDuration
    {
        [NonSerialized] int _loopsDone;

        [SerializeField, Min(1)] int totalLoops;

        public bool IsComplete()
        {
            return _loopsDone >= totalLoops;
        }

        public void Update(float timePassed, float speed)
        {
            if ((int) timePassed % (totalLoops + 1) > _loopsDone)
                _loopsDone++;
        }
    }
}