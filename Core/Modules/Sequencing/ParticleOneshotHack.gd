extends GPUParticles3D
##
##	This class should probably be part of the engine featureset. In the meantime, we can have a global class to provide this feature set. Eventually if I have the time I could look at the engine code and see how hard of a feat adding this feature would be.
##
class_name VFXParticlesGPU

func _ready() -> void:
	emitting = true
	one_shot = true

func _process(_delta : float) -> void:
	if not emitting:
		queue_free()
