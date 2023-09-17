using SSSTools.FunText.Core;
using UnityEngine;
using AnimationInfo = SSSTools.FunText.Core.AnimationInfo;

namespace SSSTools.FunText.Effects
{
    internal class AllRotator : IRotator
    {
        public Vector3 GetPivot(CharacterData data, Vector3 pivot, AnimationInfo info, IndexInfo indexInfo)
        {
            // Needs bounds
            return CharacterData.GetCustomPivot(info.Bounds, pivot);
        }
    }
}