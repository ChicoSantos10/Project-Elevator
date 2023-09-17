namespace SSSTools.FunText.AnimationTypes
{
    internal class HorizontalDisplacer : IVertexDisplacer
    {
        public float GetDisplacement(int id)
        {
            return id / 2;
        }
    }
}