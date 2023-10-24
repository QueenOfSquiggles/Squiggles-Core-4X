shader_type canvas_item;

uniform float blur_amount = 1.5f;
uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_nearest, repeat_disable;

void fragment() {
	float r = blur_amount;
	vec4 sample = texture(SCREEN_TEXTURE, SCREEN_UV);
	sample += texture(SCREEN_TEXTURE, SCREEN_UV + vec2(r, r) * SCREEN_PIXEL_SIZE);
	sample += texture(SCREEN_TEXTURE, SCREEN_UV + vec2(-r, r) * SCREEN_PIXEL_SIZE);
	sample += texture(SCREEN_TEXTURE, SCREEN_UV + vec2(r, -r) * SCREEN_PIXEL_SIZE);
	sample += texture(SCREEN_TEXTURE, SCREEN_UV + vec2(-r, -r) * SCREEN_PIXEL_SIZE);
	sample += texture(SCREEN_TEXTURE, SCREEN_UV + vec2(r, 0) * SCREEN_PIXEL_SIZE);
	sample += texture(SCREEN_TEXTURE, SCREEN_UV + vec2(-r, 0) * SCREEN_PIXEL_SIZE);
	sample += texture(SCREEN_TEXTURE, SCREEN_UV + vec2(0, r) * SCREEN_PIXEL_SIZE);
	sample += texture(SCREEN_TEXTURE, SCREEN_UV + vec2(0, -r) * SCREEN_PIXEL_SIZE);
	
	COLOR = sample / 9.0;
	COLOR.a = 1.0f;
}
