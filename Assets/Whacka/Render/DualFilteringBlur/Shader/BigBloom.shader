Shader "Hidden/BigBloom"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ThroughValue ("Through Vale", Range(0, 2)) = 1.1
    }

    CGINCLUDE
        
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
    float4 _MainTex_TexelSize;
    float _ThroughValue;
        
    fixed RgbMax(fixed4 c)
    {
        return max(c.r, max(c.g, c.b));
    }

    half4 downsample(float2 uv, float2 halfpixel)
    {
        half4 sum = tex2D(_MainTex, uv) * 4.0;
        sum += tex2D(_MainTex, uv - halfpixel.xy);
        sum += tex2D(_MainTex, uv + halfpixel.xy);
        sum += tex2D(_MainTex, uv + float2(halfpixel.x, -halfpixel.y));
        sum += tex2D(_MainTex, uv - float2(halfpixel.x, -halfpixel.y));
        return sum / 8.0;
    }

    half4 downsample2(float2 uv, float2 halfpixel)
    {
        half4 sum = 0.0f;
        sum += tex2D(_MainTex, uv - halfpixel.xy);
        sum += tex2D(_MainTex, uv + halfpixel.xy);
        sum += tex2D(_MainTex, uv + float2(halfpixel.x, -halfpixel.y));
        sum += tex2D(_MainTex, uv - float2(halfpixel.x, -halfpixel.y));
        float filter = step(1, sum.a);
        return sum / sum.a * filter;
    }

    half4 upsample(float2 uv, float2 halfpixel)
    {
        half4 sum = tex2D(_MainTex, uv + float2(-halfpixel.x * 2.0, 0.0));
        sum += tex2D(_MainTex, uv + float2(-halfpixel.x, halfpixel.y)) * 2.0;
        sum += tex2D(_MainTex, uv + float2(0.0, halfpixel.y * 2.0));
        sum += tex2D(_MainTex, uv + float2(halfpixel.x, halfpixel.y)) * 2.0;
        sum += tex2D(_MainTex, uv + float2(halfpixel.x * 2.0, 0.0));
        sum += tex2D(_MainTex, uv + float2(halfpixel.x, -halfpixel.y)) * 2.0;
        sum += tex2D(_MainTex, uv + float2(0.0, -halfpixel.y * 2.0));
        sum += tex2D(_MainTex, uv + float2(-halfpixel.x, -halfpixel.y)) * 2.0;
        return sum / 12.0;
    }

    v2f vert (appdata v)
    {
        v2f o;
        o.vertex = UnityObjectToClipPos(v.vertex);
        o.uv = v.uv;
        return o;
    }
    ENDCG

    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        
        // 0 filter
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            fixed4 frag (v2f i) : SV_Target
            {
                half4 col = tex2D(_MainTex, i.uv);
                //half4 col = downsample2(i.uv, _MainTex_TexelSize.xy / 2.0);
                float filter = step(_ThroughValue, RgbMax(col)); // 超过_ThroughValue ? 1 : 0
                col *= filter;
                col.a = filter;
                return col;
            }

            ENDCG
        }

        // 1 downsample
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            fixed4 frag (v2f i) : SV_Target
            {
                return downsample(i.uv, _MainTex_TexelSize.xy / 2.0);
            }

            ENDCG
        }

        // 2 upsample
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            fixed4 frag (v2f i) : SV_Target
            {
                return upsample(i.uv, _MainTex_TexelSize.xy / 2.0);
            }

            ENDCG
        }
        

        // 3 add
        Pass
        {
            Blend One One

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag


            fixed4 frag (v2f i) : SV_Target
            {
                half4 col = tex2D(_MainTex, i.uv);
                return col;
            }

            ENDCG
        }
    }
}
