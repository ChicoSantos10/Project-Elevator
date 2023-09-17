using SSSTools.FunText.Core;
using UnityEngine;
using AnimationInfo = SSSTools.FunText.Core.AnimationInfo;

namespace SSSTools.FunText.Effects
{
    internal class HorizontalSquash : Squash
    {
        public override void PlayAnimation(ref CharacterData data, IndexInfo indexInfo, AnimationInfo info)
        {
            float index = _index.GetIndex(indexInfo);
            Vector3 botPivot = data.GetCustomPivot(new Vector3(offset, -0.5f));
            Vector3 topPivot = data.GetCustomPivot(new Vector3(offset, 0.5f));
            
            for (int i = 0; i < data.Vertices.Length; i++)
            {
                Vector3 vertex = data.Vertices[i];

                Vector3 pivot = i switch
                {
                    0 => botPivot,
                    3 => botPivot,
                    _ => topPivot
                };

                Vector3 dir = pivot - vertex;
                
                //float y = curve.Evaluate(info.T, index, i) * magnitude * vertex.y * dir * 0.5f; // We multiply by .5f since we want magnitude of 1 to be double/half the size
                data.Vertices[i] += curve.Evaluate(info.T, index, i) * magnitude * dir;
            }
        }
    }
}