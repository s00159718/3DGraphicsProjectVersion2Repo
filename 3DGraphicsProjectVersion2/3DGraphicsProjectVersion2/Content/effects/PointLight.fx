float4x4 World;
float4x4 View;
float4x4 Projection;

texture Texture;
bool TextureEnabled = true;

float3 Position[3];

float3 LightColor[3];
float3 AmbientColor = float3(.15, .15, .15);
float3 DiffuseColor = float3(1, 1, 1);
float3 SpecularColor[3];

float Attenuation[3];
float Falloff[3];

float3 CameraPosition = float3(1, 1, 1);
float SpecularPower = 100;


sampler BasicTextureSampler = sampler_state
{
	texture = <Texture>;
};

struct VertexShaderInput
{
	float4 Position : SV_Position0;
	float2 UV : TEXCOORD0;
	float3 Normal : NORMAL0;
};

struct VertexShaderOutput
{
	float4 Position : SV_Position0;
	float2 UV : TEXCOORD0;
	float3 Normal : TEXCOORD1;
	float4 WorldPosition : TEXCOORD2;
	float3 ViewDirection : TEXCOORD3;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;

	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);

	output.Position = mul(viewPosition, Projection);
	output.WorldPosition = worldPosition;
	output.UV = input.UV;
	output.Normal = normalize(mul(input.Normal, World));
	output.ViewDirection = worldPosition - CameraPosition;

	return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float3 color = DiffuseColor;

	if (TextureEnabled)
		color *= tex2D(BasicTextureSampler, input.UV);

	float3 lighting = AmbientColor;

	for (int i = 0; i < 3; i++)
	{
		float3 SpecularColor = normalize(SpecularColor[i]);
		float3 lightDir = normalize(Position[i] - input.WorldPosition);
		float3 normal = normalize(input.Normal);
		float3 diffuse = saturate(dot(normal, lightDir));

		float3 view = normalize(input.ViewDirection);
		float d = distance(Position[i], input.WorldPosition);

		float att = 1 - pow(clamp(d / Attenuation[i], 0, 1), Falloff[i]);

		float3 refl = reflect(lightDir, normal);

		lighting += diffuse * att * LightColor[i];
	}
	return float4(color * lighting, 1);
}

technique Technique1
{
	pass Pass1
	{
		VertexShader = compile vs_5_0 VertexShaderFunction();
		PixelShader = compile ps_5_0 PixelShaderFunction();
	}
}