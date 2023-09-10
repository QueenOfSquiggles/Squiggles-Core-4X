shader_type canvas_item;

//	Credit (CC0 License)
//	"mackatap" on Godot Shaders
//	https://godotshaders.com/shader/diamond-based-screen-transition/

// Ranges from 0 to 1 over the course of the transition.
// We use this to actually animate the shader.
uniform float progress : hint_range(0, 1) = 0.0;
// Size of each diamond, in pixels.
uniform float diamondPixelSize = 10.0;

uniform bool down = true;

uniform vec4 bg_colour : source_color = vec4(vec3(0.0), 1.0);

void fragment() {
	float xFraction = fract(FRAGCOORD.x / diamondPixelSize);
	float yFraction = fract(FRAGCOORD.y / diamondPixelSize);
	float xDistance = abs(xFraction - 0.5);
	float yDistance = abs(yFraction - 0.5);

	if (down && (xDistance + yDistance + UV.x + UV.y > progress * 4.0)) {
		discard;
	}
	if (!down && (xDistance + yDistance + UV.x + UV.y < progress * 4.0)) {
		discard;
	}
	
	COLOR = bg_colour;
	COLOR.a = 1.0;
}