#version 330 core

struct Material 
{
    sampler2D diffuse;
    sampler2D specular;
    float     shininess;
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

struct PointLight {
    vec3 position;

    float constant;
    float linear;
    float quadratic;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

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

out vec4 FragColor;

in vec3 Normal;
in vec3 FragPos;

in vec2 TexCoords;

vec3 CalculateLighting(Light plight,vec3 fragPos, vec3 normal, vec2 texCoords) 
{
    vec3 ambient = plight.ambient * vec3(texture(material.diffuse, texCoords));

    // Diffuse 
    vec3 norm = normalize(normal);
    vec3 lightDir = normalize(plight.position - fragPos);
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = plight.diffuse * diff * vec3(texture(material.diffuse, texCoords));

    // Specular
    vec3 viewDir = normalize(viewPos - fragPos);
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    vec3 specular = light.specular * spec * vec3(texture(material.specular, texCoords));

    vec3 result = ambient + diffuse + specular;
    return result;
}

vec3 CalcDirLight(DirLight light, vec3 normal, vec3 viewDir)
{
    vec3 lightDir = normalize(-light.direction);
    //diffuse shading
    float diff = max(dot(normal, lightDir), 0.0);
    //specular shading
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    //combine results
    vec3 ambient  = light.ambient  * vec3(texture(material.diffuse, TexCoords));
    vec3 diffuse  = light.diffuse  * diff * vec3(texture(material.diffuse, TexCoords));
    vec3 specular = light.specular * spec * vec3(texture(material.specular, TexCoords));
    return (ambient + diffuse + specular);
}


void main() {

     DirLight dirLight;

     dirLight.direction = vec3(-10.2, -10.0, -10.3);
     dirLight.ambient =   vec3(0.02, 0.02, 0.02);
     dirLight.diffuse =   vec3(0.4, 0.4, 0.4);
     dirLight.specular =  vec3(0.5, 0.5, 0.5);

    vec3 viewDir = normalize(viewPos - FragPos);


    vec3 lightingResult = CalculateLighting(light,FragPos, Normal, TexCoords);
    lightingResult += CalcDirLight(dirLight,Normal,viewDir);
    
    FragColor = vec4(lightingResult, 1.0);
}