using System;
using UnityEngine;

namespace SSSTools.FunText.Effects
{
    [Serializable]
    internal struct Pivot
    {
        [Range(-0.5f, 0.5f)] public float X;
        [Range(-0.5f, 0.5f)] public float Y;

        public static implicit operator Vector3(Pivot p) => new Vector3(p.X, p.Y);
        public static implicit operator Vector2(Pivot p) => new Vector2(p.X, p.Y);
    }
}