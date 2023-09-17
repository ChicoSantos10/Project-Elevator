using UnityEngine;

namespace SSSTools.FunText.AnimationTypes
{
    internal struct RepeatAnimation : IAnimationType
    {
        public float ComputeT(float t)
        {
            return Mathf.Repeat(t, 1);
        }
    }
}