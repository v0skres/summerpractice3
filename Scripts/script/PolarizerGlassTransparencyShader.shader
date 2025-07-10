Shader "Custom/PolarizerGlassTransparencyShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1) // �������� ���� ������
        _MainTex ("Texture", 2D) = "white" {} // ������������ ��������
        _PolarizerOpacity ("Polarizer Opacity", Range(0, 1)) = 0 // ��� �������� ��� ���������� �������������
        _Shininess ("Shininess", Range (0.03, 1)) = 0.078 // �����, ��� �������� ������
    }
    SubShader
    {
        // ��� ���� ����� ����� ��� ���������� ��������� ������������
        // "Transparent" RenderType ��������, ��� ������ ������������ ������������.
        // "Queue"="Transparent" ���������� Unity ��������� ���� ������ ����� ������������,
        // ��� �������� �������� ������� � ����������.
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        // ����������� �������� (����������)
        // SrcAlpha OneMinusSrcAlpha - ����������� �������� ��� ������������,
        // ��� ���� ��������� ����������� � ���, ��� ��� ��������� � ������ �����,
        // ��������� �����-����� ���������.
        Blend SrcAlpha OneMinusSrcAlpha
        // ��������� ������ � Z-����� ��� ���������� ��������. ��� �������������
        // ��������, ����� ����� ��������� ���������� ������ "���������" �����
        // ����� ������� ������� ��� ���������, ������� ������ ���� ����� ������ ����.
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            fixed4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _PolarizerOpacity; // ���� �������� ������������ �� �������
            float _Shininess; // ��������� ���������� ��� ������

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // �������� ������� ���� �� �������� � �������� _Color
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;

                // ������������� �����-�����
                // _PolarizerOpacity = 0 (����������) -> col.a = 1 (������������)
                // _PolarizerOpacity = 1 (������������) -> col.a = 0 (����������)
                // �� �����, ����� opacity=0 ������ ��� ����������, opacity=1 - ������������.
                // ������� ���������� (1.0 - _PolarizerOpacity) ��� ��������.
                // ���, ���� ��� ������ ��� ������ 0 ��� ������������ � 1 ��� ��������������,
                // ������ ����������� col.a = _PolarizerOpacity;
                col.a = _PolarizerOpacity; 

                // �� ������ �������� ���� ������� �������� ������ ��� ������,
                // ����� ������� ������ ����� ������������.
                // ��� ����� ������� ������, ��� ��������� ���������:
                // fixed3 finalColor = col.rgb;
                // float spec = pow(saturate(dot(normalize(i.normal), normalize(_WorldSpaceLightPos0.xyz))), _Shininess * 128);
                // finalColor += spec * fixed3(1,1,1); // ����� ����
                // col.rgb = finalColor;

                // ��������� �����
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}