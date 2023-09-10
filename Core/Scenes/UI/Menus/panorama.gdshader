shader_type canvas_item;

// the left/right look amount. Ideally clamp this externally to prevent viewing edges
uniform float scroll = 0.0;
uniform float scroll_speed = 0.2;

// keep positive to maintain pseudo3D effect.
uniform float displacement_scale = 2.0;

// easiest to just make this a curve texture, but making PNG gives a ton of control across the Y axis. Curve texture is just super smooth and doesn't have any issues with tearing.
uniform sampler2D displacement_map : hint_default_black;

void fragment(){
	vec2 uv = UV + vec2(scroll, 0.0); // scroll the UV
	float displacement = texture(displacement_map, UV).r; // pull amount from map
	displacement *= displacement_scale; // scale
	displacement *= (0.5 - uv.y); // transform based on distance from center horizontal
	float time_offset = TIME * scroll_speed;
	COLOR = texture(TEXTURE, uv + vec2(time_offset, displacement));// pull source image, displaced by scroll and vertical stretch.
}