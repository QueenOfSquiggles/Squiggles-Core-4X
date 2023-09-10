extends CheckButton

@export var target_group := "access_description"

func _ready() -> void:
	_on_toggled(button_pressed)

func _on_toggled(_value: bool) -> void:
	var targets := get_tree().get_nodes_in_group(target_group)
	for o in targets:
		var con := o as Control
		if con == null:
			continue
		con.visible = _value
	
