using System;
using SSSTools.Extensions;
using SSSTools.FunText.AnimationTypes;
using SSSTools.FunText.Core;
using UnityEngine;
using AnimationInfo = SSSTools.FunText.Core.AnimationInfo;

namespace SSSTools.FunText.Effects
{
    [Serializable]
    internal class FadeAnimation : IBaseEffect
    {
        [SerializeField] CurveReader curve;
        [SerializeReference] IIndex _index;
        
        public void PlayAnimation(ref CharacterData data, IndexInfo indexInfo, AnimationInfo info)
        {
            float index = _index.GetIndex(indexInfo);
            
            for (int i = 0; i < data.Colors.Length; i++)
            {
                Color color = data.Colors[i];
                data.Colors[i] = Color32.Lerp(color, Color.clear, curve.Evaluate(info.T, index, i));
            }
        }
    }
}