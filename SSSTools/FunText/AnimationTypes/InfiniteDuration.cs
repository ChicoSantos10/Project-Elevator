using SSSTools.FunText.Core;

namespace SSSTools.FunText.AnimationTypes
{
    internal class InfiniteDuration : IDuration
    {
        public bool IsComplete() => false;

        public void Update(float timePassed, float speed) { }
    }
}