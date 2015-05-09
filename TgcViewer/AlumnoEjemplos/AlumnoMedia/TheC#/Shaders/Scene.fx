// ---------------------------------------------------------
// Ejemplo shader Minimo:
// ---------------------------------------------------------

/**************************************************************************************/
/* Variables comunes */
/**************************************************************************************/

//Matrices de transformacion
float4x4 matWorld; //Matriz de transformacion World
float4x4 matWorldView; //Matriz World * View
float4x4 matWorldViewProj; //Matriz World * View * Projection
float4x4 matInverseTransposeWorld; //Matriz Transpose(Invert(World))

float4 DiffuseColor;
float4 Direction;

/**************************************************************************************/
/* RenderScene */
/**************************************************************************************/

texture texSlope;
sampler2D texSlopeSampler = sampler_state
{
	Texture = (texSlope);
	ADDRESSU = WRAP;
	ADDRESSV = WRAP;
	MINFILTER = LINEAR;
	MAGFILTER = LINEAR;
	MIPFILTER = LINEAR;
};

texture texRock;
sampler2D texRockSampler = sampler_state
{
	Texture = (texRock);
	ADDRESSU = WRAP;
	ADDRESSV = WRAP;
	MINFILTER = LINEAR;
	MAGFILTER = LINEAR;
	MIPFILTER = LINEAR;
};

//Output del Vertex Shader
struct VS_OUTPUT 
{
   float4 Position :        POSITION0;
   float2 Texcoord :        TEXCOORD0;
   float3 Norm :			TEXCOORD1;
};

// ------------------------------------------------------------------
VS_OUTPUT vs_main( float4 Pos:POSITION0,float3 Normal:NORMAL, float2 Texcoord:TEXCOORD0 )
{
    VS_OUTPUT Output;

    // Change the position vector to be 4 units for proper matrix calculations.
    Pos.w = 1.0f;

    // Calculate the position of the vertex against the world, view, and projection matrices.
    Output.Position = mul(Pos, matWorld);
    Output.Position = mul(Pos, matWorldView);
    Output.Position = mul(Pos, matWorldViewProj);
    
    //Propago  las coord. de textura 
    Output.Texcoord  = Texcoord;

    //Transformo la normal y la normalizo
	Output.Norm = normalize(mul(Normal,matInverseTransposeWorld));

    return( Output );
  
}

//Pixel Shader
float4 ps_main( float2 Texcoord: TEXCOORD0, float3 N:TEXCOORD1, float4 Pos: TEXCOORD2) : COLOR0
{      
    float4 slopeColor;
    float4 rockColor;
    float slope;
    float blendAmount;
    float4 textureColor;
    float3 lightDir;
    float lightIntensity;
    float4 color;

    // Sample the grass color from the texture using the sampler at this texture coordinate location.
    slopeColor = tex2D( texSlopeSampler, Texcoord );

    // Sample the slope color from the texture using the sampler at this texture coordinate location.
    rockColor = tex2D( texRockSampler, Texcoord );

    // Calculate the slope of this point.
    slope = 1.0f - N.y;

    if(slope < 0.5)
    {
        blendAmount = (slope - 0.2f) * (1.0f / (0.7f - 0.2f));
        textureColor = lerp(slopeColor, rockColor, blendAmount);
    }

    if(slope >= 0.5) 
    {
        textureColor = rockColor;
    }

    // Invert the light direction for calculations.
    lightDir = -Direction;

    // Calculate the amount of light on this pixel.
    lightIntensity = saturate(dot(N, lightDir));
	
    // Determine the final diffuse color based on the diffuse color and the amount of light intensity.
    color = DiffuseColor * lightIntensity;

    // Saturate the final light color.
    color = saturate(color);

    // Multiply the texture color and the final light color to get the result.
    color = color * textureColor;

    return color;
}


// ------------------------------------------------------------------
technique RenderScene
{
   pass Pass_0
   {
	  VertexShader = compile vs_3_0 vs_main();
	  PixelShader = compile ps_3_0 ps_main();
   }

}
