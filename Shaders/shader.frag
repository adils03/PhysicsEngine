#version 330 core
in vec4 vColor; // Vertex shader'dan gelen renk bilgisi

out vec4 pixelColor; // Son ��kt� rengi

void main(){
	pixelColor = vColor; // Gelen rengi direkt olarak ��kt�ya atad�k
}