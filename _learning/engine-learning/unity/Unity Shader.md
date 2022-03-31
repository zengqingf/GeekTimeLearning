### Unity中的数学

* 矩阵

  * ref

    [Unity Shader Matrix[Matrix]](https://blog.csdn.net/cubesky/article/details/38664143)

    [UNITY_MATRIX_IT_MV[Matrix]](https://blog.csdn.net/cubesky/article/details/38682975)



---



### Shader效果

* 内外发光（描边）

  ``` c
  //ref: https://blog.csdn.net/qq_38721111/article/details/89469827
  //边缘内发光
  //原理：用视角方向和法线方向点乘，模型越边缘的地方，它的法线和视角方向越接近90度。用1减去上面点乘的结果得到的就是越边缘的地方越亮。
  Shader "Custom/Outline1" {
  	Properties
  	{
  		_MainTex("main tex",2D) = "black"{}
  		_RimColor("rim color",Color) = (1,1,1,1)//边缘颜色
  		_RimPower("rim power",range(1,10)) = 2//边缘强度
  	}
   
  		SubShader
  	{
  		Pass
  	{
  		CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include"UnityCG.cginc"
   
  	struct v2f
  	{
  		float4 vertex:POSITION;
  		float4 uv:TEXCOORD0;
  		float4 NdotV:COLOR;
  	};
   
  	sampler2D _MainTex;
  	float4 _RimColor;
  	float _RimPower;
   
  	v2f vert(appdata_base v)
  	{
  		v2f o;
  		o.vertex = UnityObjectToClipPos(v.vertex);
  		o.uv = v.texcoord;
  		float3 V = WorldSpaceViewDir(v.vertex);
  		V = mul(unity_WorldToObject,V);//视方向从世界到模型坐标系的转换
  		o.NdotV.x = saturate(dot(v.normal,normalize(V)));//必须在同一坐标系才能正确做点乘运算
  		return o;
  	}
   
  	half4 frag(v2f IN) :COLOR
  	{
  		half4 c = tex2D(_MainTex,IN.uv);
  		//用视方向和法线方向做点乘，越边缘的地方，法线和视方向越接近90度，点乘越接近0.
  		//用（1- 上面点乘的结果）*颜色，来反映边缘颜色情况
  		c.rgb += pow((1 - IN.NdotV.x) ,_RimPower)* _RimColor.rgb;
  		return c;
  	}
  		ENDCG
  	}
  	}
  		FallBack "Diffuse"
  }
  ```

  ``` c
  //ref: https://blog.csdn.net/qq_38721111/article/details/89469827
  //边缘外发光
  //原理：让物体的顶点位置沿着法线方向延伸，然后使用法线点乘物体法线的方向形成一个中间向四周扩散逐渐衰减的效果。
  Shader "Custom/OutLine2" {
   
  	Properties
  	{
  		_MainTex("Texture(RGB)",2D) = "grey"{}	//主纹理
  		_Color("Color",Color) = (0,0,0,1)		//主纹理颜色
  		_AtmoColor("Atmosphere Color",Color) = (0,0,0,0)	//光晕颜色
  		_Size("Size",Range(0,1)) = 0.1		//光晕范围
  		_OutLightPow("Falloff",Range(1,10)) = 5		//光晕系数
  		_OutLightStrength("Transparency",Range(5,20)) = 15	//光晕强度
  	}
   
  		SubShader{
  			Pass{
  				Name "PlaneBase"
  				Tags{"LightMode" = "Always"}
  				Cull Back		//剔除背面
  				CGPROGRAM
  	#pragma vertex vert
  	#pragma fragment frag
  	#include "UnityCG.cginc"
   
  		uniform sampler2D _MainTex;
  		uniform float4 _MainTex_ST;
  		uniform float4 _Color;
  		uniform float4 _AtmoColor;
  		uniform float _Size;
  		uniform float _OutLightPow;
  		uniform float _OutLightStrength;
   
  		struct vertexOutput {
  			float4 pos : SV_POSITION;
  			float3 normal : TEXCOORD0;
  			float3 worldvertpos : TEXCOORD1;
  			float2 texcoord : TEXCOORD2;
  		};
   
  		//顶点着色器
  		vertexOutput vert(appdata_base v)
  		{
  			vertexOutput o;
  			//顶点位置
  			o.pos = UnityObjectToClipPos(v.vertex);
  			//法线
  			o.normal = v.normal;
  			//世界坐标顶点位置
  			o.worldvertpos = mul(unity_ObjectToWorld, v.vertex).xyz;
  			o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
  			return o;
  		}
   
  		float4 frag(vertexOutput i) : COLOR
  		{
  			float4 color = tex2D(_MainTex,i.texcoord);
  			return color * _Color;
  		}
  			ENDCG
  		}
   
  		Pass{
  			Name "AtmosphereBase"
  			Tags{"LightMode" = "Always"}
  			Cull Front
  			Blend SrcAlpha One
   
  			CGPROGRAM
  	#pragma vertex vert
  	#pragma fragment frag
  	#include "UnityCG.cginc"
   
  		uniform float4 _Color;
  		uniform float4 _AtmoColor;
  		uniform float _Size;
  		uniform float _OutLightPow;
  		uniform float _OutLightStrength;
   
  		struct vertexOutput
  		{
  			float4 pos : SV_POSITION;
  			float3 normal : TEXCOORD0;
  			float3 worldvertpos : TEXCOORD1;
  		};
   
  		vertexOutput vert(appdata_base v) {
  			vertexOutput o;
  			//顶点位置以法线方向向外延伸
  			v.vertex.xyz += v.normal * _Size;
  			o.pos = UnityObjectToClipPos(v.vertex);
  			o.normal = v.normal;
  			o.worldvertpos = mul(unity_ObjectToWorld, v.vertex);
  			return o;
  		}
   
  		float4 frag(vertexOutput i) : COLOR
  		{
  			i.normal = normalize(i.normal);
  			//视角法线
  			float3 viewdir = normalize(i.worldvertpos.xyz - _WorldSpaceCameraPos.xyz);
  			float4 color = _AtmoColor;
  			//视角法线与模型法线点积形成中间为1向四周逐渐衰减为0的点积值，赋值透明通道，形成光晕效果
  			color.a = pow(saturate(dot(viewdir, i.normal)),_OutLightPow);
  			color.a *= _OutLightStrength * dot(viewdir, i.normal);
  			return color;
  		}
  		ENDCG
  	}
   
  		}
  			FallBack "Diffuse"
  }
  ```

  