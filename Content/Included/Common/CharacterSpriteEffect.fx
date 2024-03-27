#if OPENGL
#define _vs(r)  : register(vs, r)
#define _ps(r)  : register(ps, r)
#define _cb(r)
#define VS_MODEL vs_3_0
#define PS_MODEL ps_3_0
 
#define BEGIN_PARAMETERS
#define END_PARAMETERS
 
#define SAMPLE(texture, texCoord) tex2D(texture, texCoord) 
 
    sampler2D Texture : register(s0);
#else
#define _vs(r)
#define _ps(r)
#define _cb(r)
#define VS_MODEL vs_4_0_level_9_1
#define PS_MODEL ps_4_0_level_9_1
 
#define BEGIN_PARAMETERS    cbuffer Parameters : register(b0) {
#define END_PARAMETERS      };
 
#define SAMPLE(texture, texCoord) texture.Sample(texture##Sampler, texCoord)
 
Texture2D<float4> Texture : register(t0);
sampler TextureSampler : register(s0);
#endif
 
BEGIN_PARAMETERS
    float4x4 MatrixTransform _vs(c0) _cb(c0);
END_PARAMETERS
     
struct VSOutput
{
    float4 position : SV_Position;
    float4 color : COLOR0;
    float2 texCoord : TEXCOORD0;
};
 
VSOutput SpriteVertexShader(float4 position : POSITION0,
                                float4 color : COLOR0,
                                float2 texCoord : TEXCOORD0)
{
    VSOutput output;
 
    output.position = mul(position, MatrixTransform);
    output.color = color;
    output.texCoord = texCoord;
     
    return output;
}
 
float4 SpritePixelShader(VSOutput input) : SV_Target0
{
    float4 color = SAMPLE(Texture, input.texCoord) * input.color;
    float4 black = float4(0.0f, 0.0f, 0.0f, 1.0f);
    float4 clear = float4(0.0039215686f, 0.0039215686f, 0.0039215686f, 1.0f);
    
    if (color.r == 0.0f && color.g == 0.0f && color.b == 0.0f && color.a != 0.0f)
    {
        return clear;
    }
    else
    {
        return color;
    }
}
 
technique SpriteBatch
{
    pass
    {
        VertexShader = compile VS_MODEL SpriteVertexShader();
        PixelShader = compile PS_MODEL SpritePixelShader();
    }
};