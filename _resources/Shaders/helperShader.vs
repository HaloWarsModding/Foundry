#version 420
in vec3 POSITION;

layout(binding = 0, std140) uniform mvp
{
mat4 projectionMatrix;
mat4 viewMatrix;
mat4 modelMatrix;
};

void main()
{
	gl_Position = projectionMatrix * viewMatrix * modelMatrix * vec4(POSITION, 1);
}