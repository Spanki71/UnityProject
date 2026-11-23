// Colour of the showroom studio. The floor and the backdrop share it, so no horizon line shows between them.
#ifndef CODEWART_SHOWROOM_STUDIO
#define CODEWART_SHOWROOM_STUDIO

float4 _ShowroomCenter, _ShowroomEdge;
float4 _ShowroomFocus;      // xy: middle of the pool of light on screen, z: its radius in screen heights

// soft pool of light, with a touch of noise that keeps the gradient free of banding
float3 StudioColor(float4 screen, float2 pixel)
{
    float2 offset = (screen.xy / screen.w - _ShowroomFocus.xy) * float2(_ScreenParams.x / _ScreenParams.y, 1.0);
    float r = saturate(length(offset) / _ShowroomFocus.z);
    float noise = frac(52.9829189 * frac(dot(pixel, float2(0.06711056, 0.00583715))));
    return lerp(_ShowroomCenter.rgb, _ShowroomEdge.rgb, r * r * (3.0 - 2.0 * r)) + (noise - 0.5) / 128.0;
}

#endif
