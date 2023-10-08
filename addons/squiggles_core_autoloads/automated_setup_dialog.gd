@tool
extends ConfirmationDialog

@onready var btn_proj_set_default_theme: CheckButton = $PanelContainer/MarginContainer/VBoxContainer/ProjectSettings/HFlowContainer/BtnProjSetDefaultTheme
@onready var btn_proj_set_default_main: CheckButton = $PanelContainer/MarginContainer/VBoxContainer/ProjectSettings/HFlowContainer/BtnProjSetDefaultMain
@onready var check_create_app_config: CheckButton = $PanelContainer/MarginContainer/VBoxContainer/ConfigurationSettings/HFlowContainer/CheckCreateAppConfig
@onready var check_create_squiggles_config: CheckButton = $PanelContainer/MarginContainer/VBoxContainer/ConfigurationSettings/HFlowContainer/CheckCreateSquigglesConfig
@onready var check_dep_discord: CheckBox = $PanelContainer/MarginContainer/VBoxContainer/DotNetDependencies/HFlowContainer/CheckDepDiscord
@onready var check_dep_go_dot_test: CheckBox = $PanelContainer/MarginContainer/VBoxContainer/DotNetDependencies/HFlowContainer/CheckDepGoDotTest


func _on_confirmed() -> void:
	print("Starting SC4X Initialization Processes (if selected)")
	
	var requires_restart := false
	# set up project settings
	if btn_proj_set_default_theme.button_pressed:
		ProjectSettings.set_setting("gui/theme/custom", "res://Core/Assets/Theme/default_theme.tres")
		print("Updated default theme")
		requires_restart = true
	if btn_proj_set_default_main:
		ProjectSettings.set_setting("application/run/main_scene", "res://Core/sc4x_entry.tscn")
		print("Updated main scene")
		
	# create configuration files
	if check_create_app_config.button_pressed:
		var file := FileAccess.open("res://appconfig.json", FileAccess.WRITE)
		file.store_string(JSON.stringify({
			"KEY" : "VALUE"
		}, "\t"))
		print("Wrote 'appconfig.json'")
	if check_create_squiggles_config.button_pressed:
		var res = SquigglesCoreConfigFile.new()
		ResourceSaver.save(res, "res://squiggles_config.tres")
		print("Wrote 'squiggles_config.tres'")

	# .NET dependencies
	if check_dep_discord.button_pressed:
		_add_dotnet_dep("DiscordRichPresence")
	if check_dep_go_dot_test.button_pressed:
		_add_dotnet_dep("Chickensoft.GoDotTest")
	
	# check for security
	_hide_api_keys()

	if requires_restart:
		printerr("Some changes require a godot project reload to fully apply!")

	# close
	queue_free()

func _set_setting_if_empty(key : String, value : String) -> void:
	if ProjectSettings.get_setting(key) == "":
		# if no main scene is currently set
		ProjectSettings.set_setting(key, value)
		print("Automatically set Project Settings::'%s' to '%s' (SC4X default)" % [key, value])

func _add_dotnet_dep(depname : String) -> void:
	var arr = []
	var result := OS.execute("dotnet",["add", "package", depname], arr, true)
	if result != 0:
		printerr("Adding dotnet dependency exited with code %s for dependency '%s'" % [str(result), depname])
		print(arr)
	else:
		print("Added dotnet dependency: '%s'" % depname)

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

func _check_encryption_status() -> void:
	var dir = DirAccess.open("res://")
	if not dir:
		return
	if not dir.file_exists("appconfig.json") or not dir.file_exists("export_presets.cfg"):
		return
	var export_cfg = FileAccess.open("res://export_presets.cfg", FileAccess.READ)
	if not export_cfg:
		return
	var text = export_cfg.get_as_text()
	if not "encrypt_pck=false" in text:
		return
	printerr("SC4X detected that you are using an appconfig.json but are not encrypting your PCK files on export! This will allow hackers to read your API Keys as plain text! This is a really bad idea! See the link below for details on how to export encrypted builds!!! (note clicking doesn't work. Copy and paste into your browser)")
	print_rich("[url]https://docs.godotengine.org/en/stable/contributing/development/compiling/compiling_with_script_encryption_key.html[/url]")


func _on_canceled() -> void:
	queue_free()
