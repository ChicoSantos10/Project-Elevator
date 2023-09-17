using System;
using SSSTools.Extensions;
using SSSTools.FunText.Attributes;
using UnityEngine;

namespace SSSTools.FunText.AnimationTypes
{
    [Serializable]
    internal class CurveReader : SimpleCurveReader
    {
        [SerializeField] protected WrapMode wrapMode; 
        [SerializeField] float characterShiftRatio;
        [SerializeField] float speed = 1;

        public CurveReader()
        {
            if (Curve is UnityAnimationCurve curve)
                curve.Curve.postWrapMode = wrapMode;
        }

        public override float Evaluate(float t, float index, int vertexId)
        {
            t = GetT(t, index, vertexId);
            
            switch (wrapMode)
            {
                case WrapMode.Once:
                    if (t > 1 || t < 0)
                        t = 0;
                    break;
                case WrapMode.Loop:
                    t = Mathf.Repeat(t, 1);
                    break;
                case WrapMode.PingPong:
                    t = Mathf.PingPong(t, 1);
                    break;
                case WrapMode.Default:
                    break;
                case WrapMode.ClampForever:
                    t = Mathf.Clamp01(t);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            return Curve.Evaluate(t);
        }

        protected override float GetT(float t, float index, int vertexId) => base.GetT(t, speed, vertexId) + index * characterShiftRatio;
    }

    [Serializable]
    internal class SimpleCurveReader
    {
        [SerializeReference, ChooseReference] protected ICurve Curve = new UnityAnimationCurve(AnimationCurve.EaseInOut(0, 0, 1, 1));
        [SerializeReference, ChooseReference] protected IVertexDisplacer vertexDisplacer = new ById();
        [SerializeField] protected float vertexDisplacementRatio;
        
        public SimpleCurveReader()
        {
            if (Curve is UnityAnimationCurve curve)
                curve.Curve.postWrapMode = WrapMode.Clamp;
        }
        
        public virtual float Evaluate(float t, float speed, int vertexId) => Curve.Evaluate(GetT(t, speed, vertexId));

        public virtual float EvaluateClamped(float t, float speed, int vertexId) => Curve.Evaluate(Mathf.Clamp01(GetT(t, speed, vertexId)));

        public float GetVertexDisplacement(int vertexId) => vertexDisplacer.GetDisplacement(vertexId) * vertexDisplacementRatio;

        protected virtual float GetT(float t, float speed, int vertexId) => t * speed + GetVertexDisplacement(vertexId);
    }
}