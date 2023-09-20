@tool
extends EditorPlugin


func _enter_tree() -> void:
	add_autoload_singleton("Scenes", "res://Core/Scenes/Utility/Autoload/scenes.tscn")
	add_autoload_singleton("BGM", "res://Core/Scenes/Utility/Autoload/bgm.tscn")
	# set up project settings
	ProjectSettings.set_setting("gui/theme/custom", "res://Core/Assets/Theme/default_theme.tres")

func _exit_tree() -> void:
	remove_autoload_singleton("Scenes")
	remove_autoload_singleton("BGM")
