float4x4 World;
float4x4 View;
float4x4 Projection;

struct VertexShaderInput
{
    float4 Position : SV_Position0;
	float3 Normal : NORMAL0;
};

struct VertexShaderOutput
{
    float4 Position : SV_Position0;
	float2 Depth : TEXCOORD0;
	float3 Normal : TEXCOORD1;
};


VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

	float4x4 viewProjection = mul(View, Projection);
	float4x4 worldViewProjection = mul(World, viewProjection);

	output.Position = mul(input.Position, worldViewProjection);
	output.Normal = mul(input.Normal, World);

	//camera to far plane distance
	//scaling transformation
	//z is the vertex Z
	//w is the the distance from the camera
	output.Depth.xy = output.Position.zw;

    return output;
}


struct PixelShaderOutput
{
	float4 Normal : COLOR0;
	float4 Depth : COLOR1;
};

PixelShaderOutput PixelShaderFunction(VertexShaderOutput input)
{
	PixelShaderOutput output;

	float x = input.Depth.x / input.Depth.y;
	output.Depth = float4(x, x, x, x);

	//multiply each component by 2 and add .5
	//shift values from [-1,1] to [0,1]
	output.Normal.xyz = (normalize(input.Normal) / 2) + .5;

	output.Depth.a = 1;
	output.Normal.a = 1;

	return output;
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_5_0 VertexShaderFunction();
        PixelShader = compile ps_5_0 PixelShaderFunction();
    }
}
