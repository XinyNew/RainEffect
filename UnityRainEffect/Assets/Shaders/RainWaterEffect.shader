Shader "Custom/RainWaterEffect"
{
    Properties
    {
        _WaterMap ("Water Map", 2D) = "white" {}
        _TextureShine ("Texture Shine", 2D) = "white" {}
        _TextureFg ("Texture Foreground", 2D) = "white" {}
        _TextureBg ("Texture Background", 2D) = "white" {}
        _ParallaxBg ("Parallax Background", Float) = 5.0
        _ParallaxFg ("Parallax Foreground", Float) = 20.0
        _ParallaxX ("Parallax X", Float) = 0.0
        _ParallaxY ("Parallax Y", Float) = 0.0
        _TextureRatio ("Texture Ratio", Float) = 1.333
        _MinRefraction ("Min Refraction", Float) = 256.0
        _RefractionDelta ("Refraction Delta", Float) = 256.0
        _Brightness ("Brightness", Float) = 1.0
        _AlphaMultiply ("Alpha Multiply", Float) = 20.0
        _AlphaSubtract ("Alpha Subtract", Float) = 5.0
        [Toggle] _RenderShine ("Render Shine", Float) = 0
        [Toggle] _RenderShadow ("Render Shadow", Float) = 0
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 100
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile __ _RENDERSHINE_ON
            #pragma multi_compile __ _RENDERSHADOW_ON
            
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
                float4 screenPos : TEXCOORD1;
            };
            
            sampler2D _WaterMap;
            sampler2D _TextureShine;
            sampler2D _TextureFg;
            sampler2D _TextureBg;
            float4 _WaterMap_ST;
            
            float2 _Resolution;
            float2 _Parallax;
            float _ParallaxFg;
            float _ParallaxBg;
            float _ParallaxX;
            float _ParallaxY;
            float _TextureRatio;
            float _MinRefraction;
            float _RefractionDelta;
            float _Brightness;
            float _AlphaMultiply;
            float _AlphaSubtract;
            float _RenderShine;
            float _RenderShadow;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.screenPos = ComputeScreenPos(o.vertex);
                return o;
            }
            
            // Alpha-blends two colors
            float4 blend(float4 bg, float4 fg)
            {
                float3 bgm = bg.rgb * bg.a;
                float3 fgm = fg.rgb * fg.a;
                float ia = 1.0 - fg.a;
                float a = (fg.a + bg.a * ia);
                float3 rgb;
                if (a != 0.0)
                {
                    rgb = (fgm + bgm * ia) / a;
                }
                else
                {
                    rgb = float3(0.0, 0.0, 0.0);
                }
                return float4(rgb, a);
            }
            
            float2 pixel()
            {
                return float2(1.0, 1.0) / _Resolution;
            }
            
            float2 parallax(float v)
            {
                return float2(_ParallaxX, _ParallaxY) * pixel() * v;
            }
            
            float2 texCoord(float2 screenPos)
            {
                return float2(screenPos.x, _Resolution.y - screenPos.y) / _Resolution;
            }
            
            // Scales the bg up and proportionally to fill the container
            float2 scaledTexCoord(float2 tc)
            {
                float ratio = _Resolution.x / _Resolution.y;
                float2 scale = float2(1.0, 1.0);
                float2 offset = float2(0.0, 0.0);
                float ratioDelta = ratio - _TextureRatio;
                if (ratioDelta >= 0.0)
                {
                    scale.y = (1.0 + ratioDelta);
                    offset.y = ratioDelta / 2.0;
                }
                else
                {
                    scale.x = (1.0 - ratioDelta);
                    offset.x = -ratioDelta / 2.0;
                }
                return (tc + offset) / scale;
            }
            
            // Get color from fg
            float4 fgColor(float2 tc, float x, float y)
            {
                float p2 = _ParallaxFg * 2.0;
                float2 scale = float2(
                    (_Resolution.x + p2) / _Resolution.x,
                    (_Resolution.y + p2) / _Resolution.y
                );
                
                float2 scaledTC = tc / scale;
                float2 offset = float2(
                    (1.0 - (1.0 / scale.x)) / 2.0,
                    (1.0 - (1.0 / scale.y)) / 2.0
                );
                
                return tex2D(_WaterMap, (scaledTC + offset) + (pixel() * float2(x, y)) + parallax(_ParallaxFg));
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // Get resolution dynamically
                _Resolution = _ScreenParams.xy;
                
                float2 screenUV = i.screenPos.xy / i.screenPos.w;
                float2 tc = float2(screenUV.x, 1.0 - screenUV.y);
                
                float4 bg = tex2D(_TextureBg, scaledTexCoord(tc) + parallax(_ParallaxBg));
                
                float4 cur = fgColor(tc, 0.0, 0.0);
                
                float d = cur.b; // "thickness"
                float x = cur.g;
                float y = cur.r;
                
                float a = clamp(cur.a * _AlphaMultiply - _AlphaSubtract, 0.0, 1.0);
                
                float2 refraction = (float2(x, y) - 0.5) * 2.0;
                float2 refractionParallax = parallax(_ParallaxBg - _ParallaxFg);
                float2 refractionPos = scaledTexCoord(tc)
                    + (pixel() * refraction * (_MinRefraction + (d * _RefractionDelta)))
                    + refractionParallax;
                
                float4 tex = tex2D(_TextureFg, refractionPos);
                
                #ifdef _RENDERSHINE_ON
                if (_RenderShine > 0.5)
                {
                    float maxShine = 490.0;
                    float minShine = maxShine * 0.18;
                    float2 shinePos = float2(0.5, 0.5) + ((1.0 / 512.0) * refraction) * -(minShine + ((maxShine - minShine) * d));
                    float4 shine = tex2D(_TextureShine, shinePos);
                    tex = blend(tex, shine);
                }
                #endif
                
                float4 fg = float4(tex.rgb * _Brightness, a);
                
                #ifdef _RENDERSHADOW_ON
                if (_RenderShadow > 0.5)
                {
                    float borderAlpha = fgColor(tc, 0.0, 0.0 - (d * 6.0)).a;
                    borderAlpha = borderAlpha * _AlphaMultiply - (_AlphaSubtract + 0.5);
                    borderAlpha = clamp(borderAlpha, 0.0, 1.0);
                    borderAlpha *= 0.2;
                    float4 border = float4(0.0, 0.0, 0.0, borderAlpha);
                    fg = blend(border, fg);
                }
                #endif
                
                return blend(bg, fg);
            }
            ENDCG
        }
    }
}
