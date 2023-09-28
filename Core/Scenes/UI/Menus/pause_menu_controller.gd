class_name PauseMenuController
extends Node
@export var pause_menu_scene : PackedScene = preload("res://Core/Scenes/UI/Menus/pause_menu.tscn")

func _unhandled_input(event: InputEvent) -> void:
	if event.is_action_pressed("ui_cancel"):
		var node = pause_menu_scene.instantiate()
		add_child(node)
		get_viewport().set_input_as_handled()
