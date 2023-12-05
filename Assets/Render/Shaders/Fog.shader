// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/Fog"{
    Properties{
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader{

        Pass{
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata{
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                half4 color : COLOR0;
            };

            struct interpolators{
                noperspective float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                half4 color : COLOR0;
            };

            interpolators vert (appdata v){
                interpolators o;

                // Vertex
                float4 snapToPixel = UnityObjectToClipPos(v.vertex);
                float4 vertex = snapToPixel;
                vertex.xyz = snapToPixel.xyz / snapToPixel.w;
                vertex.x = floor(256 * vertex.x) / 256;
                vertex.y = floor(224 * vertex.y) / 224;
                vertex.xyz *= snapToPixel.w;
                o.vertex = vertex;

                // Vertex lighting
                o.color = v.color * UNITY_LIGHTMODEL_AMBIENT;

                // o.vertex = UnityObjectToClipPos(v.vertex)
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex, _CameraDepthTexture;
            float4 _FogColor;
            float _FogDensity, _FogOffset;

            fixed4 frag (interpolators i) : SV_Target{
                int x, y;
                float4 col = tex2D(_MainTex, i.uv);
                float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
                depth = Linear01Depth(depth);

                float viewDistance = depth * _ProjectionParams.z;

                float fogFactor = (_FogDensity / sqrt(log(2))) * max(0.0f, viewDistance - _FogOffset);
                fogFactor = exp2(-fogFactor * fogFactor);

                float4 fogOutput = lerp(_FogColor, col, saturate(fogFactor));
                return fogOutput;
            }
            ENDCG
        }
    }
}
