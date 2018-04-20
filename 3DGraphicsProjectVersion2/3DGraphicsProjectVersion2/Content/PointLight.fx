#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

matrix World;
matrix View;
matrix Projection;

float3 AmbientLightColor = float3(.15, .15, .15);
float3 DiffuseColor = float3(1, 1, 1);
float3 LightColor = float3(1, 1, 1);

float3 LightPosition = float3(0, 0, 0);
float Attenuation = 40;
float FallOff = 2;

struct VertexShaderInput
{
	float4 Position : POSITION0;
    float3 Normal : NORMAL0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
    float3 Normal : TEXCOORD1;
    float3 WorldPosition : TEXCOORD2;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

    float4 worldPos = mul(input.Position, World);
    float4 viewPos = mul(worldPos, View);
    output.Position = mul(viewPos, Projection);

    output.WorldPosition = worldPos;
    output.Normal = normalize(mul(input.Normal, World));

	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float3 color = DiffuseColor;
    float3 lightingColor = AmbientLightColor;
    float3 lightDirection = normalize(LightPosition - input.WorldPosition);
    float3 angle = saturate(dot(input.Normal, lightDirection));
    float dist = distance(LightPosition, input.WorldPosition);

    float atten = 1 - pow(clamp(dist / Attenuation, 0, 1), FallOff);

    lightingColor += angle * atten * LightColor;
    return float4(color * lightingColor, 1);

}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};