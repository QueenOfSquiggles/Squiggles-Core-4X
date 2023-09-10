@tool
extends BoxContainer

signal ValueChangedLazy(value)

@onready var label = $Lbl
@onready var slider = $HBoxContainer/HSlider
 
@export var text : String = "" :
	set(value):
		text = value
		$Lbl.text = value

@export var slider_value : float = 1.0 :
	set(n_value):
		slider_value = n_value
		$HBoxContainer/HSlider.value = n_value

@export var min_value : float = 0.0 :
	set(value):
		min_value = value
		$HBoxContainer/HSlider.min_value = value

@export var max_value : float = 1.0 :
	set(value):
		max_value = value
		$HBoxContainer/HSlider.max_value = value

@export var step_value : float = 0.1 :
	set(value):
		step_value = value
		$HBoxContainer/HSlider.step = value

func _ready() -> void:
	label.text = text
	slider.value = slider_value
	slider.min_value = min_value
	slider.max_value = max_value
	slider.step = step_value


func _on_h_slider_drag_ended(value_changed: bool) -> void:
	if value_changed:
		emit_signal("ValueChangedLazy", slider.value)


func _on_btn_reset_pressed() -> void:
	slider.value = slider_value
