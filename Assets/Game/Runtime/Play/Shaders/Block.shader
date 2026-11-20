// Blocks are outside the board only while leaving, so drawing them only within the board and its frame (BoardClip)
// cuts a leaving block at the frame's outer face. Back faces show only through that cut and are drawn flat, so the cut
// reads as a solid face.
Shader "Game/Block"
{
    Properties
    {
        [MainColor] _BaseColor("Color", Color) = (1, 1, 1, 1)
        _ShadowColor("Shadow Color", Color) = (0.5, 0.5, 0.5, 1)
        _RampThreshold("Ramp Threshold", Range(-1, 1)) = 0.5
        _RampSoftness("Ramp Softness", Range(0.01, 2)) = 1
        _GradientShade("Gradient Bottom Shade", Range(0, 1)) = 0.85
        _GradientHeight("Gradient Height", Range(0.01, 2)) = 0.52
        _RimStart("Rim Start", Range(0, 1)) = 0.2
        _RimSoftness("Rim Softness", Range(0.01, 1)) = 0.3
        _RimStrength("Rim Strength", Range(0, 1)) = 0.4
        _CutShade("Cut Face Shade", Range(0, 1)) = 0.8
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "Queue" = "Geometry" }

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        CBUFFER_START(UnityPerMaterial)
            half4 _BaseColor;
            half4 _ShadowColor;
            half _RampThreshold;
            half _RampSoftness;
            half _GradientShade;
            float _GradientHeight;
            half _RimStart;
            half _RimSoftness;
            half _RimStrength;
            half _CutShade;
        CBUFFER_END

        float4x4 _BoardWorldToLocal;
        float4 _BoardHalfSize;

        void ClipToBoard(float3 positionWS)
        {
            float3 onBoard = mul(_BoardWorldToLocal, float4(positionWS, 1)).xyz;
            clip(_BoardHalfSize.xy - abs(onBoard.xz));
        }
        ENDHLSL

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }
            Cull Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float height : TEXCOORD2;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                VertexPositionInputs position = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = position.positionCS;
                output.positionWS = position.positionWS;
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                // Above the board's ground: the supplied pieces are turned inside their prefabs, so their own Y
                // is not up.
                output.height = mul(_BoardWorldToLocal, float4(position.positionWS, 1)).y;
                return output;
            }

            half4 Frag(Varyings input, FRONT_FACE_TYPE face : FRONT_FACE_SEMANTIC) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                ClipToBoard(input.positionWS);
                if (!IS_FRONT_VFACE(face, true, false))
                    return half4(_BaseColor.rgb * _CutShade, 1);

                // Toon shading, as in the reference: the block's color toward the light whatever its strength, a shadow
                // color of its own (not a darker copy) away from it, a little darker toward the block's bottom.
                float3 normal = normalize(input.normalWS);
                float3 toLight = GetMainLight().direction;
                half lit = smoothstep(_RampThreshold - _RampSoftness * 0.5h, _RampThreshold + _RampSoftness * 0.5h, dot(normal, toLight));
                half shade = lerp(_GradientShade, 1.0h, saturate(input.height / _GradientHeight));
                half3 color = lerp(_ShadowColor.rgb, _BaseColor.rgb, lit) * shade;
                // Faces leaning toward the light's side (the far half of each stud, the far edges) catch it brighter
                // than the flat top, which is what makes the studs read round.
                half lean = dot(normal, SafeNormalize(float3(toLight.x, 0, toLight.z)));
                half rim = smoothstep(_RimStart, _RimStart + _RimSoftness, lean) * _RimStrength;
                return half4(lerp(color, 1.0h, rim), 1);
            }
            ENDHLSL
        }

        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode" = "DepthOnly" }
            ZWrite On
            ColorMask R
            Cull Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile_instancing

            struct Attributes
            {
                float4 positionOS : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
                output.positionCS = TransformWorldToHClip(output.positionWS);
                return output;
            }

            half Frag(Varyings input) : SV_Target
            {
                ClipToBoard(input.positionWS);
                return input.positionCS.z;
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
