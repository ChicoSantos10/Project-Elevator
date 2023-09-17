using System;
using SSSTools.FunText.Core;
using UnityEngine;
using AnimationInfo = SSSTools.FunText.Core.AnimationInfo;

namespace SSSTools.FunText.Effects
{
    [Serializable]
    [Obsolete]
    public class RainbowEffect : IBaseEffect
    {
        [SerializeField] float hueShiftSpeed;
        [SerializeField] float hueShiftWaveSize;
        
        public void PlayAnimation(ref CharacterData data, IndexInfo indexInfo, AnimationInfo info)
        {
            for (int i = 0; i < data.Colors.Length; i++)
            {
                data.Colors[i] = Color.HSVToRGB(
                    Mathf.PingPong(info.T * hueShiftSpeed + (indexInfo.IndexNoInvisibleChars + i / 2) * hueShiftWaveSize, 1), 1, 1);
            }
        }
    }
}