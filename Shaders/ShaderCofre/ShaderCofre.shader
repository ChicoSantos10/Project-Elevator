Shader "Unlit/ShaderCofre"
{
    Properties
    {
       [Toggle] _Active("ActiveButton", Int) = 0
       _Color("color", Color) = (1,0,0,1)
       _Intensity("Intensity", Range(0,5)) = 1
       _Speed("Speed", Range(0,5)) = 0.5
       
       
       
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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
                float4 vertex : SV_POSITION;
            };

           int _Active;
           float _Intensity;
           fixed4 _Color;
           float _Speed;
     
           

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
             
              
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //amplitude * (sin(frequencia, tempoDesdeInicio, DeslocaçãoHorizontal) + DeslocaçaoVertical) 
                float wave = 0.5 * (sin(_Time.y * _Speed) + 1);

                wave *= _Active; //ligar e desligar a wave
           
                fixed4 col = _Color * wave + fixed4(1,1,1,1) * (1 - wave); 
                            
                return col * _Intensity ;

            }
            ENDCG
        }
    }
}
