using SSSTools.FunText.Core;
using UnityEngine;
using AnimationInfo = SSSTools.FunText.Core.AnimationInfo;

namespace SSSTools.FunText.Effects
{
    internal interface IRotator
    {
        Vector3 GetPivot(CharacterData data, Vector3 pivot, AnimationInfo info, IndexInfo indexInfo);
    }
}