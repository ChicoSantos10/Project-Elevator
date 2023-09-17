namespace SSSTools.FunText.AnimationTypes
{
    internal class VerticalDisplacer : IVertexDisplacer
    {
        public float GetDisplacement(int id)
        {
            return id > 0 && id < 3 ? 1 : 0;
        }
    }
}