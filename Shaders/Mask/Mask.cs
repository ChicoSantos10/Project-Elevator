using System;
using UnityEngine;

namespace Shaders.Mask
{
    public class Mask : MonoBehaviour
    {
        static readonly int ShaderID = Shader.PropertyToID("_ID");
        
        [SerializeField, Range(0, 255)] int id = 1;
        Material _material;

        public int Id
        {
            get => id;
            set
            {
                id = value;
                SetShaderId();
            }
        }

        public Material Material
        {
            get
            {
                if (_material == null)
                    _material = GetComponent<MeshRenderer>().material;

                return _material;
            }
        }

        void Awake()
        {
            SetShaderId();
        }

        void SetShaderId()
        {
            Material.SetInt(ShaderID, Id);
        }
    }
}