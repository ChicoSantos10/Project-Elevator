Shader "Hidden/ColorMapShader"
{
    Properties
    {
        [HideInInspector] _MainTex ("", 2D) = "white" {}
        _ColorMap("Color Map", 2D) = "white" {}
        _Intensity("Color Intensity", Range(0, 5)) = 1 
        _Strength("Strength", Range(0,1)) = 0.5
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            sampler2D _ColorMap;
            float _Intensity;
            float _Strength;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                // We get the color in gamma space. This is important for effects like bloom
                const fixed4 col_intensity = col.r + col.g + col.b;
                const fixed4 color = col;
                const fixed4 uv = color * _Intensity;

                // Multiply by the intensity to preserve the gamma color when remapping
                const fixed4 map_color = tex2D(_ColorMap, uv);
                
                return (map_color * _Strength + color * (1 - _Strength)) * col_intensity;
            }
            ENDCG
        }
    }
}
