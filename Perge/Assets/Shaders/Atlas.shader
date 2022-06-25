// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

// Simplified Diffuse shader. Differences from regular Diffuse one:
// - no Main Color
// - fully supports only 1 directional light. Other lights can affect it, but it will be per-vertex/SH.

Shader "Atlas/Standart" {
    Properties{
        _MainTex("Base (RGB)", 2D) = "white" {}
        _DistanceTex("Distance (RGB)", 2D) = "white" {}
        _CameraPositon("Camera",  Vector) = (1,1,1,1)
    }
        SubShader{
            Tags { "RenderType" = "Opaque" }
            LOD 150

        CGPROGRAM
        #pragma surface surf Lambert noforwardadd

        sampler2D _MainTex;
        sampler2D _DistanceTex;

        struct Input {
            float2 uv_MainTex;
            float3 worldPos;
        };

        float4 _CameraPositon;

        void surf(Input IN, inout SurfaceOutput o) {
            
            float d = distance(IN.worldPos, _CameraPositon.xyz);

            float l = (1 / (d / 10));

            if (l > 1)
            {
                l = 1;
            }

            fixed4 c = lerp(tex2D(_DistanceTex, IN.uv_MainTex), tex2D(_MainTex, IN.uv_MainTex), l);

            o.Albedo = c.rgb;

            if (c.a <= 0)
            {
                discard;
            }

        }
        ENDCG
    }

        Fallback "Mobile/VertexLit"
}