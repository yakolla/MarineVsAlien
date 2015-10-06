Shader "ToonShader1.5/Toon Basic Outline/Ortho/Basic"
{
    Properties
    {
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
        
        UsePass "ToonShader1.5/Toon Basic/Basic/BASE"
        
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

#LINE 57

       }
    }
    Fallback "Unlit/Texture"
}