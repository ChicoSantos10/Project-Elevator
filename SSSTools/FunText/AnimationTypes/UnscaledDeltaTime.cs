using UnityEngine;

namespace SSSTools.FunText.AnimationTypes
{
    internal struct UnscaledDeltaTime : IDeltaTime
    {
        public float GetDeltaTime()
        {
            return Time.unscaledDeltaTime;
        }
    }
}