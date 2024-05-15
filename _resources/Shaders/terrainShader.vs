#version 420 
in vec3 POSITION;
in vec3 NORMALS;
out vec3 NORMALS_OUT;
out vec3 FRAG_OUT;

layout(binding = 0, std140) uniform mvp
{
mat4 projectionMatrix;
mat4 viewMatrix;
mat4 modelMatrix;
};

void main()
{
	FRAG_OUT = vec3(modelMatrix * vec4(POSITION, 1.0));
	NORMALS_OUT = NORMALS;
	gl_Position = projectionMatrix * viewMatrix * modelMatrix * vec4(POSITION, 1);
}