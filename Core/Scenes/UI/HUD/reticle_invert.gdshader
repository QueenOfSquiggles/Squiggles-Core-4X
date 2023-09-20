shader_type canvas_item;

uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear;

void fragment() {
	float mask = texture(TEXTURE, UV).a;
	if (mask <= 0.5)
	{
		discard;
	}
	vec3 inv = vec3(1.0) - texture(SCREEN_TEXTURE, UV).rgb;
	COLOR = vec4(inv, mask);
}
