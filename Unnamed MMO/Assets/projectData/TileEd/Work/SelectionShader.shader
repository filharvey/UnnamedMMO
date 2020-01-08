Shader "Hidden/TileEd/SelectionShader"
{
	Properties{
		_MainColor("Main Color", Color) = (1, 1, 1, 1)
		_RimColor("Rim Color", Color) = (1, 1, 1, 1)
		_RimPower("Rim Power", Range(0, 8)) = 3.0
		_BehindColor("Behind Color", Color) = (1, 1, 1, 1)
	}
	
	SubShader
	{
		Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "True" "ForceNoShadowCasting"="True" }

		Lighting Off
		Cull Off
		ZWrite Off
		ZTest Greater
		Offset -1, -1
		ColorMask RGBA
		Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM
		#pragma surface surf Unlit keepalpha noshadow noambient novertexlights nolightmap nodirlightmap nofog nometa

		struct Input
		{
			float3 viewDir;
			float3 worldNormal;
		};

		float4 _BehindColor;

		half4 LightingUnlit(SurfaceOutput s, half3 lightDir, half atten)
		{
			return half4(s.Albedo, s.Alpha);
		}

		void surf(Input IN, inout SurfaceOutput OUT)
		{
			OUT.Albedo = _BehindColor.rgb;
			OUT.Alpha = _BehindColor.a;
		}

		ENDCG

		Lighting Off
		Cull Off
		ZWrite Off
		ZTest LEqual
		Offset -1, -1
		ColorMask RGBA
		Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM
		#pragma surface surf Unlit keepalpha noshadow noambient novertexlights nolightmap nodirlightmap nofog nometa

		struct Input
		{
			float3 viewDir;
			float3 worldNormal;
		};

		float4 _MainColor;
		float4 _RimColor;
		float _RimPower;

		half4 LightingUnlit(SurfaceOutput s, half3 lightDir, half atten)
		{
			return half4(s.Albedo, s.Alpha);
		}

		void surf(Input IN, inout SurfaceOutput OUT)
		{
			half rim = 1.0 - saturate(dot(IN.viewDir, IN.worldNormal));
			OUT.Albedo = _MainColor.rgb;
			OUT.Alpha = clamp(max(pow(rim, _RimPower), _MainColor.a), 0.0, 1.0);
			OUT.Emission = _RimColor.rgb * pow(rim, _RimPower);
		}

		ENDCG
	}

	Fallback "Diffuse"
}
