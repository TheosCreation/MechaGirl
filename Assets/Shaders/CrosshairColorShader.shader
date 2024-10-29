Shader "Custom/CrosshairColorShader"
{
    Properties
    {
        _MainTex ("RenderTexture", 2D) = "white" {}
        _StencilRef ("Stencil Reference", Range(0, 255)) = 1 // Stencil reference value
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Overlay" }
        LOD 100

        Pass
        {
            Stencil
            {
                Ref [_StencilRef] // Use the stencil reference property
                Comp Equal
                Pass Keep
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float2 _ScreenCenterUV = float2(0.5, 0.5); // Center of the screen

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                // Sample the color from the RenderTexture
                float4 screenColor = tex2D(_MainTex, _ScreenCenterUV);

                // Calculate the luminance of the sampled color
                float luminance = dot(screenColor.rgb, float3(0.299, 0.587, 0.114));

                // If the luminance is above 0.5, use black, otherwise use white
                float4 invertedColor = luminance > 0.5 ? float4(0, 0, 0, 1) : float4(1, 1, 1, 1);

                return invertedColor; // Return the inverted color for the crosshair
            }
            ENDCG
        }
    }
}
