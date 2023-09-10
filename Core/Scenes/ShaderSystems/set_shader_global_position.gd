@tool
extends Marker3D

@export var uniform_name := ""

func _process(_delta):
	RenderingServer.global_shader_parameter_set(uniform_name, global_position)
