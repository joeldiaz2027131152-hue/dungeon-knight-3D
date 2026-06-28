Shader "DungeonKnight/SkeletonZoneTint"
{
    Properties
    {
        _Color ("Hit Tint", Color) = (1, 1, 1, 1)
        _BoneColor ("Bone Color", Color) = (0.63, 0.57, 0.45, 1)
        _ArmorColor ("Armor Color", Color) = (0.105, 0.11, 0.115, 1)
        _TrimColor ("Trim Color", Color) = (0.43, 0.39, 0.32, 1)
        _RustColor ("Rust Color", Color) = (0.38, 0.16, 0.075, 1)
        _ClothColor ("Cloth Color", Color) = (0.045, 0.043, 0.04, 1)
        _GlowColor ("Glow Color", Color) = (0, 0.82, 1, 1)
        _GlowStrength ("Glow Strength", Range(0, 1)) = 0
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
        }

        Pass
        {
            Name "Forward"
            Tags { "LightMode" = "ForwardBase" }

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
                float3 normalWS : TEXCOORD0;
                float3 positionOS : TEXCOORD1;
            };

            float4 _Color;
            float4 _BoneColor;
            float4 _ArmorColor;
            float4 _TrimColor;
            float4 _RustColor;
            float4 _ClothColor;
            float4 _GlowColor;
            float _GlowStrength;
            float _MinY;
            float _MaxY;
            float _ArmorStart;
            float _ArmorEnd;
            float _HelmetStart;
            float _BeltCenter;

            v2f vert(appdata input)
            {
                v2f output;
                output.vertex = UnityObjectToClipPos(input.vertex);
                output.normalWS = UnityObjectToWorldNormal(input.normal);
                output.positionOS = input.vertex.xyz;
                return output;
            }

            fixed4 frag(v2f input) : SV_Target
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
                float chestGlowMask = (1.0 - smoothstep(0.04, 0.23, abs(h - 0.55)))
                    * (1.0 - smoothstep(0.07, 0.38, abs(input.positionOS.x)));
                float eyeGlowMask = smoothstep(0.84, 0.9, h)
                    * (1.0 - smoothstep(0.03, 0.22, abs(input.positionOS.x)));
                float glowMask = saturate(max(chestGlowMask, eyeGlowMask) * _GlowStrength);
                float rustMask = saturate(sin(input.positionOS.x * 31.0 + input.positionOS.y * 17.0) * 0.5 + 0.5);
                rustMask *= armorMask * 0.28;

                float3 color = _BoneColor.rgb;
                color = lerp(color, _ClothColor.rgb, clothMask);
                color = lerp(color, _ArmorColor.rgb, armorMask);
                color = lerp(color, _TrimColor.rgb, beltMask);
                color = lerp(color, _RustColor.rgb, rustMask);

                float3 normalWS = normalize(input.normalWS);
                float3 lightDirection = normalize(float3(0.35, 0.8, 0.45));
                float lit = saturate(dot(normalWS, lightDirection)) * 0.7 + 0.32;
                float3 finalColor = color * _Color.rgb * lit;
                finalColor = lerp(finalColor, _GlowColor.rgb, glowMask * 0.75);
                finalColor += _GlowColor.rgb * glowMask * 0.35;
                return fixed4(finalColor, 1.0);
            }
            ENDCG
        }
    }

    Fallback "Diffuse"
}
