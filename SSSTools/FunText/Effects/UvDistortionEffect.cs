using System;
using SSSTools.FunText.AnimationTypes;
using SSSTools.FunText.Core;
using UnityEngine;
using AnimationInfo = SSSTools.FunText.Core.AnimationInfo;

namespace SSSTools.FunText.Effects
{
    [Serializable]
    internal class UvDistortionEffect : IBaseEffect
    {
        [SerializeField] CurveReader curve;
        [SerializeReference] IIndex _index;
         
        public void PlayAnimation(ref CharacterData data, IndexInfo indexInfo, AnimationInfo info)
        {
            for (int i = 0; i < data.Uvs.Length; i++)
            {
                int index = (i + 1) % data.Uvs.Length;
                
                data.Uvs[i] = Vector2.Lerp(data.Uvs[i], data.Uvs[index], curve.Evaluate(info.T, _index.GetIndex(indexInfo), i));
            }
        }
    }
}