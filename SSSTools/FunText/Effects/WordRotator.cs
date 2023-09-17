using SSSTools.FunText.Core;
using UnityEngine;
using AnimationInfo = SSSTools.FunText.Core.AnimationInfo;

namespace SSSTools.FunText.Effects
{
    internal class WordRotator : IRotator
    {
        public Vector3 GetPivot(CharacterData data, Vector3 pivot, AnimationInfo info, IndexInfo indexInfo)
        {
            // Needs Bounds of each word
            return CharacterData.GetCustomPivot(info.WordBounds[indexInfo.WordIndex], pivot);
        }
    }
}