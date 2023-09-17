using System;
using UnityEngine;

namespace SSSTools.Extensions
{
    public interface ICurve
    {
        float Evaluate(float t);
    }

    [Serializable]
    internal struct EaseInOutElastic : ICurve
    {
        [SerializeField] ElasticProperties properties;
        
        float ICurve.Evaluate(float t)
        {
            return Evaluate(t, properties);
        }

        public static float Evaluate(float t, ElasticProperties properties)
        {
            if (t == 0 || Math.Abs(t - 1) < Mathf.Epsilon)
                return t;
            
            if (properties.rigidity < 15)
                properties.rigidity = 15;

            return t < 0.5f
                ? -Mathf.Pow(2, 2 * properties.rigidity * t - properties.rigidity) * Mathf.Sin((properties.elasticity * 6 * t - ElasticProperties.B) * (2 * Mathf.PI / 3f)) / 2
                : Mathf.Pow(2, -2 * properties.rigidity * t + properties.rigidity) * Mathf.Sin((properties.elasticity * 6 * t - ElasticProperties.B) * (2 * Mathf.PI / 3f)) / 2 + 1;
        }
    }

    [Serializable]
    internal struct EaseOutElastic : ICurve
    {
        [SerializeField] ElasticProperties properties;
        
        float ICurve.Evaluate(float t)
        {   
            return Evaluate(t, properties);
        }

        public static float Evaluate(float t, ElasticProperties properties)
        {
            if (t == 0 || Math.Abs(t - 1) < Mathf.Epsilon)
                return t;

            if (properties.rigidity < 15)
                properties.rigidity = 15;
            
            return Mathf.Pow(2, -properties.rigidity * t) * Mathf.Sin((t * properties.elasticity * 3 - ElasticProperties.B) * (2 * Mathf.PI / 3)) + 1;
        }
    }

    [Serializable]
    internal struct ElasticProperties
    {
        public int elasticity;
        public float rigidity;
        public const float B = 0.75f;
    }

    [Serializable]
    internal struct EaseInElastic : ICurve
    {
        [SerializeField] ElasticProperties properties;
        
        float ICurve.Evaluate(float t)
        {
            return Evaluate(t, properties);
        }

        public static float Evaluate(float t, ElasticProperties properties)
        {
            if (t == 0 || Math.Abs(t - 1) < Mathf.Epsilon)
                return t;

            if (properties.rigidity < 15)
                properties.rigidity = 15;
            
            return -Mathf.Pow(2, properties.rigidity * t - properties.rigidity) * Mathf.Sin((t * properties.elasticity * 3 - ElasticProperties.B) * (2 * Mathf.PI / 3));
        }
    }

    [Serializable]
    internal class UnityAnimationCurve : ICurve
    {
        [SerializeField] AnimationCurve curve;

        public AnimationCurve Curve
        {
            get => curve;
            set => curve = value;
        }

        public UnityAnimationCurve()
        {
            curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
            curve.postWrapMode = WrapMode.PingPong;
        }
        
        public UnityAnimationCurve(AnimationCurve curve)
        {
            Curve = curve;
        }

        public float Evaluate(float t)
        {
            return Curve.Evaluate(t);
        }
    }

    internal struct EaseInOutBounce : ICurve
    {
        float ICurve.Evaluate(float t) => Evaluate(t);
        
        public static float Evaluate(float t)
        {
            return t < 0.5f ? (1 - EaseOutBounce.Evaluate(1 - 2 * t)) / 2 : (1 + EaseOutBounce.Evaluate(2 * t - 1)) / 2;
        }
    }

    internal struct EaseInBounce : ICurve
    {
        float ICurve.Evaluate(float t) => Evaluate(t);

        public static float Evaluate(float t) => 1 - EaseOutBounce.Evaluate(1 - t);
    }

    internal struct EaseOutBounce : ICurve
    {
        float ICurve.Evaluate(float t) => Evaluate(t);

        public static float Evaluate(float t)
        {
            const float n1 = 7.5625f;
            const float d1 = 2.75f;

            if (t < 1 / d1)
                return n1 * t * t;
            if (t < 2 / d1)
                return n1 * Mathf.Pow(t - 1.5f / d1, 2) + 0.75f;
            if (t < 2.5f / d1)
                return n1 * Mathf.Pow((t - 2.25f / d1), 2) + 0.9375f;
            return n1 * Mathf.Pow((t - 2.625f / d1), 2) + 0.984375f;
        } 
        
    }

    internal struct QuadraticEaseOut : ICurve
    {
        float ICurve.Evaluate(float t)
        {
            return Evaluate(t);
        }
        
        public static float Evaluate(float t)
        {
            float inverse = 1 - t;
            return 1 - inverse * inverse;
        }
    }
    
    internal struct QuadraticEaseIn : ICurve
    {
        float ICurve.Evaluate(float t)
        {
            return Evaluate(t);
        }

        public static float Evaluate(float t) => t * t;
    }
    
    internal struct QuadraticEaseInOut : ICurve
    {
        float ICurve.Evaluate(float t)
        {
            return Evaluate(t);
        }

        public static float Evaluate(float t)
        {
            return t < 0.5f ? 2 * QuadraticEaseIn.Evaluate(t) : 2 * (t - 0.5f) * (1 - t + 0.5f) + 0.5f;
        }
    }
}