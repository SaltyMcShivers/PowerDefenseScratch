Shader "Unlit/SpiderWebShader"
{
	Properties
	{

		_GridThicknessRadius("Grid Thickness Radius", Float) = 0.01
		_GridThicknessAngle("Grid Thickness Angle", Float) = 0.01
		_GridSpacingRadius("Grid Spacing Radius", Float) = 1.0
		_GridSpacingAngle("Grid Spacing Angle", Float) = 10.0
		_GridOffsetRadius("Grid Offset Radius", Float) = 0
		_GridOffsetAngle("Grid Offset Angle", Float) = 0
		_GridColour("Grid Colour", Color) = (0.5, 1.0, 1.0, 1.0)
		_BaseColour("Base Colour", Color) = (0.0, 0.0, 0.0, 0.0)
		_GridColourOff("Grid Colour Off", Color) = (0.5, 1.0, 1.0, 1.0)
		_BaseColourOff("Base Colour Off", Color) = (0.0, 0.0, 0.0, 0.0)

		_StartOffset("Start Offset", Float) = -10.0
		_EndOffset("End Offset", Float) = -10.0
		_TransitionSpeed("Transition Speed", Float) = 1.0
		_TransitionSize("Transistion Size", Float) = 0.0
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
			uniform float _GridThicknessRadius;
			uniform float _GridThicknessAngle;
			uniform float _GridSpacingRadius;
			uniform float _GridSpacingAngle;
			uniform float _GridOffsetRadius;
			uniform float _GridOffsetAngle;
			uniform float4 _GridColour;
			uniform float4 _BaseColour;
			uniform float4 _GridColourOff;
			uniform float4 _BaseColourOff;

			uniform float _StartOffset;
			uniform float _EndOffset;
			uniform float _TransitionSpeed;
			uniform float _TransitionSize;

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
				float4 objectOrigin = mul(_Object2World, float4(0.0,0.0,0.0,1.0));

				float2 origin2D = float2(objectOrigin.x, objectOrigin.y);
				float2 input2D = float2(input.worldPos.x, input.worldPos.y);

				float trans;
				if (_StartOffset > _EndOffset) {
					if (distance(origin2D, input2D) > (_Time.y - _StartOffset) * _TransitionSpeed) {
						trans = 0;
					}
					else if (distance(origin2D, input2D) + _TransitionSize > (_Time.y - _StartOffset) * _TransitionSpeed) {
						trans = ((_Time.y - _StartOffset) * _TransitionSpeed - distance(origin2D, input2D)) / _TransitionSize;
					}
					else 
					{
						trans = 1;
					}
				}
				else
				{
					if (distance(origin2D, input2D) > (_Time.y - _EndOffset) * _TransitionSpeed) {
						trans = 1;
					}
					else if (distance(origin2D, input2D) + _TransitionSize > (_Time.y - _EndOffset) * _TransitionSpeed) {
						trans = 1 - ((_Time.y - _EndOffset) * _TransitionSpeed - distance(origin2D, input2D)) / _TransitionSize;
					}
					else {
						trans = 0;
					}
				}
				if (frac((distance(origin2D, input2D) + _GridOffsetRadius) / _GridSpacingRadius) < (_GridThicknessRadius / _GridSpacingRadius)) {
					return lerp(_GridColourOff, _GridColour, trans);
				}
				else {
					float2 refVector = float2(1.0, 0.0);
					float2 diffVector = input2D - origin2D;
					float angleDOT = dot(normalize(refVector), normalize(diffVector));
					float rad = acos(angleDOT);

					float closestRadian = radians(round((degrees(rad)) / _GridSpacingAngle) * _GridSpacingAngle * sign(diffVector.y) + _GridOffsetAngle);

					if (abs(closestRadian - rad) > abs(closestRadian - rad + radians(_GridSpacingAngle))) closestRadian += radians(_GridSpacingAngle);
					else if (abs(closestRadian - rad) > abs(closestRadian - rad - radians(_GridSpacingAngle))) closestRadian -= radians(_GridSpacingAngle);

					//Not closestPoint, but works for now
					float2 radianPoint = float2(origin2D.x + cos(closestRadian) * distance(origin2D, input2D), origin2D.y + sin(closestRadian) * distance(origin2D, input2D));

					if (distance(radianPoint, input2D) < (_GridThicknessAngle)) {
						return lerp(_GridColourOff, _GridColour, trans);
					}
					else {
						return lerp(_BaseColourOff, _BaseColour, trans);
					}
				}
			}
			ENDCG
		}
	}
}
