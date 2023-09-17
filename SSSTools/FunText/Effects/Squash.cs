using System;
using SSSTools.FunText.AnimationTypes;
using SSSTools.FunText.Core;
using UnityEngine;
using AnimationInfo = SSSTools.FunText.Core.AnimationInfo;

namespace SSSTools.FunText.Effects
{
    [Serializable]
    internal abstract class Squash : IBaseEffect
    {
        [SerializeField] protected CurveReader curve;
        [SerializeReference] protected IIndex _index;
        [SerializeField] protected float magnitude = 1;
        [SerializeField, Range(-0.5f, 0.5f)] protected float offset = 0;
        
        public abstract void PlayAnimation(ref CharacterData data, IndexInfo indexInfo, AnimationInfo info);
    }
}