using System;
using SSSTools.FunText.AnimationTypes;
using SSSTools.FunText.Core;
using UnityEngine;
using AnimationInfo = SSSTools.FunText.Core.AnimationInfo;

namespace SSSTools.FunText.Effects
{
    [Serializable]
    internal class TwoColorsBlend : IBaseEffect
    {
        [SerializeField] Gradient leftGradient;
        [SerializeField] Gradient rightGradient;
        [SerializeField] CurveReader curve;

        public void PlayAnimation(ref CharacterData data, IndexInfo indexInfo, AnimationInfo info)
        {
            float middle = (info.CountNoInvisibleCharacters - 1) / 2.0f;

            for (int i = 0; i < data.Colors.Length; i++)
            {
                Gradient gradient;
                float index;
                if (indexInfo.IndexNoInvisibleChars < middle)
                {
                    gradient = leftGradient;
                    index = indexInfo.IndexNoInvisibleChars;
                }
                else if (indexInfo.IndexNoInvisibleChars > middle)
                {
                    gradient = rightGradient;
                    index = indexInfo.IndexNoInvisibleChars - middle;
                }
                else
                {
                    if (i < 2)
                    {
                        gradient = leftGradient;
                        index = indexInfo.IndexNoInvisibleChars;
                    }
                    else
                    {
                        gradient = rightGradient;
                        index = indexInfo.IndexNoInvisibleChars - middle;
                    }
                }

                data.Colors[i] = gradient.Evaluate(curve.Evaluate(info.T, index / middle, i));
            }
        }
    }
}