using System;
using SSSTools.FunText.AnimationTypes;
using SSSTools.FunText.Attributes;
using SSSTools.FunText.Effects;
using UnityEngine;

namespace SSSTools.FunText.Core
{
    [Serializable]
    public class TextAnimation
    {
        [SerializeReference, ChooseReference] IAnimationType animationType;
        [SerializeReference, ChooseReference] IDeltaTime deltaTime;
        [SerializeReference, ChooseReference] IDuration duration;
        [SerializeField, Min(0)] float speed;

        public string tag;
        [SerializeReference, ChooseReference] IBaseEffect effect; // TODO: Make it a list

        public IAnimationType AnimationType => animationType;
        
        public IDeltaTime DeltaTime => deltaTime;
        
        public IDuration Duration => duration;
        
        public float Speed => speed;

        /*public void Play(CharacterData[] data, int start, int end)
        {
            if (duration.IsComplete())
                return;
            
            UpdateT();
            
            for (int i = start; i < end && i < data.Length; i++)
            {
                if (data[i].Info.isVisible)
                    effect.PlayAnimation(ref data[i], i, _t);
            }
        }*/

        public void Play(CharacterData[] data, AnimationInfo animation)
        {
            if (duration.IsComplete())
                return;

            for (int i = animation.Start; i <= animation.End && i < data.Length; i++)
            {
                if (!data[i].IsVisible)
                    continue;

                int index = i - animation.Start;
                IndexInfo info = new IndexInfo(
                    index,
                    animation.GetIndexWithoutInvisibleChars(index, out int wordIndex),
                    i,
                    wordIndex,
                    animation.GetNormalized(index),
                    animation.GetNormalizedNoInvisible(index)); 

                effect.PlayAnimation(ref data[i], info, animation);
            }
        }

        // public void Initialize()
        // {
        //     effect.Initialize();
        // }
    }

    public readonly struct IndexInfo
    {
        public readonly int Index;
        public readonly int IndexNoInvisibleChars;
        public readonly int RealIndex;
        public readonly int WordIndex;
        public readonly float NormalizedIndex;
        public readonly float NormalizedNoInvisibleChars;

        public IndexInfo(int index, int indexNoInvisibleChars, int realIndex, int wordIndex, float normalizedIndex,
            float normalizedNoInvisibleChars)
        {
            Index = index;
            IndexNoInvisibleChars = indexNoInvisibleChars;
            RealIndex = realIndex;
            NormalizedIndex = normalizedIndex;
            NormalizedNoInvisibleChars = normalizedNoInvisibleChars;
            WordIndex = wordIndex;
        }

        public override string ToString()
        {
            return
                $"Real Index: {RealIndex}\n" +
                $"Index: {Index}\n" +
                $"IndexNoInv: {IndexNoInvisibleChars}\n" +
                $"NormIndex: {NormalizedIndex}\n" +
                $"NormNoInv: {NormalizedNoInvisibleChars}";
        }
    }
}