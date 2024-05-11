Shader "Hidden/AVProMovieCapture/SideBySideAlpha"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
		Lighting Off
		LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			#pragma multi_compile ALPHA_TOP_BOTTOM ALPHA_LEFT_RIGHT
			#pragma multi_compile __ FLIPPED SCREEN_FLIPPED

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

#if ALPHA_TOP_BOTTOM
	#if FLIPPED || (SCREEN_FLIPPED && !(SHADER_API_MOBILE && SHADER_API_VULKAN))
				o.uv.y = 1.0 - ((o.vertex.y + 1.0) / 2.0);
	#else
				o.uv.y = (o.vertex.y + 1.0) / 2.0;
	#endif
#else
				o.uv.x = (o.vertex.x + 1.0) / 2.0;
	#if FLIPPED || (SCREEN_FLIPPED && !(SHADER_API_MOBILE && SHADER_API_VULKAN))
				// Intentionally blank
	#else
				o.uv.y = (o.vertex.y + 1.0) / 2.0;
	#endif
#endif

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				float4 col;
#if ALPHA_TOP_BOTTOM
	#if SCREEN_FLIPPED
				if( i.uv.y > 0.5 )
				{
					float texAlpha = tex2D(_MainTex, float2(i.uv.x, (i.uv.y * 2.0) - 1.0)).a; 
					col = float4(texAlpha, texAlpha, texAlpha, 1.0);
				}
				else
				{
					col = tex2D(_MainTex, float2(i.uv.x, (i.uv.y * 2.0)));
				}
	#else
				if( i.uv.y < 0.5 )
				{
					float texAlpha = tex2D(_MainTex, float2(i.uv.x, (i.uv.y * 2.0))).a;
					col = float4(texAlpha, texAlpha, texAlpha, 1.0);
				}
				else
				{
					col = tex2D(_MainTex, float2(i.uv.x, ((i.uv.y * 2.0) - 1.0)));
				}
	#endif
#else
				if( i.uv.x > 0.5 )
				{
					float texAlpha = tex2D(_MainTex, float2( ((i.uv.x * 2.0) - 1.0), i.uv.y)).a;
					col = float4(texAlpha, texAlpha, texAlpha, 1.0);
				}
				else
				{
					col = tex2D(_MainTex, float2((i.uv.x * 2.0), i.uv.y));
				}
#endif

				return col;
			}
            ENDCG
        }
    }
}
