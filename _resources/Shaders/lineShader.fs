#version 330 core

in vec2 Vertex_UV;
in vec4 Vertex_Color;
out vec4 FragColor;

layout(binding = 0, std140) uniform mvp
{
mat4 projectionMatrix;
mat4 viewMatrix;
mat4 modelMatrix;
};

layout(binding = 1, std140) uniform col
{
vec4 color;
};


void main()
{
  FragColor = color;
}