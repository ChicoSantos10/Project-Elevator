using SSSTools.FunText.Core;

namespace SSSTools.FunText.AnimationTypes
{
    internal class RealIndex : IIndex
    {
        public float GetIndex(IndexInfo info)
        {
            return info.RealIndex;
        }
    }
}