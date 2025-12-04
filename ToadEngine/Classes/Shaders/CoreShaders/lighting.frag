#version 330 core
struct Material 
{
	sampler2D diffuse;
	sampler2D specular;
	sampler2D normals;
	float shininess;
	bool hasNormalMap;
};

struct DirLight
{
	vec3 direction;

	vec3 ambient;
	vec3 diffuse;
	vec3 specular;

	sampler2D shadowMap;
	mat4 fragPosLightSpace;
};

struct PointLight 
{
	vec3 position;

	float constant;
	float linear;
	float quadratic;

	vec3 ambient;
	vec3 diffuse;
	vec3 specular;
};

struct SpotLight 
{
	vec3 position;
	vec3 direction;

	float cutOff;
	float outerCutOff;

	vec3 ambient;
	vec3 diffuse;
	vec3 specular;

	float constant;
	float linear;
	float quadratic;

	//int shadowIndex;
};

//uniform sampler2D pointLightShadowMaps[16];
//uniform mat4 pointLightFragPosLightSpaces[16];

uniform DirLight dirLight;

#define NR_POINT_LIGHTS 100
uniform PointLight pointLights[NR_POINT_LIGHTS];
uniform int pointLightAmount;

#define NR_SPOT_LIGHTS 100
uniform SpotLight spotLights[NR_SPOT_LIGHTS];
uniform int spotLightAmount;

uniform Material material;
uniform vec3 viewPos;

out vec4 FragColor;

in vec3 Normal;
in vec3 FragPos;
in vec2 TexCoords;
in vec3 worldPos;
in vec3 worldN;

vec3 CalcDirLight(DirLight light, vec3 normal, vec3 viewDir);
vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir);
vec3 CalcSpotLight(SpotLight light, vec3 normal, vec3 fragPos, vec3 viewDir);
float ShadowCalculation(vec4 fragPosLightSpace, sampler2D shadowMap, float bias);
vec3 getNormalFromMap();

void main()
{	
	vec3 norm;

	if (material.hasNormalMap) 
	{
		norm = getNormalFromMap();
	}
	else norm = normalize(Normal);
	
	vec3 viewDir = normalize(viewPos - FragPos);
	vec3 result = CalcDirLight(dirLight, norm, viewDir);

	if (pointLightAmount != 0)
	{
		for (int i = 0; i < pointLightAmount; i++)
			result += CalcPointLight(pointLights[i], norm, FragPos, viewDir);
	}

	if (spotLightAmount != 0)
	{
		for (int i = 0; i < spotLightAmount; i++)
			result += CalcSpotLight(spotLights[i], norm, FragPos, viewDir);
	}
	
	FragColor = vec4(result, 1.0);
}

float ShadowCalculation(vec4 fragPosLightSpace, sampler2D shadowMap, float bias)
{
	vec3 projCoords = fragPosLightSpace.xyz / fragPosLightSpace.w;
	projCoords = projCoords * 0.5 + 0.5;

	float closestDepth = texture(shadowMap, projCoords.xy).r;
	float currentDepth = projCoords.z;

	float shadow = 0.0;
	vec2 texelSize = 1.0 / textureSize(shadowMap, 0);
	for (int x = -1; x <= 2; x++)
	{
		for (int y = -1; y <= 1; y++)
		{
			float pcfDepth = texture(shadowMap, projCoords.xy + vec2(x, y) * texelSize).r;
			shadow += currentDepth - bias > pcfDepth ? 1.0 : 0.0;
		}
	}
	shadow /= 9.0;

	if (projCoords.z > 1.0)
		shadow = 0.0;

	return shadow;
}

vec3 getNormalFromMap()
{
	vec3 tangentNormal = texture(material.normals, TexCoords).xyz * 2.0 - 1.0;

	vec3 Q1 = dFdx(worldPos);
	vec3 Q2 = dFdy(worldPos);
	vec2 st1 = dFdx(TexCoords);
	vec2 st2 = dFdy(TexCoords);

	vec3 n = normalize(worldN);
	vec3 t = normalize(Q1 * st2.t - Q2 * st1.t);
	vec3 b = normalize(-Q1 * st2.s + Q2 * st1.s);

	mat3 tbn = mat3(t, b, n);

	return normalize(tbn * tangentNormal);
}

