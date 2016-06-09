Shader "Custom/FogShader" {
	Properties {
		_FogColor("Fog Color", Color) = (1,1,1,1)
		_LeftMost("Left Most", Float) = -1.0
		_RightMost("Right Most", Float) = 1.0
		_TopMost("Top Most", Float) = -1.0
		_BottomMost("Bottom Most", Float) = 1.0
		_Falloff("Falloff", Float) = 1.0
	}

	SubShader{
		Tags{ "Queue" = "Transparent" }

		Pass{
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM

			// Define the vertex and fragment shader functions
			#pragma vertex vert
			#pragma fragment frag 

			// Access Shaderlab properties
			uniform float4 _FogColor;
			uniform float _LeftMost;
			uniform float _RightMost;
			uniform float _TopMost;
			uniform float _BottomMost;
			uniform float _Falloff;

			// Input into the vertex shader
			struct vertexInput {
				float4 vertex : POSITION;
			};


			// Output from vertex shader into fragment shader
			struct vertexOutput {
				float4 pos : SV_POSITION;
				float4 worldPos : TEXCOORD0;
			};


			// VERTEX SHADER
			vertexOutput vert(vertexInput input) {
				vertexOutput output;
				output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
				// Calculate the world position coordinates to pass to the fragment shader
				output.worldPos = mul(_Object2World, input.vertex);
				return output;
			}
			// FRAGMENT SHADER
			float4 frag(vertexOutput input) : COLOR{
				float2 topLeft = float2(_LeftMost, _TopMost);
				float2 bottomRight = float2(_RightMost, _BottomMost);
				if (input.worldPos.x > topLeft.x && input.worldPos.x < bottomRight.x && input.worldPos.y > topLeft.y && input.worldPos.y < bottomRight.y) {
					return float4(0.0, 0.0, 0.0, 0.0);
				}
				else {
					return _FogColor;
				}
			}
			ENDCG
		}
	}
}
