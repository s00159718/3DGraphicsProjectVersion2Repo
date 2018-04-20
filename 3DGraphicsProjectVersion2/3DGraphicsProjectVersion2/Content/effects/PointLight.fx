float4x4 World;
float4x4 View;
float4x4 Projection;

texture Texture;
bool TextureEnabled = false;

float3 AmbientColor = float3(.15, .15, .15);
float3 DiffuseColor = float3(1, 1, 1);
float3 LightColor = float3(1, 0, 0);

float3 Position = float3(0, 0, 10);
float Attenuation = 20;
float Falloff = 2;

float3 CameraPosition = float3(1, 1, 1);
float SpecularPower = 32;
float3 SpecularColor = float3(1, 0, 0);

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
		float3 lightDir = normalize(Position - input.WorldPosition);
		float3 normal = normalize(input.Normal);
		float3 diffuse = saturate(dot(normal, lightDir));

		float3 refl = reflect(lightDir, normal);
		float3 view = normalize(input.ViewDirection);
		float d = distance(Position, input.WorldPosition);

		//inverse square implementation
		float att = 1 - pow(clamp(d / Attenuation, 0, 1), Falloff);
		//color += pow(saturate(dot(refl, view)), SpecularPower) * SpecularColor;

		lighting += diffuse * att * LightColor;
		
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