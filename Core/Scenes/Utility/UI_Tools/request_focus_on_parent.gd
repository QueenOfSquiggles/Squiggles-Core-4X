extends Control

@export var focus_target : NodePath = ".."
@export var visibility_target : NodePath = ".."

func _ready() -> void:
	var par := get_node(visibility_target) as Control
	assert(par, "RequestFocusOnParent:target must be a control node that can take focus!")
	par.ready.connect(_request_focus)
	par.visibility_changed.connect(_req_focus_visbility)

func _request_focus() -> void:
	var par := get_node(focus_target) as Control
	par.grab_focus()
	#print("Grabbing focus: ", par.name)

func _req_focus_visbility() -> void:
	var par := get_node(visibility_target) as Control
	if not par.visible:
		return
	#print("Visibilty changed on", par.name)
	_request_focus()
