Shader "ToonShader1.5/Toon Basic Outline/Perspective/Basic Tint Alpha"
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
       
            Program "vp" {
// Vertex combos: 1
//   opengl - ALU: 4 to 4
//   d3d9 - ALU: 4 to 4
SubProgram "opengl " {
Keywords { }
Bind "vertex" Vertex
"!!ARBvp1.0
# 4 ALU
PARAM c[5] = { program.local[0],
		state.matrix.mvp };
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 4 instructions, 0 R-regs
"
}

SubProgram "d3d9 " {
Keywords { }
Bind "vertex" Vertex
Matrix 0 [glstate_matrix_mvp]
"vs_2_0
; 4 ALU
dcl_position0 v0
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}

SubProgram "gles " {
Keywords { }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;


attribute vec4 _glesVertex;
void main ()
{
  mediump vec4 tmpvar_1;
  highp vec4 tmpvar_2;
  tmpvar_2 = (gl_ModelViewProjectionMatrix * _glesVertex);
  tmpvar_1 = tmpvar_2;
  gl_Position = tmpvar_1;
}



#endif
#ifdef FRAGMENT

void main ()
{
  gl_FragData[0] = vec4(0.0, 0.0, 0.0, 0.0);
}



#endif"
}

SubProgram "glesdesktop " {
Keywords { }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;


attribute vec4 _glesVertex;
void main ()
{
  mediump vec4 tmpvar_1;
  highp vec4 tmpvar_2;
  tmpvar_2 = (gl_ModelViewProjectionMatrix * _glesVertex);
  tmpvar_1 = tmpvar_2;
  gl_Position = tmpvar_1;
}



#endif
#ifdef FRAGMENT

void main ()
{
  gl_FragData[0] = vec4(0.0, 0.0, 0.0, 0.0);
}



#endif"
}

}
Program "fp" {
// Fragment combos: 1
//   opengl - ALU: 1 to 1, TEX: 0 to 0
//   d3d9 - ALU: 2 to 2
SubProgram "opengl " {
Keywords { }
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 1 ALU, 0 TEX
PARAM c[1] = { { 0 } };
MOV result.color, c[0].x;
END
# 1 instructions, 0 R-regs
"
}

SubProgram "d3d9 " {
Keywords { }
"ps_2_0
; 2 ALU
def c0, 0.00000000, 0, 0, 0
mov_pp r0, c0.x
mov_pp oC0, r0
"
}

SubProgram "gles " {
Keywords { }
"!!GLES"
}

SubProgram "glesdesktop " {
Keywords { }
"!!GLES"
}

}

