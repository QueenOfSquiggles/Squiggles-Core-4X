extends SubViewportContainer

@export var rotation_scale := 0.03
@export var lerp_speed = 0.4
@onready var object := $SubViewport/Camera3D/ObjectRotator

var is_drag := false;
var down_pos : Vector2
var current_pos : Vector2

func _unhandled_input(event: InputEvent) -> void:
	if event is InputEventMouseButton:
		var mb := event as InputEventMouseButton
		if mb.button_index == 1 or mb.button_index == 2:
			is_drag = mb.pressed
			if is_drag:
				down_pos = mb.position
	if event is InputEventMouseMotion:
		current_pos = (event as InputEventMouseMotion).position

func _process(delta: float) -> void:
	if not is_drag:
		return
	var mouse_change = current_pos - down_pos
	_rotate(mouse_change * rotation_scale, delta)

func _rotate(delta : Vector2, time_delta : float) -> void:
	object.rotation.x = lerp(object.rotation.x, delta.y, time_delta * lerp_speed)
	object.rotation.y = lerp(object.rotation.y, delta.x, time_delta * lerp_speed)

