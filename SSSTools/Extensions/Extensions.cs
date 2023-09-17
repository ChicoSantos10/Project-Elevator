using System.Collections.Generic;
using SSSTools.FunText.Core;
using UnityEngine;

namespace SSSTools.Extensions
{
    public static class Extensions
    {
        public static float Remap(this float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }

        public static IEnumerable<T> And<T>(this IEnumerable<T> source, IEnumerable<T> value)
        {
            foreach (T v in source)
            {
                yield return v;
            }

            foreach (T v in value)
            {
                yield return v;
            }
        }

        public static IEnumerable<(T, int)> WithIndex<T>(this IEnumerable<T> source)
        {
            int index = 0;
            foreach (T item in source)
            {
                yield return (item, index++);
            }
        }

        public static void SetPixel(this Texture2D texture, Vector2Int position, Color color)
        {
            texture.SetPixel(position.x, position.y, color);
        }
        
        public static Vector2Int ToVectorInt(this Vector2 v) => new Vector2Int((int)v.x, (int)v.y);

        public static Vector2 ToViewportPosition(this Vector2 v)
        {
            Vector2 res = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
            return v / res;
        }

        public static Bounds Rotate(this Bounds bounds)
        {
            bounds.extents = new Vector3(bounds.extents.z, bounds.extents.y, bounds.extents.x);
            // bounds.SetMinMax(
            //     new Vector3(bounds.min.z, bounds.min.y, bounds.min.x),
            //     new Vector3(bounds.max.z, bounds.max.y, bounds.max.x));

            return bounds;
        }

        // TODO: Move to fun text folders
        public static IEnumerable<ShowCharacterEffect> And(this IEnumerable<ShowCharacterEffectDataObject> source, IEnumerable<ShowCharacterEffect> value)
        {   
            foreach (ShowCharacterEffectDataObject v in source)
            {
                yield return v;
            }

            foreach (ShowCharacterEffect v in value)
            {
                yield return v;
            }
        }

        public static IEnumerable<(string, ShowCharacterEffect)> And(this IEnumerable<ShowCharacterEffectDataObject> source, IEnumerable<ShowCharEffectTag> value)
        {
            foreach (ShowCharacterEffectDataObject v in source)
            {
                yield return (v.Tag, v.Effect);
            }

            foreach (ShowCharEffectTag v in value)
            {
                yield return (v.Tag, v.Effect);
            }
        }
    }
}