#LINE 49
   
        }
        
        Pass 
 		{
			//Name "BASE"
            Blend SrcAlpha OneMinusSrcAlpha 
				Program "vp" {
// Vertex combos: 1
//   opengl - ALU: 7 to 7
//   d3d9 - ALU: 7 to 7
SubProgram "opengl " {
Keywords { }
Bind "vertex" Vertex
Bind "normal" Normal
"!!ARBvp1.0
# 7 ALU
PARAM c[9] = { { 0.5 },
		state.matrix.mvp,
		state.matrix.modelview[0].invtrans };
TEMP R0;
DP3 R0.x, vertex.normal, c[5];
DP3 R0.y, vertex.normal, c[6];
MAD result.texcoord[2].xy, R0, c[0].x, c[0].x;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 7 instructions, 1 R-regs
"
}

SubProgram "d3d9 " {
Keywords { }
Bind "vertex" Vertex
Bind "normal" Normal
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [glstate_matrix_invtrans_modelview0]
"vs_2_0
; 7 ALU
def c8, 0.50000000, 0, 0, 0
dcl_position0 v0
dcl_normal0 v1
dp3 r0.x, v1, c4
dp3 r0.y, v1, c5
mad oT2.xy, r0, c8.x, c8.x
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}

SubProgram "gles " {
Keywords { }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;
#define gl_ModelViewMatrixInverseTranspose glstate_matrix_invtrans_modelview0
uniform mat4 glstate_matrix_invtrans_modelview0;
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying mediump vec2 xlv_TEXCOORD2;


attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  mediump vec4 tmpvar_1;
  mediump vec2 tmpvar_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (gl_ModelViewProjectionMatrix * _glesVertex);
  tmpvar_1 = tmpvar_3;
  mat3 tmpvar_4;
  tmpvar_4[0] = gl_ModelViewMatrixInverseTranspose[0].xyz;
  tmpvar_4[1] = gl_ModelViewMatrixInverseTranspose[1].xyz;
  tmpvar_4[2] = gl_ModelViewMatrixInverseTranspose[2].xyz;
  highp vec2 tmpvar_5;
  tmpvar_5 = (((tmpvar_4 * normalize (_glesNormal)).xy * 0.5) + 0.5);
  tmpvar_2 = tmpvar_5;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD2 = tmpvar_2;
}



#endif
#ifdef FRAGMENT

varying mediump vec2 xlv_TEXCOORD2;
uniform sampler2D _ToonShade;
uniform lowp vec4 _Color;
void main ()
{
  lowp vec4 tex;
  tex = ((texture2D (_ToonShade, xlv_TEXCOORD2) * _Color) * 2.0);
  tex.w = _Color.w;
  gl_FragData[0] = tex;
}



#endif"
}

SubProgram "glesdesktop " {
Keywords { }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;
#define gl_ModelViewMatrixInverseTranspose glstate_matrix_invtrans_modelview0
uniform mat4 glstate_matrix_invtrans_modelview0;
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying mediump vec2 xlv_TEXCOORD2;


attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  mediump vec4 tmpvar_1;
  mediump vec2 tmpvar_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (gl_ModelViewProjectionMatrix * _glesVertex);
  tmpvar_1 = tmpvar_3;
  mat3 tmpvar_4;
  tmpvar_4[0] = gl_ModelViewMatrixInverseTranspose[0].xyz;
  tmpvar_4[1] = gl_ModelViewMatrixInverseTranspose[1].xyz;
  tmpvar_4[2] = gl_ModelViewMatrixInverseTranspose[2].xyz;
  highp vec2 tmpvar_5;
  tmpvar_5 = (((tmpvar_4 * normalize (_glesNormal)).xy * 0.5) + 0.5);
  tmpvar_2 = tmpvar_5;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD2 = tmpvar_2;
}



#endif
#ifdef FRAGMENT

varying mediump vec2 xlv_TEXCOORD2;
uniform sampler2D _ToonShade;
uniform lowp vec4 _Color;
void main ()
{
  lowp vec4 tex;
  tex = ((texture2D (_ToonShade, xlv_TEXCOORD2) * _Color) * 2.0);
  tex.w = _Color.w;
  gl_FragData[0] = tex;
}



#endif"
}

}
Program "fp" {
// Fragment combos: 1
//   opengl - ALU: 4 to 4, TEX: 1 to 1
//   d3d9 - ALU: 4 to 4, TEX: 1 to 1
SubProgram "opengl " {
Keywords { }
Vector 0 [_Color]
SetTexture 0 [_ToonShade] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 4 ALU, 1 TEX
PARAM c[2] = { program.local[0],
		{ 2 } };
TEMP R0;
TEX R0.xyz, fragment.texcoord[2], texture[0], 2D;
MUL R0.xyz, R0, c[0];
MUL result.color.xyz, R0, c[1].x;
MOV result.color.w, c[0];
END
# 4 instructions, 1 R-regs
"
}

SubProgram "d3d9 " {
Keywords { }
Vector 0 [_Color]
SetTexture 0 [_ToonShade] 2D
"ps_2_0
; 4 ALU, 1 TEX
dcl_2d s0
def c1, 2.00000000, 0, 0, 0
dcl t2.xy
texld r0, t2, s0
mul r0.xyz, r0, c0
mul r0.xyz, r0, c1.x
mov_pp r0.w, c0
mov_pp oC0, r0
"
}

SubProgram "gles " {
Keywords { }
"!!GLES"
}

SubProgram "glesdesktop " {
Keywords { }
"!!GLES"
}

}

