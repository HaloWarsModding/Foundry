#version 330 core
out vec4 FragColor;
layout(binding = 1, std140) uniform col
{
vec4 color;
};


void main()
{
FragColor = color;
}