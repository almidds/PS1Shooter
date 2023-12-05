Shader "Hidden/CRT"{
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

            sampler2D _MainTex;
            float _Curvature, _VignetteWidth;

            fixed4 frag (interpolators i) : SV_Target{
                float2 uv = i.uv * 2.0f - 1.0f;
                float2 offset = uv.yx / _Curvature;
                uv = uv + uv * offset * offset;
                uv = uv * 0.5f + 0.5f;

                fixed4 col = tex2D(_MainTex, uv);
                if (uv.x < 0.0f || uv.x > 1.0f || uv.y < 0.0f || uv.y > 1.0f) {
                    col = 0;
                }

                uv = uv * 2.0f - 1.0f;
                float2 vignette = _VignetteWidth / _ScreenParams.xy;
                vignette = smoothstep(0.0f, vignette, 1.0f - abs(uv));
                vignette = saturate(vignette);

                col.g *= (sin(i.uv.y * _ScreenParams.y * 2.0f) + 1.0f) * 0.15f + 1.0f;
                col.rb *= (cos(i.uv.y * _ScreenParams.y * 2.0f) + 1.0f) * 0.135f + 1.0f; 

                return saturate(col) * vignette.x * vignette.y;
            }
            ENDCG
        }
    }
}
