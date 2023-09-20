extends PanelContainer

@onready var slider_main_bus := $VBoxContainer/PanelContainer/GridContainer/SliderMain
@onready var slider_vo := $VBoxContainer/PanelContainer/GridContainer/SliderVoiceOver
@onready var slider_sfx := $VBoxContainer/PanelContainer/GridContainer/SliderSFX
@onready var slider_creature := $VBoxContainer/PanelContainer/GridContainer/SliderCreature
@onready var slider_drones := $VBoxContainer/PanelContainer/GridContainer/SliderDrones

func _ready() -> void:
	slider_main_bus.value = AudioServer.get_bus_volume_db(0)
	slider_vo.value = AudioServer.get_bus_volume_db(1)
	slider_sfx.value = AudioServer.get_bus_volume_db(2)
	slider_creature.value = AudioServer.get_bus_volume_db(3)
	slider_drones.value = AudioServer.get_bus_volume_db(4)

func _on_slider_main_drag_ended(value_changed: bool) -> void:
	if (value_changed):
		_on_slider_changed("Master", slider_main_bus.value)


func _on_slider_voice_over_drag_ended(value_changed: bool) -> void:
	if (value_changed):
		_on_slider_changed("VoiceOver", slider_vo.value)


func _on_slider_sfx_drag_ended(value_changed: bool) -> void:
	if (value_changed):
		_on_slider_changed("SoundEffects", slider_sfx.value)


func _on_slider_creature_drag_ended(value_changed: bool) -> void:
	if (value_changed):
		_on_slider_changed("CreatureSounds", slider_creature.value)


func _on_slider_drones_drag_ended(value_changed: bool) -> void:
	if (value_changed):
		_on_slider_changed("DronesAndMusic", slider_drones.value)


func _on_slider_changed(bus_name : String, value : float) -> void:
	var index := AudioServer.get_bus_index(bus_name)
	AudioServer.set_bus_volume_db(index, value)
