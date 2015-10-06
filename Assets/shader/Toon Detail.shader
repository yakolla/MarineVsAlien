
Shader "Toon Shader/Toon Detail" {
    Properties 
    {
        _Color ("Base Color", Color) = (0.5,0.5,0.5,1.0)
        _ToonShade ("Shade", 2D) = "gray" {}
        _MainTex ("Detail", 2D) = "gray" {}
    }
   
    Subshader 
    {
    	ZWrite On
    	Fog {Mode Off}
		Cull Back
        Pass 
        {
            Name "BASE"
            Program "vp" {
// Vertex combos: 1
//   opengl - ALU: 8 to 8
//   d3d9 - ALU: 8 to 8
SubProgram "opengl " {
Keywords { }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Vector 9 [_MainTex_ST]
"!!ARBvp1.0
# 8 ALU
PARAM c[10] = { { 0.5 },
		state.matrix.mvp,
		state.matrix.modelview[0].invtrans,
		program.local[9] };
TEMP R0;
DP3 R0.x, vertex.normal, c[5];
DP3 R0.y, vertex.normal, c[6];
MAD result.texcoord[0].xy, vertex.texcoord[0], c[9], c[9].zwzw;
MAD result.texcoord[1].xy, R0, c[0].x, c[0].x;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 8 instructions, 1 R-regs
"
}

SubProgram "d3d9 " {
Keywords { }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [glstate_matrix_invtrans_modelview0]
Vector 8 [_MainTex_ST]
"vs_2_0
; 8 ALU
def c9, 0.50000000, 0, 0, 0
dcl_position0 v0
dcl_normal0 v1
dcl_texcoord0 v2
dp3 r0.x, v1, c4
dp3 r0.y, v1, c5
mad oT0.xy, v2, c8, c8.zwzw
mad oT1.xy, r0, c9.x, c9.x
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

varying mediump vec2 xlv_TEXCOORD1;
varying mediump vec2 xlv_TEXCOORD0;


uniform mediump vec4 _MainTex_ST;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  mediump vec4 tmpvar_1;
  mediump vec2 tmpvar_2;
  mediump vec2 tmpvar_3;
  highp vec4 tmpvar_4;
  tmpvar_4 = (gl_ModelViewProjectionMatrix * _glesVertex);
  tmpvar_1 = tmpvar_4;
  mat3 tmpvar_5;
  tmpvar_5[0] = gl_ModelViewMatrixInverseTranspose[0].xyz;
  tmpvar_5[1] = gl_ModelViewMatrixInverseTranspose[1].xyz;
  tmpvar_5[2] = gl_ModelViewMatrixInverseTranspose[2].xyz;
  highp vec2 tmpvar_6;
  tmpvar_6 = (((tmpvar_5 * normalize (_glesNormal)).xy * 0.5) + 0.5);
  tmpvar_3 = tmpvar_6;
  highp vec2 tmpvar_7;
  tmpvar_7 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_2 = tmpvar_7;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = tmpvar_3;
}



#endif
#ifdef FRAGMENT

varying mediump vec2 xlv_TEXCOORD1;
varying mediump vec2 xlv_TEXCOORD0;
uniform sampler2D _ToonShade;
uniform sampler2D _MainTex;
uniform lowp vec4 _Color;
void main ()
{
  gl_FragData[0] = (((texture2D (_ToonShade, xlv_TEXCOORD1) * _Color) * 2.0) * texture2D (_MainTex, xlv_TEXCOORD0));
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

varying mediump vec2 xlv_TEXCOORD1;
varying mediump vec2 xlv_TEXCOORD0;


uniform mediump vec4 _MainTex_ST;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  mediump vec4 tmpvar_1;
  mediump vec2 tmpvar_2;
  mediump vec2 tmpvar_3;
  highp vec4 tmpvar_4;
  tmpvar_4 = (gl_ModelViewProjectionMatrix * _glesVertex);
  tmpvar_1 = tmpvar_4;
  mat3 tmpvar_5;
  tmpvar_5[0] = gl_ModelViewMatrixInverseTranspose[0].xyz;
  tmpvar_5[1] = gl_ModelViewMatrixInverseTranspose[1].xyz;
  tmpvar_5[2] = gl_ModelViewMatrixInverseTranspose[2].xyz;
  highp vec2 tmpvar_6;
  tmpvar_6 = (((tmpvar_5 * normalize (_glesNormal)).xy * 0.5) + 0.5);
  tmpvar_3 = tmpvar_6;
  highp vec2 tmpvar_7;
  tmpvar_7 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_2 = tmpvar_7;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = tmpvar_3;
}



#endif
#ifdef FRAGMENT

varying mediump vec2 xlv_TEXCOORD1;
varying mediump vec2 xlv_TEXCOORD0;
uniform sampler2D _ToonShade;
uniform sampler2D _MainTex;
uniform lowp vec4 _Color;
void main ()
{
  gl_FragData[0] = (((texture2D (_ToonShade, xlv_TEXCOORD1) * _Color) * 2.0) * texture2D (_MainTex, xlv_TEXCOORD0));
}



#endif"
}

SubProgram "flash " {
Keywords { }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [glstate_matrix_invtrans_modelview0]
Vector 8 [_MainTex_ST]
"agal_vs
c9 0.5 0.0 0.0 0.0
[bc]
bcaaaaaaaaaaabacabaaaaoeaaaaaaaaaeaaaaoeabaaaaaa dp3 r0.x, a1, c4
bcaaaaaaaaaaacacabaaaaoeaaaaaaaaafaaaaoeabaaaaaa dp3 r0.y, a1, c5
adaaaaaaaaaaamacadaaaaoeaaaaaaaaaiaaaaoeabaaaaaa mul r0.zw, a3, c8
abaaaaaaaaaaadaeaaaaaapoacaaaaaaaiaaaaooabaaaaaa add v0.xy, r0.zwww, c8.zwzw
adaaaaaaaaaaadacaaaaaafeacaaaaaaajaaaaaaabaaaaaa mul r0.xy, r0.xyyy, c9.x
abaaaaaaabaaadaeaaaaaafeacaaaaaaajaaaaaaabaaaaaa add v1.xy, r0.xyyy, c9.x
bdaaaaaaaaaaaiadaaaaaaoeaaaaaaaaadaaaaoeabaaaaaa dp4 o0.w, a0, c3
bdaaaaaaaaaaaeadaaaaaaoeaaaaaaaaacaaaaoeabaaaaaa dp4 o0.z, a0, c2
bdaaaaaaaaaaacadaaaaaaoeaaaaaaaaabaaaaoeabaaaaaa dp4 o0.y, a0, c1
bdaaaaaaaaaaabadaaaaaaoeaaaaaaaaaaaaaaoeabaaaaaa dp4 o0.x, a0, c0
aaaaaaaaaaaaamaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v0.zw, c0
aaaaaaaaabaaamaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v1.zw, c0
"
}

}
Program "fp" {
// Fragment combos: 1
//   opengl - ALU: 5 to 5, TEX: 2 to 2
//   d3d9 - ALU: 4 to 4, TEX: 2 to 2
SubProgram "opengl " {
Keywords { }
Vector 0 [_Color]
SetTexture 0 [_ToonShade] 2D
SetTexture 1 [_MainTex] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 5 ALU, 2 TEX
PARAM c[2] = { program.local[0],
		{ 2 } };
TEMP R0;
TEMP R1;
TEX R0, fragment.texcoord[1], texture[0], 2D;
TEX R1, fragment.texcoord[0], texture[1], 2D;
MUL R0, R0, c[0];
MUL R0, R0, c[1].x;
MUL result.color, R0, R1;
END
# 5 instructions, 2 R-regs
"
}

SubProgram "d3d9 " {
Keywords { }
Vector 0 [_Color]
SetTexture 0 [_ToonShade] 2D
SetTexture 1 [_MainTex] 2D
"ps_2_0
; 4 ALU, 2 TEX
dcl_2d s0
dcl_2d s1
def c1, 2.00000000, 0, 0, 0
dcl t0.xy
dcl t1.xy
texld r1, t1, s0
texld r0, t0, s1
mul r1, r1, c0
mul r1, r1, c1.x
mul_pp r0, r1, r0
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

SubProgram "flash " {
Keywords { }
Vector 0 [_Color]
SetTexture 0 [_ToonShade] 2D
SetTexture 1 [_MainTex] 2D
"agal_ps
c1 2.0 0.0 0.0 0.0
[bc]
ciaaaaaaabaaapacabaaaaoeaeaaaaaaaaaaaaaaafaababb tex r1, v1, s0 <2d wrap linear point>
ciaaaaaaaaaaapacaaaaaaoeaeaaaaaaabaaaaaaafaababb tex r0, v0, s1 <2d wrap linear point>
adaaaaaaabaaapacabaaaaoeacaaaaaaaaaaaaoeabaaaaaa mul r1, r1, c0
adaaaaaaabaaapacabaaaaoeacaaaaaaabaaaaaaabaaaaaa mul r1, r1, c1.x
adaaaaaaaaaaapacabaaaaoeacaaaaaaaaaaaaoeacaaaaaa mul r0, r1, r0
aaaaaaaaaaaaapadaaaaaaoeacaaaaaaaaaaaaaaaaaaaaaa mov o0, r0
"
}

}

#LINE 53

        }
    }
}