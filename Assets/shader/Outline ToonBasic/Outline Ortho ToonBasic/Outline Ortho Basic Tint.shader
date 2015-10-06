Shader "ToonShader1.5/Toon Basic Outline/Ortho/Basic Tint"
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
              	    
        Pass 
 		{
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
  gl_FragData[0] = ((texture2D (_ToonShade, xlv_TEXCOORD2) * _Color) * 2.0);
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
  gl_FragData[0] = ((texture2D (_ToonShade, xlv_TEXCOORD2) * _Color) * 2.0);
}



#endif"
}

}
Program "fp" {
// Fragment combos: 1
//   opengl - ALU: 3 to 3, TEX: 1 to 1
//   d3d9 - ALU: 3 to 3, TEX: 1 to 1
SubProgram "opengl " {
Keywords { }
Vector 0 [_Color]
SetTexture 0 [_ToonShade] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 3 ALU, 1 TEX
PARAM c[2] = { program.local[0],
		{ 2 } };
TEMP R0;
TEX R0, fragment.texcoord[2], texture[0], 2D;
MUL R0, R0, c[0];
MUL result.color, R0, c[1].x;
END
# 3 instructions, 1 R-regs
"
}

SubProgram "d3d9 " {
Keywords { }
Vector 0 [_Color]
SetTexture 0 [_ToonShade] 2D
"ps_2_0
; 3 ALU, 1 TEX
dcl_2d s0
def c1, 2.00000000, 0, 0, 0
dcl t2.xy
texld r0, t2, s0
mul r0, r0, c0
mul r0, r0, c1.x
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

#LINE 52

            }
        
        Pass
        {
			Cull Front
            Blend SrcAlpha OneMinusSrcAlpha
            Program "vp" {
// Vertex combos: 1
//   opengl - ALU: 8 to 8
//   d3d9 - ALU: 8 to 8
SubProgram "opengl " {
Keywords { }
Bind "vertex" Vertex
Bind "normal" Normal
Float 5 [_Outline]
Vector 6 [_OutlineColor]
"!!ARBvp1.0
# 8 ALU
PARAM c[7] = { program.local[0],
		state.matrix.mvp,
		program.local[5..6] };
TEMP R0;
MUL R0.xyz, vertex.normal, c[5].x;
MOV R0.w, vertex.position;
ADD R0.xyz, vertex.position, R0;
DP4 result.position.w, R0, c[4];
DP4 result.position.z, R0, c[3];
DP4 result.position.y, R0, c[2];
DP4 result.position.x, R0, c[1];
MOV result.color, c[6];
END
# 8 instructions, 1 R-regs
"
}

SubProgram "d3d9 " {
Keywords { }
Bind "vertex" Vertex
Bind "normal" Normal
Matrix 0 [glstate_matrix_mvp]
Float 4 [_Outline]
Vector 5 [_OutlineColor]
"vs_2_0
; 8 ALU
dcl_position0 v0
dcl_normal0 v1
mul r0.xyz, v1, c4.x
mov r0.w, v0
add r0.xyz, v0, r0
dp4 oPos.w, r0, c3
dp4 oPos.z, r0, c2
dp4 oPos.y, r0, c1
dp4 oPos.x, r0, c0
mov oD0, c5
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

varying lowp vec4 xlv_COLOR;

uniform lowp vec4 _OutlineColor;
uniform lowp float _Outline;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  mediump vec4 tmpvar_1;
  tmpvar_1 = _glesVertex;
  highp vec3 tmpvar_2;
  tmpvar_2 = (_glesVertex.xyz + (normalize (_glesNormal) * _Outline));
  tmpvar_1.xyz = tmpvar_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (gl_ModelViewProjectionMatrix * tmpvar_1);
  tmpvar_1 = tmpvar_3;
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
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying lowp vec4 xlv_COLOR;

uniform lowp vec4 _OutlineColor;
uniform lowp float _Outline;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  mediump vec4 tmpvar_1;
  tmpvar_1 = _glesVertex;
  highp vec3 tmpvar_2;
  tmpvar_2 = (_glesVertex.xyz + (normalize (_glesNormal) * _Outline));
  tmpvar_1.xyz = tmpvar_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (gl_ModelViewProjectionMatrix * tmpvar_1);
  tmpvar_1 = tmpvar_3;
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

#LINE 92

        }
    }
    Fallback "Transparent/Diffuse"
}