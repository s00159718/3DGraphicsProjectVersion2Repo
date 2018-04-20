float4x4 WorldViewProjection;
float4x4 InverseViewProjection;

float3 LightColor;
float3 LightPosition;
float LightAttenuation;
float LightFalloff = 2;

texture2D DepthTexture;
texture2D NormalTexture;

sampler2D DepthSampler = sampler_state
{
	texture = <DepthTexture>;
};

sampler2D NormalSampler = sampler_state
{
	texture = <NormalTexture>;
};

struct VertexShaderInput
{
	float4 Position : SV_Position0;
};

struct VertexShaderOutput
{
	float4 Position : SV_Position0;
	float4 LightPosition : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;

	output.Position = mul(input.Position, WorldViewProjection);
	output.LightPosition = output.Position;

	return output;
}

float2 ProjectToScreen(float4 position)
{
	float2 screenPos = position.xy / position.w;
	return 0.5f * (float2(screenPos.x, -screenPos.y) + 1);
};

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	//2D screen position of the 3D light position
	float2 texCoord = ProjectToScreen(input.LightPosition);
	//depth value at this pixel position
	float4 depth = tex2D(DepthSampler, texCoord);

	float4 position;
	//X nad Y position will be sampled from UV space
	position.x = texCoord.x * 2 - 1;
	position.y = (1 - texCoord.y) * 2 - 1;

	//depth stored in Red channel of the Depth Texture
	position.z = depth.r;
	position.w = 1.0f;

	//screen to world
	position = mul(position, InverseViewProjection);
	//division for a floating point variable
	position.xyz /= position.w;

	//opposite of normal capture
	//move from [0,1] to [-1,1] range
	float4 normal = (tex2D(NormalSampler, texCoord) - .5) * 2;

	float3 lightDirection = normalize(LightPosition - position);
	float lighting = clamp(dot(normal, lightDirection), 0, 1);

	float d = distance(LightPosition, position);
	float att = 1 - pow(clamp(d / LightAttenuation, 0, 1), LightFalloff);

	return float4 (LightColor * lighting * att, 1);
}

technique Technique1
{
	pass Pass1
	{
		VertexShader = compile vs_5_0 VertexShaderFunction();
		PixelShader = compile ps_5_0 PixelShaderFunction();
	}
}

