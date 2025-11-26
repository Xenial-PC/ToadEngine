#version 330 core
layout(location = 0) in vec2 aPosition;

uniform vec2 position;
uniform vec2 size;
uniform mat4 projection;

void main()
{
    vec2 world = position + aPosition * size;
    gl_Position = vec4(world, 0.0, 1.0) * projection;
}