vec3 CalcDirLight(DirLight light, vec3 normal, vec3 viewDir)
{
	vec3 color = texture(material.diffuse, TexCoords).rgb;

	vec3 lightDir = normalize(-light.direction);
	vec3 halfwayDir = normalize(lightDir + viewDir);

	float diff = max(dot(normal, lightDir), 0.0);
	float spec = pow(max(dot(normal, halfwayDir), 0.0), material.shininess);

	vec3 ambient = light.ambient * vec3(texture(material.diffuse, TexCoords));
	vec3 diffuse = light.diffuse * diff * vec3(texture(material.diffuse, TexCoords));
	vec3 specular = light.specular * spec * vec3(texture(material.specular, TexCoords));
	
	float bias = max(0.005 * (1.0 - dot(normal, lightDir)), 0.005);
	vec4 fragPosLightSpace = vec4(FragPos, 1.0) * light.fragPosLightSpace;
	float shadow = ShadowCalculation(fragPosLightSpace, light.shadowMap, bias);

	return (ambient + (1.0 - shadow) * (diffuse + specular)) * color;
}

vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir)
{
	vec3 color = texture(material.diffuse, TexCoords).rgb;

	vec3 lightDir = normalize(light.position - FragPos);
	vec3 halfwayDir = normalize(lightDir + viewDir);

	float diff = max(dot(normal, lightDir), 0.0);
	float spec = pow(max(dot(normal, halfwayDir), 0.0), material.shininess);

	float dist = length(light.position - fragPos);
	float attenuation = 1.0 / (light.constant + light.linear * dist + light.quadratic * (dist * dist));

	vec3 ambient = light.ambient * vec3(texture(material.diffuse, TexCoords));
	vec3 diffuse = light.diffuse * diff * vec3(texture(material.diffuse, TexCoords));
	vec3 specular = light.specular * spec * vec3(texture(material.specular, TexCoords));

	ambient *= attenuation;
	diffuse *= attenuation;
	specular *= attenuation;

	//float shadow = ShadowCalculation(FragPosLightSpace, 0.05);
	return (ambient + (1.0 - 0.0) * (diffuse + specular)) * color;
}

vec3 CalcSpotLight(SpotLight light, vec3 normal, vec3 fragPos, vec3 viewDir)
{
	vec3 color = texture(material.diffuse, TexCoords).rgb;

	vec3 lightDir = normalize(light.position - FragPos);
	vec3 halfwayDir = normalize(lightDir + viewDir);

	float diff = max(dot(normal, lightDir), 0.0);
	float spec = pow(max(dot(normal, halfwayDir), 0.0), material.shininess);

	float dist = length(light.position - FragPos);
	float attenuation = 1.0 / (light.constant + light.linear * dist + light.quadratic * (dist * dist));

	float theta = dot(lightDir, normalize(-light.direction));
	float epsilon = light.cutOff - light.outerCutOff;
	float intensity = clamp((theta - light.outerCutOff) / epsilon, 0.0, 1.0);

	vec3 ambient = light.ambient * vec3(texture(material.diffuse, TexCoords));
	vec3 diffuse = light.diffuse * diff * vec3(texture(material.diffuse, TexCoords));
	vec3 specular = light.specular * spec * vec3(texture(material.specular, TexCoords));

	ambient *= attenuation;
	diffuse *= attenuation * intensity;
	specular *= attenuation * intensity;

	float bias = max(0.05 * (1.0 - dot(normal, lightDir)), 0.005);
	//vec4 fragPosLightSpace = vec4(FragPos, 1.0) * pointLightFragPosLightSpaces[light.shadowIndex];
	//float shadow = ShadowCalculation(fragPosLightSpace, pointLightShadowMaps[light.shadowIndex], bias);

	return (ambient + (1.0 - 0.0) * (diffuse + specular)) * color;
}