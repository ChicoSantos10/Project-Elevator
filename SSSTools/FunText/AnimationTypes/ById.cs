namespace SSSTools.FunText.AnimationTypes
{
    internal class ById : IVertexDisplacer
    {
        public float GetDisplacement(int id)
        {
            return id;
        }
    }
}