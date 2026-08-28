// Face
uniform sampler2D	_FaceTex;
uniform float		_FaceUVSpeedX;
uniform float		_FaceUVSpeedY;
uniform fixed4		_FaceColor;
uniform float		_FaceDilate;

// Outline
uniform fixed4		_OutlineColor;
uniform sampler2D	_OutlineTex;
uniform float		_OutlineUVSpeedX;
uniform float		_OutlineUVSpeedY;
uniform float		_OutlineWidth;
uniform float		_OutlineSoftness;

// Bevel
uniform float		_Bevel;
uniform float		_BevelOffset;
uniform float		_BevelWidth;
uniform float		_BevelClamp;
uniform float		_BevelRoundness;

// Light
uniform float		_LightAngle;
uniform fixed4		_SpecularColor;
uniform float		_SpecularPower;
uniform float		_Reflectivity;
uniform float		_Diffuse;
uniform float		_Ambient;

// Bump Map
uniform sampler2D	_BumpMap;
uniform float		_BumpOutline;
uniform float		_BumpFace;

// Reflection
uniform fixed4		_ReflectFaceColor;
uniform fixed4		_ReflectOutlineColor;
uniform samplerCUBE	_Cube;
uniform float4		_EnvMatrixRotation;

// Underlay
uniform fixed4		_UnderlayColor;
uniform float		_UnderlayOffsetX;
uniform float		_UnderlayOffsetY;
uniform float		_UnderlayDilate;
uniform float		_UnderlaySoftness;

// Glow
uniform fixed4		_GlowColor;
uniform float		_GlowOffset;
uniform float		_GlowInner;
uniform float		_GlowOuter;
uniform float		_GlowPower;

// Weights
uniform float		_WeightNormal;
uniform float		_WeightBold;

// Shader flags & ratios
uniform float		_ShaderFlags;
uniform float		_ScaleRatioA;
uniform float		_ScaleRatioB;
uniform float		_ScaleRatioC;

// Atlas
uniform sampler2D	_MainTex;
uniform float		_TextureWidth;
uniform float		_TextureHeight;
uniform float		_GradientScale;
uniform float		_ScaleX;
uniform float		_ScaleY;
uniform float		_PerspectiveFilter;
uniform float		_Sharpness;

// Vertex offset
uniform float		_VertexOffsetX;
uniform float		_VertexOffsetY;

// Masking
uniform float4		_MaskCoord;
uniform float4		_ClipRect;
uniform float		_MaskSoftnessX;
uniform float		_MaskSoftnessY;

// Stencil
uniform float		_StencilComp;
uniform float		_Stencil;
uniform float		_StencilOp;
uniform float		_StencilWriteMask;
uniform float		_StencilReadMask;

uniform float		_CullMode;
uniform float		_ColorMask;