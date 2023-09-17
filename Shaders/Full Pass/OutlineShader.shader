Shader "Project_Elevator/OutlineShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _EdgeColor("Edge color", Color) = (0,0,0,1)
        _EdgeColorTolerance("Color tolerance", Range(0, 5)) = 0.01
        _EdgeNormalTolerance("Normal tolerance", Range(0, 2)) = 0.01
        _EdgeDepthTolerance("Depth tolerance", Range(0, 2)) = 0.01
        [IntRange] _Scale("Scale", Range(1,10)) = 1
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
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 view_space_dir : TEXCOORD1;
            };

            float4x4 _ClipToView;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.view_space_dir = mul(_ClipToView, v.vertex).xyz;
                return o;
            }

            sampler2D _MainTex;
            sampler2D _CameraColorTexture;
            sampler2D _CameraDepthTexture;
            sampler2D _CameraDepthNormalsTexture;

            half4 _EdgeColor;

            float _EdgeColorTolerance;
            float _EdgeNormalTolerance;
            float _EdgeDepthTolerance;
            int _Scale;

            static const int roberts_cross_x[4] = {
                1, 0,
                0, -1
            };

            static const int roberts_cross_y[4] = {
                0, 1,
                -1, 0
            };

            //#define DEBUG_NORMAL
            //#define DEBUG_DEPTH

            void DecodeDepthNormals(float2 uv, out float depth, out float3 normal)
            {
                DecodeDepthNormal(tex2D(_CameraDepthNormalsTexture, uv), depth, normal);
                depth = Linear01Depth(depth);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                #if defined(DEBUG_NORMAL) || defined(DEBUG_DEPTH)

                float debug_depth;
                float3 debug_normal;
                DecodeDepthNormals(i.uv, debug_depth, debug_normal);
                
                #ifdef DEBUG_NORMAL

                //return float4(DecodeViewNormalStereo(tex2D(_CameraDepthNormalsTexture, i.uv)), 1);
                //return tex2D(_CameraDepthNormalsTexture, i.uv);
                return float4(debug_normal, 1);

                #endif
                #ifdef DEBUG_DEPTH

                //return SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
                return debug_depth;

                #endif
                #endif

                float2 uv_step = 1 / _ScreenParams.xy;

                const float half_scale_floor = floor(_Scale * 0.5);
                const float half_scale_ceil = ceil(_Scale * 0.5);

                // Sample the pixels in an X shape, roughly centered around i.texcoord.
                // As the _CameraDepthTexture and _CameraNormalsTexture default samplers
                // use point filtering, we use the above variables to ensure we offset
                // exactly one pixel at a time.
                const float2 bottom_left_uv = i.uv - uv_step * half_scale_floor;
                const float2 top_right_uv = i.uv + uv_step * half_scale_ceil;
                const float2 bottom_right_uv = i.uv + float2(uv_step.x * half_scale_ceil,
                                                             - uv_step.y * half_scale_floor);
                const float2 top_left_uv = i.uv + float2(-uv_step.x * half_scale_floor, uv_step.y * half_scale_ceil);

                half4 color = tex2D(_MainTex, i.uv);

                float4 color_samples[] =
                {
                    tex2D(_CameraColorTexture, top_left_uv),
                    tex2D(_CameraColorTexture, top_right_uv),
                    tex2D(_CameraColorTexture, bottom_left_uv),
                    tex2D(_CameraColorTexture, bottom_right_uv),
                };

                float3 top_left_normal;
                float top_left_depth;
                DecodeDepthNormals(top_left_uv, top_left_depth, top_left_normal);

                float3 top_right_normal;
                float top_right_depth;
                DecodeDepthNormals(top_right_uv, top_right_depth, top_right_normal);

                float3 bottom_left_normal;
                float bottom_left_depth;
                DecodeDepthNormals(bottom_left_uv, bottom_left_depth, bottom_left_normal);

                float3 bottom_right_normal;
                float bottom_right_depth;
                DecodeDepthNormals(bottom_right_uv, bottom_right_depth, bottom_right_normal);

                float3 normal_samples[] =
                {
                    top_left_normal,
                    top_right_normal,
                    bottom_left_normal,
                    bottom_right_normal
                };

                float depth_samples[] =
                {
                    top_left_depth,
                    top_right_depth,
                    bottom_left_depth,
                    bottom_right_depth
                };

                float4 color_horizontal = 0, color_vertical = 0;

                color_horizontal += color_samples[0] * roberts_cross_x[0];
                color_horizontal += color_samples[3] * roberts_cross_x[3];

                color_vertical += color_samples[1] * roberts_cross_y[1];
                color_vertical += color_samples[2] * roberts_cross_y[2];

                const int color_edge = sqrt(dot(color_horizontal, color_horizontal) + dot(color_vertical, color_vertical)) >
                    _EdgeColorTolerance;

                float3 normal_horizontal = 0, normal_vertical = 0;

                normal_horizontal += normal_samples[0] * roberts_cross_x[0];
                normal_horizontal += normal_samples[3] * roberts_cross_x[3];

                normal_vertical += normal_samples[1] * roberts_cross_y[1];
                normal_vertical += normal_samples[2] * roberts_cross_y[2];

                const int normal_edge = sqrt(dot(normal_horizontal, normal_horizontal) + dot(normal_vertical, normal_vertical)) >
                    _EdgeNormalTolerance;

                float depth_horizontal = 0, depth_vertical = 0;

                depth_horizontal += depth_samples[0] * roberts_cross_x[0];
                depth_horizontal += depth_samples[3] * roberts_cross_x[3];

                depth_vertical += depth_samples[1] * roberts_cross_y[1];
                depth_vertical += depth_samples[2] * roberts_cross_y[2];

                float depth;
                float3 normal;
                DecodeDepthNormals(i.uv, depth, normal);
                
                float depth_tolerance = _EdgeDepthTolerance * depth;

                float3 viewNormal = normal * 2 - 1;
                float NdotV = 1 - dot(viewNormal, -i.view_space_dir);

                float normal_tolerance = saturate((NdotV - _EdgeNormalTolerance) / (1 - _EdgeNormalTolerance));

                depth_tolerance *= normal_tolerance * 7 + 1;

                const int depth_edge = sqrt(depth_horizontal * depth_horizontal + depth_vertical * depth_vertical) > depth_tolerance;

                half edge = max(color_edge, max(normal_edge, depth_edge));

                return edge * _EdgeColor + (1 - edge) * color;
            }
            ENDCG
        }
    }
}