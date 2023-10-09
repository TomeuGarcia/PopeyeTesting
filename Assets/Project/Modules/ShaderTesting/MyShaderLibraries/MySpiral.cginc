float makeSpiral(float2 uv, float numberOfSpikes, float spikeSize, float spikeFade, float spikeOriginOffset, 
                float twistStraightness, float rotationSpeed, float time)
{
    uv -= 0.5f;
    uv *= 2.0f;

    float2 st = float2(atan2(uv.x, uv.y), length(uv));

    float twist = pow(st.y, twistStraightness);

    uv = float2(st.x / UNITY_TWO_PI + 0.5f + time * rotationSpeed + twist, st.y);

    float x = uv.x * numberOfSpikes;
    float m = min(frac(x), frac(1.0f - x));
    float c = smoothstep(0.0f, spikeFade, (m*spikeSize) + spikeOriginOffset - uv.y);

    return c;
}