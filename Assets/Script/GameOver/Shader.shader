Shader "Unlit/Shader"
{
    Properties
    {
        [PerRendererData]_MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,0,0,1)
        _Progress ("Progress", Range(0,1)) = 0
        _Softness ("Softness", Range(0.01,1)) = 0.3
        _StartEdge ("Start Edge", Range(0.5,1.5)) = 1.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            fixed4 _Color;
            float _Progress;
            float _Softness;
            float _StartEdge;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //中心(0.5,0.5)から距離。中心=0、四隅=1になるよう正規化
                float2 d = (i.uv - 0.5) *  2.0f;
                float dist = length(d) / 1.4142;

                //Progress 0 のときは境目が四隅の外、1のときは中心の内側にある
                float edge = lerp(_StartEdge + _Softness, -_Softness, _Progress);

                //境目より外側を赤くする(Softnessの幅でぼかす)
                float a = smoothstep(edge, edge + _Softness, dist);

                return fixed4(_Color.rgb,a * _Color.a);
            }
            ENDCG
        }
    }
}
