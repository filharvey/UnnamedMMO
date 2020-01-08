Shader "Hidden/TileEd/GridShader1"
{
	Properties 
	{
		_MainTex("Diffuse", 2D) = "white" {}
		_Tint("Tint", Color) = (1, 1, 1, 1)
	}
	
	SubShader 
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" "ForceNoShadowCasting"="True"}
		
		Lighting Off
		Cull Off
		ZWrite Off
		ZTest LEqual
		Offset -1,-1
		ColorMask RGBA
		Blend SrcAlpha OneMinusSrcAlpha
	
		CGPROGRAM
		#pragma surface surf Unlit keepalpha noshadow noambient novertexlights nolightmap nodirlightmap nofog nometa

		struct Input
		{
			float2 uv_MainTex;
		};

		sampler2D _MainTex;
		fixed4 _Tint;

		half4 LightingUnlit(SurfaceOutput s, half3 lightDir, half atten)
		{
			return half4(s.Albedo, s.Alpha);
		}

		void surf(Input IN, inout SurfaceOutput OUT)
		{
			OUT.Albedo = _Tint.rgb;
			OUT.Alpha = min(tex2D(_MainTex, IN.uv_MainTex).a, _Tint.a);
		}

		ENDCG
	}
	
	Fallback "Diffuse"
}