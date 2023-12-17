#version 330 core

layout (location = 0) in vec3 vertPosition;
layout (location = 1) in vec2 vertTexCoord;

out vec2 texCoord;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main() {
    texCoord = vertTexCoord;
    gl_Position = vec4(vertPosition, 1.0) * model * view * projection;
}