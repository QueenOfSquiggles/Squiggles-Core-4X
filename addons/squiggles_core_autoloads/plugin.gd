@tool
extends EditorPlugin


func _enter_tree() -> void:
	_hide_api_keys()
  # load autoload singletons (scene files as they require a particular structure)
	add_autoload_singleton("Scenes", "res://Core/Scenes/Utility/Autoload/scenes.tscn")
	add_autoload_singleton("BGM", "res://Core/Scenes/Utility/Autoload/bgm.tscn")

	# set up project settings
	_set_setting_if_empty("gui/theme/custom", "res://Core/Assets/Theme/default_theme.tres")
	_set_setting_if_empty("application/run/main_scene", "res://Core/sc4x_entry.tscn")

	if ProjectSettings.get_setting("application/run/main_scene") == "":
		# if no main scene is currently set
		ProjectSettings.set_setting("application/run/main_scene", "")
		print("Auto set main scene to SC4X main scene")

func _exit_tree() -> void:
	remove_autoload_singleton("Scenes")
	remove_autoload_singleton("BGM")

func _set_setting_if_empty(key : String, value : String) -> void:
	if ProjectSettings.get_setting(key) == "":
		# if no main scene is currently set
		ProjectSettings.set_setting(key, value)
		print("Automatically set Project Settings::'%s' to '%s' (SC4X default)" % [key, value])
	

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