#LINE 92

            }
        
        Pass
        {
			Cull Front
            Blend SrcAlpha OneMinusSrcAlpha
            Program "vp" {
// Vertex combos: 1
//   opengl - ALU: 16 to 16
//   d3d9 - ALU: 16 to 16
SubProgram "opengl " {
Keywords { }
Bind "vertex" Vertex
Bind "normal" Normal
Float 13 [_Outline]
Vector 14 [_OutlineColor]
"!!ARBvp1.0
# 16 ALU
PARAM c[15] = { program.local[0],
		state.matrix.modelview[0],
		state.matrix.projection,
		state.matrix.mvp,
		program.local[13..14] };
TEMP R0;
TEMP R1;
DP3 R0.x, vertex.normal, c[1];
MUL R0.z, R0.x, c[5].x;
DP3 R0.y, vertex.normal, c[2];
DP4 R1.x, vertex.position, c[3];
MUL R0.w, R0.y, c[6].y;
DP4 R0.x, vertex.position, c[11];
MUL R0.zw, R0, R0.x;
MUL R0.zw, R0, c[13].x;
RCP R0.y, -R1.x;
MUL R1.xy, R0.zwzw, R0.y;
DP4 R0.y, vertex.position, c[12];
DP4 R0.w, vertex.position, c[10];
DP4 R0.z, vertex.position, c[9];
ADD result.position.xy, R0.zwzw, R1;
MOV result.color, c[14];
MOV result.position.zw, R0.xyxy;
END
# 16 instructions, 2 R-regs
"
}

SubProgram "d3d9 " {
Keywords { }
Bind "vertex" Vertex
Bind "normal" Normal
Matrix 0 [glstate_matrix_modelview0]
Matrix 4 [glstate_matrix_projection]
Matrix 8 [glstate_matrix_mvp]
Float 12 [_Outline]
Vector 13 [_OutlineColor]
"vs_2_0
; 16 ALU
dcl_position0 v0
dcl_normal0 v1
dp3 r0.x, v1, c0
mul r0.z, r0.x, c4.x
dp3 r0.y, v1, c1
dp4 r1.x, v0, c2
mul r0.w, r0.y, c5.y
dp4 r0.x, v0, c10
mul r0.zw, r0, r0.x
mul r0.zw, r0, c12.x
rcp r0.y, -r1.x
mul r1.xy, r0.zwzw, r0.y
dp4 r0.y, v0, c11
dp4 r0.w, v0, c9
dp4 r0.z, v0, c8
add oPos.xy, r0.zwzw, r1
mov oD0, c13
mov oPos.zw, r0.xyxy
"
}

SubProgram "gles " {
Keywords { }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ProjectionMatrix glstate_matrix_projection
uniform mat4 glstate_matrix_projection;
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying lowp vec4 xlv_COLOR;



uniform lowp vec4 _OutlineColor;
uniform lowp float _Outline;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  mediump vec4 tmpvar_1;
  highp vec4 tmpvar_2;
  tmpvar_2 = (gl_ModelViewProjectionMatrix * _glesVertex);
  tmpvar_1 = tmpvar_2;
  mat3 tmpvar_3;
  tmpvar_3[0] = gl_ModelViewMatrix[0].xyz;
  tmpvar_3[1] = gl_ModelViewMatrix[1].xyz;
  tmpvar_3[2] = gl_ModelViewMatrix[2].xyz;
  highp vec3 tmpvar_4;
  tmpvar_4 = (tmpvar_3 * normalize (_glesNormal));
  highp vec2 tmpvar_5;
  tmpvar_5.x = (tmpvar_4.x * gl_ProjectionMatrix[0].x);
  tmpvar_5.y = (tmpvar_4.y * gl_ProjectionMatrix[1].y);
  highp vec2 tmpvar_6;
  tmpvar_6 = (tmpvar_1.xy + (((tmpvar_5 * tmpvar_1.z) * _Outline) / -((gl_ModelViewMatrix * _glesVertex).z)));
  tmpvar_1.xy = tmpvar_6;
  gl_Position = tmpvar_1;
  xlv_COLOR = _OutlineColor;
}



#endif
#ifdef FRAGMENT

varying lowp vec4 xlv_COLOR;
void main ()
{
  gl_FragData[0] = xlv_COLOR;
}



#endif"
}

SubProgram "glesdesktop " {
Keywords { }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ProjectionMatrix glstate_matrix_projection
uniform mat4 glstate_matrix_projection;
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying lowp vec4 xlv_COLOR;



uniform lowp vec4 _OutlineColor;
uniform lowp float _Outline;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  mediump vec4 tmpvar_1;
  highp vec4 tmpvar_2;
  tmpvar_2 = (gl_ModelViewProjectionMatrix * _glesVertex);
  tmpvar_1 = tmpvar_2;
  mat3 tmpvar_3;
  tmpvar_3[0] = gl_ModelViewMatrix[0].xyz;
  tmpvar_3[1] = gl_ModelViewMatrix[1].xyz;
  tmpvar_3[2] = gl_ModelViewMatrix[2].xyz;
  highp vec3 tmpvar_4;
  tmpvar_4 = (tmpvar_3 * normalize (_glesNormal));
  highp vec2 tmpvar_5;
  tmpvar_5.x = (tmpvar_4.x * gl_ProjectionMatrix[0].x);
  tmpvar_5.y = (tmpvar_4.y * gl_ProjectionMatrix[1].y);
  highp vec2 tmpvar_6;
  tmpvar_6 = (tmpvar_1.xy + (((tmpvar_5 * tmpvar_1.z) * _Outline) / -((gl_ModelViewMatrix * _glesVertex).z)));
  tmpvar_1.xy = tmpvar_6;
  gl_Position = tmpvar_1;
  xlv_COLOR = _OutlineColor;
}



#endif
#ifdef FRAGMENT

varying lowp vec4 xlv_COLOR;
void main ()
{
  gl_FragData[0] = xlv_COLOR;
}



#endif"
}

}
Program "fp" {
// Fragment combos: 1
//   opengl - ALU: 1 to 1, TEX: 0 to 0
//   d3d9 - ALU: 1 to 1
SubProgram "opengl " {
Keywords { }
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 1 ALU, 0 TEX
MOV result.color, fragment.color.primary;
END
# 1 instructions, 0 R-regs
"
}

SubProgram "d3d9 " {
Keywords { }
"ps_2_0
; 1 ALU
dcl v0
mov_pp oC0, v0
"
}

SubProgram "gles " {
Keywords { }
"!!GLES"
}

SubProgram "glesdesktop " {
Keywords { }
"!!GLES"
}

}

#LINE 134

        }
    }
    Fallback "Transparent/Diffuse"
}