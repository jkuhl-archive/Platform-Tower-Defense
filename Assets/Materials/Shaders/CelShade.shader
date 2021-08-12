Shader "CelShade"
{
	Properties
	{
		// Base properties
		[MainTexture]
		base_texture("Base Texture", 2D) = "white" {}
		[MainColor]
		base_color("Base Color", Color) = (0.7, 0.7, 0.7, 1)
		
		// Shading properties
		[HDR]
		shading_color("Shading Color", Color) = (0.2,0.2,0.2,1)
		
		// Gloss properties
		gloss_enabled("Gloss Enabled", Int) = 0
		
		[HDR]
		gloss_color("Gloss Color", Color) = (0.75,0.75,0.75,1)
		gloss_factor("Gloss Factor", Float) = 50.0
	}
	SubShader
	{
		Pass
		{
			Tags
			{
				"LightMode" = "ForwardBase"
				"PassFlags" = "OnlyDirectional"
			}
			
			// Define what the shader is doing
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase

			#include "AutoLight.cginc"
			#include "Lighting.cginc"
			#include "UnityCG.cginc"

			// Base property variables
			sampler2D base_texture;
			float4 base_texture_ST;
			float4 base_color;

			// Shading property variables
			float4 shading_color;

			// Gloss property variables
			bool gloss_enabled;
			float4 gloss_color;
			float gloss_factor;

			struct appdata
			{
				float3 normal : NORMAL;
				float4 vertex : POSITION;				
				float4 uv : TEXCOORD0;
			};

			struct v2f
			{
				SHADOW_COORDS(2)
				float3 worldNormal : NORMAL;
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 viewDir : TEXCOORD1;
			};
		
			v2f vert (appdata v)
			{
				v2f o;
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.viewDir = WorldSpaceViewDir(v.vertex);
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, base_texture);
				TRANSFER_SHADOW(o)
				return o;
			}
			
			float4 frag (v2f i) : SV_Target
			{
				float3 normal = normalize(i.worldNormal);
				float4 sample = tex2D(base_texture, i.uv);


				// Setup lighting and shadows
				float NdotL = dot(_WorldSpaceLightPos0, normal);
				float shadow = SHADOW_ATTENUATION(i);
				float lightIntensity = smoothstep(0, 0.01, NdotL * shadow);
				float4 light = lightIntensity * _LightColor0;

				// Setup gloss
				float3 viewDir = normalize(i.viewDir);
				float3 halfVector = normalize(_WorldSpaceLightPos0 + viewDir);
				float NdotH = dot(normal, halfVector);
				float glossItensity = pow(NdotH * lightIntensity, gloss_factor * gloss_factor);
				float glossItensitySmooth = smoothstep(0.005, 0.01, glossItensity);
				float4 gloss = glossItensitySmooth * gloss_color;


				// Calculate processed color
				float4 processing = shading_color + light + gloss;
				if (!gloss_enabled)
				{
					processing = shading_color + light;
				}

				return base_color * sample * processing;
			}
			ENDCG
		}
		
		UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
	}
}
