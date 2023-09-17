using System;
using SSSTools.FunText.AnimationTypes;
using UnityEngine;

namespace SSSTools.FunText.Effects
{
    [Serializable]
    internal class FadeInOut : ICharacterEffect
    {
        [SerializeField] SimpleCurveReader curveReader;
        
        public void PlayAnimation(ref CharacterData data, float t)
        {   
            for (int i = 0; i < data.Colors.Length; i++)
            {
                Color color = data.Colors[i];
                float speed = 1 - curveReader.GetVertexDisplacement(i);
                data.Colors[i] = Color32.Lerp(Color.clear, color, curveReader.EvaluateClamped(t, speed, i));
            }
        }
    }
}