Shader "Custom/ToonShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Ramp ("Toon Ramp", 2D) = "white"{}
        _ShadowColor ("Shadow Color", Color) = (0.5, 0.5, 0.5, 1)
        _ShadowThreshold ("Shadow Threshold", Range(0,1)) = 0.5        
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline Width", Range(0, 0.2)) = 0.01
        _OrthoOutlineScale ("Ortho Scale", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "LightMode"="ForwardBase"}
        LOD 200
        
        Pass
        {
            Name "OUTLINE"
            Tags { "LightMode" = "Always" }
            Cull Front
            ZWrite On            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            
            struct v2f
            {
                float4 pos : SV_POSITION;                
            };
            
            float _OutlineWidth;
            float4 _OutlineColor;
            float _OrthoOutlineScale;
            
            v2f vert(appdata v)
            {
                v2f o;
                                
                float4 clipPos = UnityObjectToClipPos(v.vertex);
                                
                float isOrtho = unity_OrthoParams.w;
                
                if (isOrtho > 0.5)
                {                    
                    float3 normalVS = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal);
                    float2 offset = normalize(normalVS.xy) * _OutlineWidth * _OrthoOutlineScale;
                    clipPos.xy += offset;
                }
                else
                {                    
                    float3 normalOS = v.normal;
                    float3 outlineOffset = normalOS * _OutlineWidth;
                    v.vertex.xyz += outlineOffset;
                    clipPos = UnityObjectToClipPos(v.vertex);
                }
                
                o.pos = clipPos;
                return o;
            }
            
            fixed4 frag(v2f i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDCG
        }

        Pass
        {
            Name "TOON"
            Tags { "LightMode" = "ForwardBase"}
            Cull Back
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                SHADOW_COORDS(3)
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _Ramp;
            fixed4 _Color;
            fixed4 _ShadowColor;
            float _ShadowThreshold;            
            
            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                TRANSFER_SHADOW(o);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                float3 worldNormal = normalize(i.worldNormal);
                float3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
                
                float NdotL = dot(worldNormal, worldLightDir);
                float shadow = SHADOW_ATTENUATION(i);
                
                float isOrtho = unity_OrthoParams.w;
                float light = NdotL * shadow;
                
                if (isOrtho > 0.5)
                {
                    light = lerp(light, step(_ShadowThreshold, light), 0.7);
                }
                
                float toonLight = step(_ShadowThreshold, light);                
                float3 ramp = tex2D(_Ramp, float2(toonLight, 0.5)).rgb;
                
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;                
                float3 ambient = ShadeSH9(float4(worldNormal, 1));
                float3 lighting = lerp(_ShadowColor.rgb, _LightColor0.rgb, ramp);
                
                col.rgb *= (lighting + ambient);
                
                return col;
            }
            ENDCG
        }

        Pass
        {
            Name "ShadowCaster"
            Tags {"LightMode" = "ShadowCaster"}
            
            ZWrite On
            ZTest LEqual
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
            #include "UnityCG.cginc"
            
            struct v2f
            {
                V2F_SHADOW_CASTER;
            };
            
            v2f vert(appdata_base v)
            {
                v2f o;
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o);
                return o;
            }
            
            float4 frag(v2f i) : SV_Target
            {
                SHADOW_CASTER_FRAGMENT(i)
            }
            
            ENDCG
        }
    }
    FallBack "Diffuse"
}
