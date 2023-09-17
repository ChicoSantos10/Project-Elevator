using System.Collections.Generic;
using SSSTools.FunText.Core;
using TMPro;
using UnityEngine;

namespace SSSTools.FunText.Effects
{
    public struct CharacterData
    {
        public Vector3[] Vertices { get; set; }

        public Color32[] Colors { get; set; }

        public Vector2[] Uvs { get; set; }
        
        public Vector2[] Uvs2 { get; set; }

        public int Index { get; }

        public TMP_CharacterInfo Info { get; }

        public bool IsVisible => TimeVisible > 0;
        
        public float TimeVisible { get; internal set; }
        
        public float TimeHidden { get; internal set; }

        public List<ShowCharacterEffect> OnBecomeVisibleEffects;

        public readonly Vector3[] SourceVertices;
        public readonly Color32[] SourceColors;
        public readonly Vector2[] SourceUvs;
        public readonly Vector2[] SourceUvs2;

        public CharacterData(Vector3[] vertices, Color32[] colors, Vector2[] uvs, Vector2[] uvs2, TMP_CharacterInfo info, int index)
        {
            SourceVertices = vertices;
            SourceColors = colors;
            SourceUvs = uvs;
            SourceUvs2 = uvs2;
            Info = info;
            Index = index;
            
            Vertices = new Vector3[vertices.Length];
            Colors = new Color32[colors.Length];
            Uvs = new Vector2[uvs.Length];
            Uvs2 = new Vector2[uvs2.Length];
            
            OnBecomeVisibleEffects = new List<ShowCharacterEffect>();
            
            TimeVisible = 0;
            TimeHidden = 0;
        }

        public void Reset()
        {
            for (int i = 0; i < SourceColors.Length; i++)
            {
                Colors[i] = SourceColors[i];
                Vertices[i] = SourceVertices[i];
                Uvs[i] = SourceUvs[i];
                Uvs2[i] = SourceUvs2[i];
            }
        }

        public void PlayShowAnimations()
        {
            foreach (ShowCharacterEffect effect in OnBecomeVisibleEffects)
            {
                if (!effect.HasCharacterFinishedShowing(this))
                    effect.PlayShowEffect(ref this);
            }
        }

        public void PlayHideEffects()
        {
            foreach (ShowCharacterEffect effect in OnBecomeVisibleEffects)
            {
                if (!effect.HasCharacterFinishedHiding(this))
                    effect.PlayHideEffect(ref this);
            }
        }

        public Vector3 GetMiddlePoint()
        {
            return GetMiddlePoint(SourceVertices);
        }

        public static Vector3 GetMiddlePoint(Vector3[] vertices)
        {
            return (vertices[0] + vertices[vertices.Length - 2]) / 2;
        }

        /// <summary>
        /// Gets a custom pivot for a char
        /// </summary>
        /// <param name="offset">Offset from the middle</param>
        /// <returns></returns>
        public Vector3 GetCustomPivot(Vector3 offset)
        {
            return GetCustomPivot(SourceVertices, offset);
        }

        public static Vector3 GetCustomPivot(TextBounds bounds, Vector3 offset)
        {
            return GetCustomPivot(new[]
                {
                    bounds.BotLeft,
                    new Vector3(bounds.BotLeft.x, bounds.TopRight.y),
                    bounds.TopRight,
                    new Vector3(bounds.TopRight.x, bounds.BotLeft.y)
                },
                offset);
        }

        public static Vector3 GetCustomPivot(Vector3[] vertices, Vector3 offset)
        {
            Vector3 middle = GetMiddlePoint(vertices);
            float horizontalSize = vertices[vertices.Length - 1].x - vertices[0].x;
            float verticalSize = vertices[1].y - vertices[0].y;

            return middle + new Vector3(offset.x * horizontalSize, offset.y * verticalSize);
        }
    }
}