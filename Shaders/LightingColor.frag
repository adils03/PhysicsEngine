#version 330 core

struct Material 
{
    float shininess;
};

struct Light 
{
    float constant;
    float linear;
    float quadratic;

    vec3 position;
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

#define NR_POINT_LIGHTS 4
uniform Light pointLights[NR_POINT_LIGHTS];

struct DirLight 
{
    vec3 direction;
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

uniform DirLight dirLight;

uniform Light light;
uniform Material material;
uniform vec3 viewPos;
uniform vec3 objectColor; // Yeni uniform renk değeri
uniform vec4 objectColor4; // Yeni uniform renk değeri

out vec4 FragColor;

in vec3 Normal;
in vec3 FragPos;

vec3 CalculateLighting(Light light, vec3 fragPos, vec3 normal, vec3 viewDir)
{
    vec3 lightDir = normalize(light.position - fragPos);
    float distance = length(light.position - fragPos);
    float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));
    float diff = max(dot(normal, lightDir), 0.0);
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    vec3 ambient = light.ambient * objectColor * attenuation; // Texture yerine uniform renk
    vec3 diffuse = light.diffuse * diff * objectColor4.rgb * attenuation; // Texture yerine uniform renk
    vec3 specular = light.specular * spec * objectColor4.rgb * attenuation; // Texture yerine uniform renk
    return (ambient + diffuse + specular);
}

vec3 CalcDirLight(DirLight light, vec3 normal, vec3 viewDir)
{
    vec3 lightDir = normalize(-light.direction);
    float diff = max(dot(normal, lightDir), 0.0);
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    vec3 ambient = light.ambient *  objectColor4.rgb; // Texture yerine uniform renk
    vec3 diffuse = light.diffuse * diff *  objectColor4.rgb; // Texture yerine uniform renk
    vec3 specular = light.specular * spec *  objectColor4.rgb; // Texture yerine uniform renk
    return (ambient + diffuse + specular);
}

void main() {
    vec3 viewDir = normalize(viewPos - FragPos);
    
    vec3 lightingResult = CalcDirLight(dirLight, Normal, viewDir);
   
    for(int i = 0; i < NR_POINT_LIGHTS; i++)
       lightingResult += CalculateLighting(pointLights[i], FragPos, Normal, viewDir);

    FragColor = vec4(lightingResult, 1.0);
}
