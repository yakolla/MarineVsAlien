Shader "ToonShader1.5/Toon Basic/Basic" {
    Properties 
    {
        _ToonShade ("Shading Texture", 2D) = "gray" {}
    }
   
    Subshader 
    {
    	Tags {"RenderType"="Opaque" "Queue"="Geometry" "IgnoreProjector"="True"}
    	Cull Back
        Lighting Off
        Fog { Mode Off }
        LOD 100
        Pass 
        {
            Name "BASE"
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
void main ()
{
  gl_FragData[0] = texture2D (_ToonShade, xlv_TEXCOORD2);
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
void main ()
{
  gl_FragData[0] = texture2D (_ToonShade, xlv_TEXCOORD2);
}



#endif"
}

SubProgram "flash " {
Keywords { }
Bind "vertex" Vertex
Bind "normal" Normal
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [glstate_matrix_invtrans_modelview0]
"agal_vs
c8 0.5 0.0 0.0 0.0
[bc]
bcaaaaaaaaaaabacabaaaaoeaaaaaaaaaeaaaaoeabaaaaaa dp3 r0.x, a1, c4
bcaaaaaaaaaaacacabaaaaoeaaaaaaaaafaaaaoeabaaaaaa dp3 r0.y, a1, c5
adaaaaaaaaaaadacaaaaaafeacaaaaaaaiaaaaaaabaaaaaa mul r0.xy, r0.xyyy, c8.x
abaaaaaaacaaadaeaaaaaafeacaaaaaaaiaaaaaaabaaaaaa add v2.xy, r0.xyyy, c8.x
bdaaaaaaaaaaaiadaaaaaaoeaaaaaaaaadaaaaoeabaaaaaa dp4 o0.w, a0, c3
bdaaaaaaaaaaaeadaaaaaaoeaaaaaaaaacaaaaoeabaaaaaa dp4 o0.z, a0, c2
bdaaaaaaaaaaacadaaaaaaoeaaaaaaaaabaaaaoeabaaaaaa dp4 o0.y, a0, c1
bdaaaaaaaaaaabadaaaaaaoeaaaaaaaaaaaaaaoeabaaaaaa dp4 o0.x, a0, c0
aaaaaaaaacaaamaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v2.zw, c0
"
}

}
Program "fp" {
// Fragment combos: 1
//   opengl - ALU: 1 to 1, TEX: 1 to 1
//   d3d9 - ALU: 1 to 1, TEX: 1 to 1
SubProgram "opengl " {
Keywords { }
SetTexture 0 [_ToonShade] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 1 ALU, 1 TEX
TEX result.color, fragment.texcoord[2], texture[0], 2D;
END
# 1 instructions, 0 R-regs
"
}

SubProgram "d3d9 " {
Keywords { }
SetTexture 0 [_ToonShade] 2D
"ps_2_0
; 1 ALU, 1 TEX
dcl_2d s0
dcl t2.xy
texld r0, t2, s0
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
SetTexture 0 [_ToonShade] 2D
"agal_ps
[bc]
ciaaaaaaaaaaapacacaaaaoeaeaaaaaaaaaaaaaaafaababb tex r0, v2, s0 <2d wrap linear point>
aaaaaaaaaaaaapadaaaaaaoeacaaaaaaaaaaaaaaaaaaaaaa mov o0, r0
"
}

}

#LINE 48

        }
    }
    Fallback "Unlit/Texture"
}