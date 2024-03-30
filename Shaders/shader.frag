#version 330 core
in vec4 vColor; // Vertex shader'dan gelen renk bilgisi

out vec4 pixelColor; // Son çýktý rengi

void main(){
	pixelColor = vColor; // Gelen rengi direkt olarak çýktýya atadýk
}