Shader "Custom/EyeVignetteShdr"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OffsetX ("Offset X", Float) = 0.0
        _OffsetY("Offset Y", Float) = 0.0
        _Exponential ("Exponential", Float) = 1.0
		_Color("Color", Color) = (1,1,1,1)
        _PlayerDying("Player Dying", Int) = 0
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
                fixed4 color : COLOR;
            };

            float _OffsetX;
			float _OffsetY;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float _Exponential;
            float4 _Color;
			int _PlayerDying;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                
                
                if (_PlayerDying == 0)
                {
                    return col;
                }
                else
                {
                    float radial = sin(i.uv.x + _OffsetX) * 0.95 * cos(i.uv.y + _OffsetY) * 0.95;
                    radial = pow(radial, _Exponential * 6);
                    return lerp(_Color, col, radial);
                }

				
                
            }
            ENDCG
        }
    }
}
