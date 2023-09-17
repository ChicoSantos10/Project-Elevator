using SSSTools.FunText.Core;

namespace SSSTools.FunText.AnimationTypes
{
    internal class IndexBase0NoInvisibleChars : IIndex
    {
        public float GetIndex(IndexInfo info)
        {
            return info.IndexNoInvisibleChars;
        }
    }
}