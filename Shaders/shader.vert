#version 330 core

uniform vec2 ViewportSize;
layout (location=0) in vec3 aPosition;
layout (location=1) in vec4 aColor; // Renk bilgisini aldýk

out vec4 vColor; // Fragment shader'a göndereceðimiz renk bilgisi
uniform mat4 Model;//Bakýlacak
uniform mat4 View;//Bakýlacak
uniform mat4 Projection;//Bakýlacak

void main(){
    float nx = aPosition.x / ViewportSize.x * 2.0 - 1.0; 
    float ny = aPosition.y / ViewportSize.y * 2.0 - 1.0; 
    gl_Position = vec4(nx, ny, 0.0, 1.0) * Model * View * Projection; // Sonucu saklamak için gl_Position'a atama yaptýk
    vColor = aColor ; // Aldýðýmýz renk bilgisini fragment shader'a aktardýk
}                        