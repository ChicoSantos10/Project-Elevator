namespace SSSTools.FunText.AnimationTypes
{
    public interface IDuration
    {
        bool IsComplete();
        void Update(float timePassed, float speed);
    }
}