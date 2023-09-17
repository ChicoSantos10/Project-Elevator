using SSSTools.Extensions;
using UnityEngine;

namespace SSSTools.FunText.Core
{
    [CreateAssetMenu(fileName = "New Show Character Effect", menuName = SssToolsPaths.FunTextCharEffectObjectPath, order = 0)]
    public class ShowCharacterEffectDataObject : ScriptableObject
    {
        [SerializeField] ShowCharEffectTag effect;

        public ShowCharacterEffect Effect => effect.Effect;
        
        public string Tag => effect.Tag;
        
        public static implicit operator ShowCharacterEffect(ShowCharacterEffectDataObject o) => o.Effect;
    }
}