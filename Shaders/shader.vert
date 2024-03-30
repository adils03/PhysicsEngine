#version 330 core

uniform vec2 ViewportSize;
layout (location=0) in vec3 aPosition;
layout (location=1) in vec4 aColor; // Renk bilgisini ald�k

out vec4 vColor; // Fragment shader'a g�nderece�imiz renk bilgisi
uniform mat4 Model;//Bak�lacak
uniform mat4 View;//Bak�lacak
uniform mat4 Projection;//Bak�lacak

void main(){
    float nx = aPosition.x / ViewportSize.x * 2.0 - 1.0; 
    float ny = aPosition.y / ViewportSize.y * 2.0 - 1.0; 
    gl_Position = vec4(nx, ny, 0.0, 1.0) * Model * View * Projection; // Sonucu saklamak i�in gl_Position'a atama yapt�k
    vColor = aColor ; // Ald���m�z renk bilgisini fragment shader'a aktard�k
}                        