using System;
using SSSTools.FunText.AnimationTypes;
using SSSTools.FunText.Core;
using UnityEngine;
using AnimationInfo = SSSTools.FunText.Core.AnimationInfo;

namespace SSSTools.FunText.Effects
{
    [Serializable]
    internal class HorizontalGradientAnimation : GradientAnimation
    {
        // TODO: For each vertex instead of per character
        public override void PlayAnimation(ref CharacterData data, IndexInfo indexInfo, AnimationInfo info)
        {
            float index = Index.GetIndex(indexInfo); 
                
            for (int i = 0; i < data.Colors.Length; i++)
            {
                data.Colors[i] = gradient.Evaluate(curve.Evaluate(info.T, index, i));
            }
        }
    }
}