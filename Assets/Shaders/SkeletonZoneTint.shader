Shader "DungeonKnight/SkeletonZoneTint"
{
    Properties
    {
        _Color ("Hit Tint", Color) = (1, 1, 1, 1)
        _BoneColor ("Bone Color", Color) = (0.86, 0.82, 0.68, 1)
        _ArmorColor ("Armor Color", Color) = (0.14, 0.13, 0.14, 1)
        _TrimColor ("Trim Color", Color) = (0.58, 0.40, 0.16, 1)
        _RustColor ("Rust Color", Color) = (0.50, 0.18, 0.06, 1)
        _ClothColor ("Cloth Color", Color) = (0.18, 0.18, 0.14, 1)
        _MinY ("Min Y", Float) = -1
        _MaxY ("Max Y", Float) = 1
        _ArmorStart ("Armor Start", Range(0, 1)) = 0.43
        _ArmorEnd ("Armor End", Range(0, 1)) = 0.72
        _HelmetStart ("Helmet Start", Range(0, 1)) = 0.82
        _BeltCenter ("Belt Center", Range(0, 1)) = 0.40
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            Name "Forward"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 normalWS : TEXCOORD0;
                float3 positionOS : TEXCOORD1;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float4 _BoneColor;
                float4 _ArmorColor;
                float4 _TrimColor;
                float4 _RustColor;
                float4 _ClothColor;
                float _MinY;
                float _MaxY;
                float _ArmorStart;
                float _ArmorEnd;
                float _HelmetStart;
                float _BeltCenter;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInputs = GetVertexNormalInputs(input.normalOS);
                output.positionCS = positionInputs.positionCS;
                output.normalWS = normalInputs.normalWS;
                output.positionOS = input.positionOS.xyz;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float heightRange = max(0.0001, _MaxY - _MinY);
                float h = saturate((input.positionOS.y - _MinY) / heightRange);

                float chestMask = smoothstep(_ArmorStart, _ArmorStart + 0.045, h)
                    * (1.0 - smoothstep(_ArmorEnd, _ArmorEnd + 0.055, h));
                float helmetMask = smoothstep(_HelmetStart, _HelmetStart + 0.06, h);
                float armorMask = saturate(max(chestMask, helmetMask));

                float beltMask = 1.0 - smoothstep(0.035, 0.075, abs(h - _BeltCenter));
                float clothHeightMask = smoothstep(0.16, 0.22, h) * (1.0 - smoothstep(0.43, 0.5, h));
                float clothCenterMask = 1.0 - smoothstep(0.2, 0.5, abs(input.positionOS.x));
                float clothMask = saturate(clothHeightMask * clothCenterMask);
                float rustMask = saturate(sin(input.positionOS.x * 31.0 + input.positionOS.y * 17.0) * 0.5 + 0.5);
                rustMask *= armorMask * 0.28;

                float3 color = _BoneColor.rgb;
                color = lerp(color, _ClothColor.rgb, clothMask);
                color = lerp(color, _ArmorColor.rgb, armorMask);
                color = lerp(color, _TrimColor.rgb, beltMask);
                color = lerp(color, _RustColor.rgb, rustMask);

                Light mainLight = GetMainLight();
                float3 normalWS = normalize(input.normalWS);
                float lit = saturate(dot(normalWS, mainLight.direction)) * 0.7 + 0.32;
                float3 finalColor = color * _Color.rgb * lit;
                return half4(finalColor, 1.0);
            }
            ENDHLSL
        }
    }

    Fallback "Diffuse"
}
