@tool
extends EditorPlugin


func _enter_tree() -> void:
  # load autoload singletons (scene files as they require a particular structure)
	add_autoload_singleton("Scenes", "res://Core/Scenes/Utility/Autoload/scenes.tscn")
	add_autoload_singleton("BGM", "res://Core/Scenes/Utility/Autoload/bgm.tscn")

	# set up project settings
	ProjectSettings.set_setting("gui/theme/custom", "res://Core/Assets/Theme/default_theme.tres")
	_hide_api_keys()

func _exit_tree() -> void:
	remove_autoload_singleton("Scenes")
	remove_autoload_singleton("BGM")


func _hide_api_keys() -> void:
	if not FileAccess.file_exists("res://appconfig.json"):
		return # no app.json file to hide
	var ignoreFile := FileAccess.open("res://.gitignore", FileAccess.READ_WRITE)
	var contents = ignoreFile.get_as_text()
	if "appconfig.json" in contents:
		return
	ignoreFile.seek_end()
	ignoreFile.store_line("# SC4X configuration file for secret API keys")
	ignoreFile.store_line("appconfig.json")
	print("Successfully hid 'appconfig.json' file! You should really add that to your .gitignore yourself :/")
