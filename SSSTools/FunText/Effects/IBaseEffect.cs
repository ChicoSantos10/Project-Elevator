using System;
using SSSTools.FunText.AnimationTypes;
using SSSTools.FunText.Attributes;
using SSSTools.FunText.Core;
using UnityEngine;
using AnimationInfo = SSSTools.FunText.Core.AnimationInfo;
using Random = UnityEngine.Random;

namespace SSSTools.FunText.Effects
{
    public interface IBaseEffect
    {
        void PlayAnimation(ref CharacterData data, IndexInfo indexInfo, AnimationInfo info);
    }

    [Serializable]
    internal class ShakeEffect : IBaseEffect
    {
        Vector3[] directions;
        int index;

        [SerializeField] float magnitude;
        [SerializeField] float delay = 0.1f;
        [SerializeReference, ChooseReference] IIndex indexType;

        public void Initialize()
        {
            directions = new Vector3[4];

            for (int i = 0; i < 4; i++)
            {
                directions[i] = Random.insideUnitCircle;
            }
        }

        public void PlayAnimation(ref CharacterData data, IndexInfo indexInfo, AnimationInfo info)
        {
            float speed = 1 / delay;
            index = (int) (info.TimePassed * speed) % directions.Length;

            float newT = Mathf.PingPong(info.TimePassed, delay / 2) * speed * 2;

            for (int i = 0; i < data.Vertices.Length; i++)
            {
                data.Vertices[i] += Vector3.Lerp(Vector3.zero, directions[index] * magnitude,
                    newT);
            }
        }
    }

    public interface ICharacterEffect
    {
        void PlayAnimation(ref CharacterData data, float duration);
    }
}