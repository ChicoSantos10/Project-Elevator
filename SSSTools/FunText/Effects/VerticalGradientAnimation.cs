using System;
using SSSTools.FunText.Core;

namespace SSSTools.FunText.Effects
{
    [Serializable]
    internal class VerticalGradientAnimation : GradientAnimation
    {
        
        public override void PlayAnimation(ref CharacterData data, IndexInfo indexInfo, AnimationInfo info)
        {
            float index = Index.GetIndex(indexInfo);
                
            data.Colors[0] = gradient.Evaluate(curve.Evaluate(info.T, index, 0));
            data.Colors[1] = gradient.Evaluate(1 - curve.Evaluate(info.T, index, 1));
            data.Colors[2] = gradient.Evaluate(1 - curve.Evaluate(info.T, index, 2));
            data.Colors[3] = gradient.Evaluate(curve.Evaluate(info.T, index, 3));
        }
    }
}