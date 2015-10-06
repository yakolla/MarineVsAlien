Shader "ToonShader1.5/Toon Basic Outline/Ortho/Basic Tint Alpha"
{
    Properties
    {
        _Color ("Base Color", Color) = (0.5,0.5,0.5,1.0)
        _OutlineColor ("Outline Color", Color) = (0.5,0.5,0.5,1.0)
        _Outline ("Outline width", Float) = 0.01
        _ToonShade ("Shading Texture (RGB)", 2D) = "gray" {}
    }
 
    SubShader
    {
       
        
        Tags {"RenderType"="Transparent" "Queue"="Transparent" "IgnoreProjector"="True"}
        Lighting Off
        Fog { Mode Off }
        LOD 200
       
       	Pass {
        	//Name "TrBASE"
            ZWrite On
            ColorMask 0
       
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma exclude_renderers xbox360
            #pragma exclude_renderers flash
            #pragma fragmentoption ARB_precision_hint_fastest
            #include "UnityCG.cginc"
     
            struct v2f {
                half4 pos : SV_POSITION;
            };
     
            v2f vert (appdata_base v)
            {
                v2f o;
                o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
                return o;
            }
     
            fixed4 frag (v2f i) : COLOR
            {
                return half4 (0);
            }
            ENDCG   
        }
        
        Pass 
 		{
			//Name "BASE"
            Blend SrcAlpha OneMinusSrcAlpha 
				CGPROGRAM
            	
                #pragma vertex vert
                #pragma fragment frag
                #pragma exclude_renderers xbox360
				#pragma exclude_renderers flash
                #pragma fragmentoption ARB_precision_hint_fastest
                #include "UnityCG.cginc"
                
                struct v2f 
                {
                    half4 pos : SV_POSITION;
                    half2 uvn : TEXCOORD2;

                };
               
                v2f vert (appdata_base v)
                {
                    v2f o;
                    o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
                   // o.uvn = (COMPUTE_VIEW_NORMAL.xy * 0.5) + 0.5;
                   o.uvn = (mul((half3x3)UNITY_MATRIX_IT_MV, v.normal).xy * 0.5) + 0.5;
                    return o;
                }

                sampler2D _ToonShade;
                fixed4 _Color;
                
                fixed4 frag (v2f i) : COLOR
                {
                   
                   fixed4 tex = tex2D(_ToonShade, i.uvn)*_Color*2.0;
                   tex.a = _Color.a;
                   return tex;
                }
                
            ENDCG
            }
        
        Pass
        {
			Cull Front
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
			#include "UnityCG.cginc"
			#pragma exclude_renderers xbox360
			#pragma exclude_renderers flash
			#pragma fragmentoption ARB_precision_hint_fastest
            #pragma vertex vert
 			#pragma fragment frag
	

            struct v2f 
            {
                half4 pos : POSITION;
                fixed4 color : COLOR;
            };
            
            uniform fixed _Outline;
            uniform fixed4 _OutlineColor;
 
            v2f vert(appdata_base v) 
            {

                v2f o;
			    o.pos = v.vertex;
			    o.pos.xyz += v.normal.xyz *_Outline;
			    o.pos = mul(UNITY_MATRIX_MVP, o.pos);
			    o.color = _OutlineColor;
			    return o;
            }
            
            fixed4 frag(v2f i) :COLOR 
			{
		    	return i.color;
			}
            
            ENDCG
        }
    }
    Fallback "Transparent/Diffuse"
}