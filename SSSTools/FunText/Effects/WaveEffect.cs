using System;
using SSSTools.FunText.AnimationTypes;
using SSSTools.FunText.Core;
using UnityEngine;
using AnimationInfo = SSSTools.FunText.Core.AnimationInfo;

namespace SSSTools.FunText.Effects
{
    [Serializable]
    public class WaveEffect : IBaseEffect
    {
        // TODO: Index type and refactor
        [SerializeReference] IIndex _index;
        [SerializeField, Min(0)] float frequency = 10f;
        [SerializeField, Min(0)] float amplitude = 30f;
        [SerializeField, Range(0, Mathf.PI)] float phaseShift = 0;
        [SerializeField, Min(0)] float verticalShift = 0;
        [SerializeField] bool fullCharacter;
        
        public void PlayAnimation(ref CharacterData data, IndexInfo indexInfo, AnimationInfo info)
        {
            for (int i = 0; i < data.Vertices.Length; i++)
            {
                float index = _index.GetIndex(indexInfo);
                data.Vertices[i] += Wave(index, info.T);
            }
        }
        
        Vector3 Wave(float offset, float t)
        {
            float time = t * 2 * Mathf.PI / frequency;
            return new Vector2(0, amplitude * Mathf.Sin(frequency * (time - offset * phaseShift)) + verticalShift);
        }
    }
}