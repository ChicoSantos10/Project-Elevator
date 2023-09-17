Shader "Hidden/ViewNormalShader"
{
    Properties
    {
        _ID("ID", Int) = 0
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD 100

        Stencil
        {
            Ref[_ID]
            Comp Equal
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 view_normal : NORMAL;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.view_normal = COMPUTE_VIEW_NORMAL;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return float4(normalize(i.view_normal) * 0.5 + 0.5, 0);
            }
            ENDCG
        }
    }
}