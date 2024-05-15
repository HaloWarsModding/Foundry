#version 330 core
in vec3 NORMALS_OUT;
in vec3 FRAG_OUT;
out vec4 FragColor;

void main()
{
	vec3 lightDir = normalize(vec3(0,10,0) - FRAG_OUT);
	float diff = max(dot(NORMALS_OUT, vec3(0,1,0)), 0.0);
	FragColor = vec4(.6,.6,.6,1) * diff;
}