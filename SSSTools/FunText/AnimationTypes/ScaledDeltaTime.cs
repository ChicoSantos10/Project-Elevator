using System;
using UnityEngine;

namespace SSSTools.FunText.AnimationTypes
{
    [Serializable]
    internal struct ScaledDeltaTime : IDeltaTime
    {
        public float GetDeltaTime()
        {
            return Time.deltaTime;
        }
    }
}