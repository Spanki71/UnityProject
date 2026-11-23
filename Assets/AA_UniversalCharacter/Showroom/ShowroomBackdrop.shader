// Backdrop of the showroom: the studio colour behind everything. Unlit, so one pass serves the Universal and the Built-in Render Pipeline.
Shader "Codewart/Showroom Backdrop"
{
    SubShader
    {
        Tags { "RenderType" = "Background" "Queue" = "Background" "IgnoreProjector" = "True" }
        ZWrite Off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "ShowroomStudio.cginc"

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 screen : TEXCOORD0;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.screen = ComputeScreenPos(o.pos);
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                return float4(StudioColor(i.screen, i.pos.xy), 1);
            }
            ENDCG
        }
    }
}
