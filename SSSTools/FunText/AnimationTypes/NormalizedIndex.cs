using SSSTools.FunText.Core;

namespace SSSTools.FunText.AnimationTypes
{
    internal class NormalizedIndex : IIndex
    {
        public float GetIndex(IndexInfo info)
        {
            return info.NormalizedIndex;
        }
    }
}