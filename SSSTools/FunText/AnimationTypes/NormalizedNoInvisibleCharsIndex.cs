using SSSTools.FunText.Core;

namespace SSSTools.FunText.AnimationTypes
{
    internal class NormalizedNoInvisibleCharsIndex : IIndex
    {
        public float GetIndex(IndexInfo info)
        {
            return info.NormalizedNoInvisibleChars;
        }
    }
}