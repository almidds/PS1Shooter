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
            };

            struct interpolators{
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            interpolators vert (appdata v){
                interpolators o;
                o.vertex = UnityObjectToClipPos(v.vertex);
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
