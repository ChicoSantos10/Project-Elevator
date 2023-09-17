using SSSTools.Extensions;
using UnityEngine;

namespace SSSTools.FunText.Core
{
    [CreateAssetMenu(fileName = "New Text Animation", menuName = SssToolsPaths.FunTextAnimationObjectPath, order = 0)]
    public class TextAnimationDataObject : ScriptableObject
    {
        [SerializeField] TextAnimation animation;

        public TextAnimation Animation => animation;
        
        public static implicit operator TextAnimation(TextAnimationDataObject o) => o.animation;
    }
}