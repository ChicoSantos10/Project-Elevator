using System;
using SSSTools.FunText.AnimationTypes;
using SSSTools.FunText.Core;
using UnityEngine;
using AnimationInfo = SSSTools.FunText.Core.AnimationInfo;

namespace SSSTools.FunText.Effects
{
    [Serializable]
    internal class RotationEffect : IBaseEffect
    {
        [SerializeField] CurveReader curve;
        [SerializeReference] IIndex _index;
        [SerializeField, Range(0, 360)] float maxRotation;
        [SerializeReference] IRotator _rotator;
        [SerializeField] Pivot pivot;

        public void PlayAnimation(ref CharacterData data, IndexInfo indexInfo, AnimationInfo info)
        {
            Vector3 piv = _rotator.GetPivot(data, pivot, info, indexInfo);
            float index = _index.GetIndex(indexInfo);
            
            for (int i = 0; i < data.Vertices.Length; i++)
            {
                float angle = curve.Evaluate(info.T, index, i) * maxRotation;
                data.Vertices[i] = RotateAroundPivot(data.Vertices[i], piv, angle);
            }
        }

        Vector3 RotateAroundPivot(Vector3 pos, Vector3 pivot, float angle)
        {
            Vector3 dir = pos - pivot;
            dir = Quaternion.Euler(0, 0, angle) * dir;
            return dir + pivot;
        }
    }
}