using System;
using SSSTools.Extensions;
using SSSTools.FunText.AnimationTypes;
using SSSTools.FunText.Core;
using UnityEngine;
using AnimationInfo = SSSTools.FunText.Core.AnimationInfo;

namespace SSSTools.FunText.Effects
{
    [Serializable]
    internal class ExpandAnimation : IBaseEffect
    {
        [SerializeField] CurveReader curve;
        [SerializeReference] IIndex _index;
        [SerializeField] float magnitude = 1;
        [SerializeField] Pivot pivot;
        
        public void PlayAnimation(ref CharacterData data, IndexInfo indexInfo, AnimationInfo info)
        {
            Vector3 pivot = data.GetCustomPivot(this.pivot);
            float index = _index.GetIndex(indexInfo);
            
            for (int i = 0; i < data.Vertices.Length; i++)
            {
                Vector3 direction = data.Vertices[i] - pivot;
                data.Vertices[i] += curve.Evaluate(info.T, index, i) * magnitude * direction;
            }
        }
    }
    
    [Serializable]
    internal class ExpandCharEffect : ICharacterEffect
    {
        //[SerializeField] CurveReader curve;
        [SerializeReference] ICurve _curve = new EaseOutBounce();
        [SerializeField] Pivot pivot;
        
        public void PlayAnimation(ref CharacterData data, float t)
        {
            Vector3 pivot = data.GetCustomPivot(this.pivot);
            
            for (int i = 0; i < data.Vertices.Length; i++)
            {
                //float t = data.TimeVisible / duration;
                data.Vertices[i] = Vector3.LerpUnclamped(pivot, data.Vertices[i], _curve.Evaluate(t));
            }
        }
    }
}