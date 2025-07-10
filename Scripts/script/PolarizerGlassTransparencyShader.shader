Shader "Custom/PolarizerGlassTransparencyShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1) // Основной цвет стекла
        _MainTex ("Texture", 2D) = "white" {} // Опциональная текстура
        _PolarizerOpacity ("Polarizer Opacity", Range(0, 1)) = 0 // Наш параметр для управления прозрачностью
        _Shininess ("Shininess", Range (0.03, 1)) = 0.078 // Блеск, для имитации стекла
    }
    SubShader
    {
        // Эти теги очень важны для правильной обработки прозрачности
        // "Transparent" RenderType означает, что шейдер поддерживает прозрачность.
        // "Queue"="Transparent" заставляет Unity рендерить этот объект после непрозрачных,
        // что помогает избежать проблем с отрисовкой.
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        // Настраиваем блендинг (смешивание)
        // SrcAlpha OneMinusSrcAlpha - стандартный блендинг для прозрачности,
        // где цвет фрагмента смешивается с тем, что уже находится в буфере цвета,
        // используя альфа-канал фрагмента.
        Blend SrcAlpha OneMinusSrcAlpha
        // Отключаем запись в Z-буфер для прозрачных объектов. Это предотвращает
        // ситуации, когда более удаленный прозрачный объект "закрывает" собой
        // более близкие объекты при отрисовке, которые должны быть видны сквозь него.
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
            float _PolarizerOpacity; // Наше свойство прозрачности из скрипта
            float _Shininess; // Добавляем переменную для блеска

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
                // Получаем базовый цвет из текстуры и свойства _Color
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;

                // Устанавливаем альфа-канал
                // _PolarizerOpacity = 0 (прозрачный) -> col.a = 1 (непрозрачный)
                // _PolarizerOpacity = 1 (непрозрачный) -> col.a = 0 (прозрачный)
                // Мы хотим, чтобы opacity=0 делало его прозрачным, opacity=1 - непрозрачным.
                // Поэтому используем (1.0 - _PolarizerOpacity) для инверсии.
                // Или, если ваш скрипт уже выдает 0 для прозрачности и 1 для непрозрачности,
                // просто используйте col.a = _PolarizerOpacity;
                col.a = _PolarizerOpacity; 

                // Вы можете добавить сюда простую имитацию блеска или бликов,
                // чтобы сделать стекло более реалистичным.
                // Это очень простой пример, без реального освещения:
                // fixed3 finalColor = col.rgb;
                // float spec = pow(saturate(dot(normalize(i.normal), normalize(_WorldSpaceLightPos0.xyz))), _Shininess * 128);
                // finalColor += spec * fixed3(1,1,1); // Белый блик
                // col.rgb = finalColor;

                // Применяем туман
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}