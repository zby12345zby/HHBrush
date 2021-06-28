
Shader "Unlit/Shaer"
{
    Properties{
    _Color("Main Tint", Color) = (1, 1, 1, 1)
        _MainTex("Texture", 2D) = "white" {}
    _OutlineWidth("OutlineWidth", Range(0, 1)) = 0.1
        _OutlineColor("OutlineColor", Color) = (1, 1, 1, 1)
        _AlphaBlend("AlphaBlend", Range(0, 1)) = 0.8  // 用于在透明纹理的基础上控制整体的透明度
        _AlphaTest("AlphaTest", Range(0, 1)) = 0.5
    }
        SubShader
    {
        // RenderType标签可以让Unity把这个Shader归入到提前定义的组(Transparent)用于指明该Shader是一个使用了透明度混合的Shader
        // IgnoreProjector=True这意味着该Shader不会受到投影器(Projectors)的影响
        // 为了使用透明度混合的Shader一般都应该在SubShader中设置这三个标签
        Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }


        //第一个Pass，开启深度写入，不输出颜色，处理AlphaTest部分
        Pass{
            // 仅仅是把模型的深度信息写入深度缓冲中从而剔除模型中被自身遮挡的片元


            ZWrite On


            // 用于设置颜色通道的写掩码(Wirte Mask)ColorMask RGB|A|0|(R/G/B/A组合)
            // 当为0时意味着该Pass不写入任何颜色通道，也就不会输出任何颜色
            //也可以不用，这样就能刻意控制AlphaTest导致的发丝边缘的颜色
            ColorMask 0


            CGPROGRAM


            #pragma vertex vert  
            #pragma fragment frag  
            #include "Lighting.cginc"  


            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed _AlphaTest;


            struct a2v {
            float4 vertex : POSITION;
            float4 texcoord : TEXCOORD0;
            };


            struct v2f {
            float4 pos : SV_POSITION;
            float2 uv : TEXCOORD2;
            };


            v2f vert(a2v v) {
            v2f o;
            o.pos = UnityObjectToClipPos(v.vertex);
            o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
            return o;
            }


            fixed4 frag(v2f i) : SV_Target{
            fixed4 texColor = tex2D(_MainTex, i.uv);

            clip(texColor.a - _AlphaTest); //进行AlphaTest     //clip函数，但参数为负数则舍弃该片元输出

            return fixed4(0,0,0,1);
            }
            ENDCG
            }
        //第二个Pass，关闭深度写入，处理AlphaBlend半透明，处理光照等部分
        Pass{
                // 向前渲染路径的方式
                Tags{ "LightMode" = "ForwardBase" }

                Cull Off //关闭剔除，让头发双面显示

                ZWrite Off //关闭深度写入  
                //透明度混合需要关闭深度写入    
                //Orgb = SrcAlpha * Srgb + OneMinusSrcAlpha * Drgb
                //Oa = SrcAlpha * Sa + OneMinusSrcAlpha * Da
                //将本Shader计算出的颜色值(源颜色值) * 源Alpha值 + 目标颜色值(可以理解为背景色) * (1-源Alpha值)，从而让源物体展示出了(1-alpha)的透明度。
                Blend SrcAlpha OneMinusSrcAlpha

                CGPROGRAM

                #pragma vertex vert  
                #pragma fragment frag  
                #include "Lighting.cginc"  

                fixed4  _Color;
                sampler2D _MainTex;
                float4 _MainTex_ST;
                fixed _AlphaBlend;

                struct a2v {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
                };

                struct v2f {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float2 uv : TEXCOORD2;
                };

                v2f vert(a2v v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
                }

                fixed4 frag(v2f i) : SV_Target{
                fixed3 worldNormal = normalize(i.worldNormal);
                fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
                fixed4 texColor = tex2D(_MainTex, i.uv);
                fixed3 albedo = texColor.rgb * _Color;
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
                fixed3 diffuse = _LightColor0.rgb * albedo * max(0, dot(worldNormal, worldLightDir));
                // 设置透明度通道的值
                return fixed4(ambient + diffuse, texColor.a * _AlphaBlend);
                }
                ENDCG
                }
                //第三个Pass，处理描边
                Pass{
                Cull Front //剔除正面 

                CGPROGRAM
                #pragma vertex vert  
                #pragma fragment frag         
                #include "UnityCG.cginc"  


                sampler2D _MainTex;
                float4 _MainTex_ST;
                fixed _AlphaTest;
                float _OutlineWidth;
                float4 _OutlineColor;

                struct a2v
                {
                float4 vertex : POSITION;
                float4 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
                };

                struct v2f
                {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD2;
                };
                v2f vert(a2v v)
                {
                v2f o;
                //o.pos = mul(UNITY_MATRIX_MVP, v.vertex);    
                //float3 normal = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal); 
                //float2 offset = TransformViewToProjection(normal.xy);
                //o.pos.xy += offset *o.pos.z * _OutlineWidth;

                //方法二：
                //o.pos = UnityObjectToClipPos(v.vertex);
                //float3 normal = UnityObjectToWorldNormal(v.normal);
                //normal = mul(UNITY_MATRIX_VP, normal);
                //o.pos.xyz += normal * _Outline;
                float4 pos = mul(UNITY_MATRIX_MV, v.vertex);   // viewPos,顶点变换到视角空间
                float3 normal = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal);  // viewNormal，UNITY_MATRIX_IT_MV这个针对方向向量，法线变换
                normal.z = -0.5;   // viewNormal 对顶点法线的z分量进行处理，使他们等于一个定值
                float4 newNoraml = float4(normalize(normal), 0); //归一化后的noraml
                pos = pos + newNoraml * _OutlineWidth;  // viewPos 沿法线方向对顶点进行扩大
                o.pos = mul(UNITY_MATRIX_P, pos);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
                }


                fixed4 frag(v2f i) : SV_Target
                {
                fixed4 texColor = tex2D(_MainTex, i.uv);
                //AlphaTest为了让描边也能根据透贴部分来勾边,也可以不加
                clip(texColor.a - _AlphaTest);

                return _OutlineColor;
                }
                ENDCG
                }
    }
        FallBack "Diffuse"
}
