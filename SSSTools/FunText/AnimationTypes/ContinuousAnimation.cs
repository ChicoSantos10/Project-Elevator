namespace SSSTools.FunText.AnimationTypes
{
    internal class ContinuousAnimation : IAnimationType
    {
        public float ComputeT(float t)
        {
            return t;
        }
    }
}