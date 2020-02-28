// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "MK4/Foliage Fantasy no wind"
{
	Properties
	{
		_Albedo("Albedo", 2D) = "gray" {}
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_NormalMap("Normal Map", 2D) = "bump" {}
		_Composite("Composite", 2D) = "black" {}
		_NormalPower("Normal Power", Range( 0 , 5)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Geometry+0" }
		Cull Off
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows exclude_path:deferred 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float _NormalPower;
		uniform sampler2D _NormalMap;
		uniform float4 _NormalMap_ST;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform sampler2D _Composite;
		uniform float4 _Composite_ST;
		uniform float _Cutoff = 0.5;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_NormalMap = i.uv_texcoord * _NormalMap_ST.xy + _NormalMap_ST.zw;
			o.Normal = UnpackScaleNormal( tex2D( _NormalMap, uv_NormalMap ), _NormalPower );
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float4 tex2DNode22 = tex2D( _Albedo, uv_Albedo );
			o.Albedo = tex2DNode22.rgb;
			float2 uv_Composite = i.uv_texcoord * _Composite_ST.xy + _Composite_ST.zw;
			o.Smoothness = tex2D( _Composite, uv_Composite ).r;
			o.Alpha = 1;
			clip( tex2DNode22.a - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16400
62;42;1541;991;1473.891;1072.544;1;True;True
Node;AmplifyShaderEditor.RangedFloatNode;39;-1030.478,-597.9401;Float;False;Property;_NormalPower;Normal Power;4;0;Create;True;0;0;False;0;0;1;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;37;-693.596,-688.6545;Float;True;Property;_NormalMap;Normal Map;2;0;Create;True;0;0;False;0;None;50774eb8fbbd8ea4287779309859c547;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;22;-688.7999,-887.2222;Float;True;Property;_Albedo;Albedo;0;0;Create;True;0;0;False;0;None;c43db6ab7d0b4164eb5c3930747e50e9;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;38;-692.5126,-502.1854;Float;True;Property;_Composite;Composite;3;0;Create;True;0;0;False;0;None;c43db6ab7d0b4164eb5c3930747e50e9;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;305.9751,-361.7126;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;MK4/Foliage Fantasy no wind;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;TransparentCutout;;Geometry;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;37;5;39;0
WireConnection;0;0;22;0
WireConnection;0;1;37;0
WireConnection;0;4;38;0
WireConnection;0;10;22;4
ASEEND*/
//CHKSM=4CD495FBB341FC58DB69195FFA9E8F985A045D8B