#version 150

layout (lines) in;
layout (triangle_strip) out;
layout (max_vertices = 8) out;

layout(binding = 0, std140) uniform mvp
{
mat4 projectionMatrix;
mat4 viewMatrix;
mat4 modelMatrix;
};

void main()
{
	float size = 1.0f;
	
	mat4 vp = projectionMatrix * viewMatrix;
	mat4 mv = viewMatrix;
	
	vec3 right = vec3(mv[0][0],
					  mv[0][1],
					  mv[0][2]);
					  
	vec3 up    = vec3(mv[1][0],
					  mv[1][1],
					  mv[1][2]);
  
	vec3 front = vec3(mv[2][0],
					  mv[2][1],
					  mv[2][2]);
					  
	
	
	vec3 u = normalize(gl_in[1].gl_Position.xyz - gl_in[0].gl_Position.xyz);
	
	gl_Position = (gl_in[0].gl_Position + vec4(0,size,0,1));
	EmitVertex();
	gl_Position = (gl_in[1].gl_Position + vec4(0,size,0,1));
	EmitVertex();
	gl_Position = (gl_in[0].gl_Position - vec4(0,size,0,1));
	EmitVertex();
	gl_Position = (gl_in[1].gl_Position - vec4(0,size,0,1));
	EmitVertex();
	EndPrimitive();
	
	gl_Position = (gl_in[0].gl_Position + vec4(size,0,0,1));
	EmitVertex();
	gl_Position = (gl_in[1].gl_Position + vec4(size,0,0,1));
	EmitVertex();
	gl_Position = (gl_in[0].gl_Position - vec4(size,0,0,1));
	EmitVertex();
	gl_Position = (gl_in[1].gl_Position - vec4(size,0,0,1));
	EmitVertex();
	EndPrimitive();
}