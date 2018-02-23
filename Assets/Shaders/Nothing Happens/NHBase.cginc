#ifndef NH_BASE
#define NH_BASE

float fog(float3 posWorld, half4 cube, float amount, float exp, float offset, float limit)
{
    // distance
    float d = -1 * distance(_WorldSpaceCameraPos,posWorld);

    // fog, clamped 0-1
    float f = cube.a * saturate(1.0 - pow(exp, (amount * d) + offset));

    // return clamped by limit
    return clamp(f, 0.0, limit);
}

#endif