using System;
using SSSTools.FunText.AnimationTypes;
using SSSTools.FunText.Attributes;
using SSSTools.FunText.Core;
using UnityEngine;
using AnimationInfo = SSSTools.FunText.Core.AnimationInfo;

namespace SSSTools.FunText.Effects
{
    [Serializable]
    internal abstract class GradientAnimation : IBaseEffect
    {
        [SerializeField] protected Gradient gradient;
        [SerializeField] protected CurveReader curve;
        [SerializeReference, ChooseReference] protected IIndex Index;
        
        public abstract void PlayAnimation(ref CharacterData data, IndexInfo indexInfo, AnimationInfo info);
    }
}