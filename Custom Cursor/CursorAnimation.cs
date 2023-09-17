using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Custom_Cursor
{
    [CreateAssetMenu(fileName = "New Cursor Animation", menuName = "Cursor/Animation", order = 0)]
    public class CursorAnimation : ScriptableObject, IEnumerator<Texture2D>
    {
        enum OffsetType
        {
            TopRight,
            TopLeft,
            BotRight,
            BotLeft,
            Center
        }
        
        [SerializeField] Texture2D[] sprites;
        [SerializeField] OffsetType offset;
        [SerializeField, Range(0.1f, 5)] float speed = 1;

        int _index = -1;

        public Vector2 Offset
        {
            get
            {
                return offset switch
                {
                    OffsetType.TopRight => new Vector2(Current!.width, 0),
                    OffsetType.TopLeft => Vector2.zero,
                    OffsetType.BotRight => new Vector2(Current!.width, Current.height),
                    OffsetType.BotLeft => new Vector2(0, Current!.height),
                    OffsetType.Center => new Vector2(Current!.width / 2f, Current.height / 2f),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        } 
            
        public Texture2D Current => sprites[_index];

        object IEnumerator.Current => Current;

        public float Speed
        {
            get => speed;
            set => speed = value;
        }

        public bool MoveNext()
        {
            _index++;

            return _index < sprites.Length;
        }

        public void Reset()
        {
            _index = -1;
        }
        
        public void Dispose()
        {
        }
    }
}