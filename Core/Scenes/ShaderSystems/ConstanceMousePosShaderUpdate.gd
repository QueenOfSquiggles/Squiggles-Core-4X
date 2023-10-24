extends Node

@export var mouse_pos_shader_uniform_name := "mouse_position"
@export var current_mouse_pos : Vector2

# This will just constantly find the viewport position of the mouse and assign the shader global. This allows for shaders to use the global mouse position to apply certain effects.
func _process(_delta):
	current_mouse_pos = get_viewport().get_mouse_position()
	RenderingServer.global_shader_parameter_set(mouse_pos_shader_uniform_name, current_mouse_pos)
