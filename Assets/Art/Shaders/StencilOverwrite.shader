Shader "Stencil/StencilOverwrite"{
	//show values to edit in inspector
	Properties{
		[IntRange] _StencilRef ("Stencil Reference Value", Range(0,255)) = 0
	}

	SubShader{
		//the material is completely non-transparent and is rendered at the same time as the other opaque geometry
		Tags{ "RenderType"="Opaque" "Queue"="Geometry-1"}

        //stencil operation
		Stencil{
			Ref [_StencilRef]
			Comp Less
			Pass Replace 
		}

		Pass{
            //don't draw color or depth
			Blend Zero One
			ZWrite Off

			CGPROGRAM
			#include "UnityCG.cginc"

			#pragma vertex vert
			#pragma fragment frag

			//the depth texture
			sampler2D _CameraDepthTexture;

			struct appdata{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f{
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert(appdata v){
				v2f o;
				//calculate the position in clip space to render the object
				o.position = UnityObjectToClipPos(v.vertex);
				o.uv=v.uv;
				return o;
			}

			fixed4 frag(v2f i) : SV_TARGET{
				float depth = Linear01Depth(tex2D(_CameraDepthTexture, i.uv));
				if(depth >= 0.999)
				{
					discard;
				}
				return depth;
			}

			ENDCG
		}
	}
}