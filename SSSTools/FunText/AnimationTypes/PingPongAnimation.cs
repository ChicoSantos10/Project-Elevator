using UnityEngine;

namespace SSSTools.FunText.AnimationTypes
{
    internal struct PingPongAnimation : IAnimationType
    {
        public float ComputeT(float t)
        {
            return Mathf.PingPong(t, 1);
        }
    }
}