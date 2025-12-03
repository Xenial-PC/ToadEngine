#version 330 core
layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec3 aNormal;
layout(location = 2) in vec2 aTexCoords;
layout(location = 3) in vec3 aTangent;
layout(location = 4) in vec3 aBitangent;

out vec3 Normal;
out vec3 FragPos;
out vec2 TexCoords;
out mat3 TBN;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
	gl_Position = vec4(aPosition, 1.0) * model * view * projection;
	FragPos = vec3(vec4(aPosition, 1.0) * model);
	Normal = mat3(transpose(inverse(model))) * aNormal;
	TexCoords = aTexCoords;

	mat3 normalMatrix = mat3(transpose(inverse(model)));
	vec3 T = normalize(normalMatrix * aTangent);
	vec3 N = normalize(normalMatrix * aNormal);

	T = normalize(T - dot(T, N) * N);
	vec3 B = cross(N, T);

	TBN = mat3(T, B, N);
}