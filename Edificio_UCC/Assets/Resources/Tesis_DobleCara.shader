// Material del edificio visible por las dos caras.
// El modelo de SketchUp trae caras de un solo lado: desde dentro de un salón o un pasillo,
// algunos muros, techos y pisos no se dibujan y se "ve a través" de ellos.
// Este shader dibuja ambas caras (Cull Off) en una sola pasada e invierte la normal en la
// cara trasera para que la iluminación sea correcta. Luz: ambiente (SH) + luz direccional + niebla.
// Compatible con Single Pass Instanced (Quest).
Shader "Tesis/DobleCara"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Textura", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 100
        Cull Off

        Pass
        {
            Tags { "LightMode"="ForwardBase" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 nrm : TEXCOORD1;
                UNITY_FOG_COORDS(2)
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.nrm = UnityObjectToWorldNormal(v.normal);
                UNITY_TRANSFER_FOG(o, o.pos);
                return o;
            }

            fixed4 frag (v2f i, fixed cara : VFACE) : SV_Target
            {
                float3 n = normalize(i.nrm) * (cara > 0 ? 1 : -1);
                fixed4 c = tex2D(_MainTex, i.uv) * _Color;
                float difusa = saturate(dot(n, _WorldSpaceLightPos0.xyz));
                fixed3 luz = ShadeSH9(float4(n, 1)) + _LightColor0.rgb * difusa;
                fixed4 col = fixed4(c.rgb * luz, 1);
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
