    Shader "Toon Shader/Outlined Toon Detail"
    {
        Properties
        {
            _Color ("Base Color", Color) = (0.5,0.5,0.5,1.0)
            _OutlineColor ("Outline Color", Color) = (0.5,0.5,0.5,1.0)
            _Outline ("Outline width", Range (0.001, 0.05)) = 0.01
            _ToonShade ("Shading Texture (RGB)", 2D) = "gray" {}
            _MainTex ("Detail", 2D) = "gray" {}
        }
     
        SubShader
        {
           
            UsePass "Toon Shader/Toon Detail/BASE"
            
            Pass
            {
                Program "vp" {
// Vertex combos: 1
//   opengl - ALU: 15 to 15
//   d3d9 - ALU: 15 to 15
SubProgram "opengl " {
Keywords { }
Bind "vertex" Vertex
Bind "normal" Normal
Float 13 [_Outline]
Vector 14 [_OutlineColor]
"!!ARBvp1.0
# 15 ALU
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
DP4 R0.x, vertex.position, c[11];
MUL R0.w, R0.y, c[6].y;
MUL R0.zw, R0, R0.x;
DP4 R0.y, vertex.position, c[3];
MUL R0.zw, R0, c[13].x;
RCP R0.y, -R0.y;
DP4 R1.x, vertex.position, c[9];
DP4 R1.y, vertex.position, c[10];
MAD result.position.xy, R0.zwzw, R0.y, R1;
MOV result.color, c[14];
DP4 result.position.w, vertex.position, c[12];
MOV result.position.z, R0.x;
END
# 15 instructions, 2 R-regs
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
; 15 ALU
dcl_position0 v0
dcl_normal0 v1
dp3 r0.x, v1, c0
mul r0.z, r0.x, c4.x
dp3 r0.y, v1, c1
dp4 r0.x, v0, c10
mul r0.w, r0.y, c5.y
mul r0.zw, r0, r0.x
dp4 r0.y, v0, c2
mul r0.zw, r0, c12.x
rcp r0.y, -r0.y
dp4 r1.x, v0, c8
dp4 r1.y, v0, c9
mad oPos.xy, r0.zwzw, r0.y, r1
mov oD0, c13
dp4 oPos.w, v0, c11
mov oPos.z, r0.x
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
  highp vec4 tmpvar_1;
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
  tmpvar_1.xy = (tmpvar_2.xy + (((tmpvar_5 * tmpvar_2.z) * _Outline) / -((gl_ModelViewMatrix * _glesVertex).z)));
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
  highp vec4 tmpvar_1;
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
  tmpvar_1.xy = (tmpvar_2.xy + (((tmpvar_5 * tmpvar_2.z) * _Outline) / -((gl_ModelViewMatrix * _glesVertex).z)));
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

               
                Cull Front
                ZWrite On
                Blend SrcAlpha OneMinusSrcAlpha
                SetTexture [_ToonShade] { combine primary }
            }
        }
       
        Fallback "Diffuse"
    }