Shader "Custom/StackShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _StackHeight ("Stack Height", Float) = 0.1
        _LayerCount ("Layer Count", Float) = 10
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            float _StackHeight;
            float _LayerCount;

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
                
                // 층마다 일정 간격으로 정점 이동
                float layer = floor(v.vertex.y * _LayerCount);
                v.vertex.y += layer * _StackHeight / _LayerCount;
                
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }
}
