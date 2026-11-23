// Floor of the showroom: the studio colour, unlit, so it melts into the backdrop. It only takes the key light shadow and a soft contact shade under the character.
// The first SubShader serves the Universal Render Pipeline and is skipped when that package is missing, the second one serves the Built-in Render Pipeline.
Shader "Codewart/Showroom Floor"
{
    Properties
    {
        _ShadowDepth("Shadow depth", Range(0, 1)) = 0.42
        _ContactDepth("Contact shade depth", Range(0, 1)) = 0.3
        _ContactRadius("Contact shade radius", Float) = 0.7
    }
    SubShader
    {
        PackageRequirements { "com.unity.render-pipelines.universal" }
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
        Pass
        {
            Name "Floor"
            Tags { "LightMode" = "UniversalForward" }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile_fragment _ _SHADOWS_SOFT _SHADOWS_SOFT_LOW _SHADOWS_SOFT_MEDIUM _SHADOWS_SOFT_HIGH
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "ShowroomStudio.cginc"

            float _ShadowDepth, _ContactDepth, _ContactRadius;

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 pos : SV_POSITION;
                float4 screen : TEXCOORD0;
                float3 world : TEXCOORD1;
            };

            Varyings vert(Attributes v)
            {
                VertexPositionInputs p = GetVertexPositionInputs(v.positionOS.xyz);
                Varyings o;
                o.pos = p.positionCS;
                o.screen = p.positionNDC;
                o.world = p.positionWS;
                return o;
            }

            float4 frag(Varyings i) : SV_Target
            {
                float away = length(i.world.xz);
                float contact = saturate(1.0 - away / _ContactRadius);
                float shadow = lerp(MainLightRealtimeShadow(TransformWorldToShadowCoord(i.world)), 1, smoothstep(8.0, 11.0, away));     // the far floor stays clean, whatever the range of the shadow map
                float shade = lerp(1 - _ShadowDepth, 1, shadow) * (1 - _ContactDepth * contact * contact);
                return float4(StudioColor(i.screen, i.pos.xy) * shade, 1);
            }
            ENDHLSL
        }
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        Pass
        {
            Tags { "LightMode" = "ForwardBase" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "ShowroomStudio.cginc"

            fixed _ShadowDepth, _ContactDepth;
            float _ContactRadius;

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 screen : TEXCOORD0;
                float2 ground : TEXCOORD1;
                SHADOW_COORDS(2)
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.screen = ComputeScreenPos(o.pos);
                o.ground = mul(unity_ObjectToWorld, v.vertex).xz;
                TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float away = length(i.ground);
                float contact = saturate(1.0 - away / _ContactRadius);
                fixed shadow = lerp(SHADOW_ATTENUATION(i), 1, smoothstep(8.0, 11.0, away));     // the far floor stays clean, whatever the range of the shadow map
                fixed shade = lerp(1 - _ShadowDepth, 1, shadow) * (1 - _ContactDepth * contact * contact);
                return fixed4(StudioColor(i.screen, i.pos.xy) * shade, 1);
            }
            ENDCG
        }
    }
    FallBack "VertexLit"    // its depth pass lets shadows fall on the floor
}
