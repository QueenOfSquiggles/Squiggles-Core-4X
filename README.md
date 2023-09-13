# Squiggles Common 4X

A boilerplate game base for making games in Godot 4.X with C#

## Requirements
- Godot 4.X (newer versions may work with tweaks?)
- Vulkan supporting Graphics Card

## Licensing
The code, assets, and such within this repository are all under the MIT license unless otherwise specified.
There do exist several third party assets in this repo. For each pack the license is contained in a license file within the folder of the assets. I make no claim to have created these assets, they are solely the creation of their listed creators. Many such third party assets are used for testing purposes primarily and should be purged when creating a game using this game base. 

## Installation and Usage

The three main "objects" for this library are the `addons`, `Core`, and `squiggles_config.tres`. Each of these can and should be dropped into the root folder. `addons` and `Core` are set up to support symbolic linking. The `squiggles_config.tres` is a custom resource that defines the configurations for your game.

### Steps
1. Install main components: `addons`, `Core`, `squiggles_config.tres`
2. Build code
3. Enable Plugins
4. Set Main Scene to: `res://Core/this_is_your_main_scene.tscn`
5. Modify configurations in `squiggles_config`.
6. Test run

## Main Features
*I may have forgotten some lol*

- Pre-made Opening Launch Animation
- Support for Demo builds
	- Include "demo" as a [custom feature tag](https://docs.godotengine.org/en/stable/tutorials/export/feature_tags.html) on any export to mark it as a demo build
- Pre-made menus
	- Main Menu
	- Options Menu
	- Credits Sceen
	- End of Demo screen (used demo builds)
- Many options
	- Variety of Graphics Options
	- Pre-Constructed Audio Bus Layout with options
	- Accessibility Options
		- Alternative Font Options
			- Gothic (default)
			- Noto Sans
			- Open Dyslexie (intended to make reading easier for people with dyslexia)
		- Disable flashing lights
			- does need to be integrated on a game by game basis
		- Custom control bindings
			- supports Keyboard & Mouse and Generic Gamepad
			- Rebinds saved and reloaded automatically
		- Volume Limiter
			- Sets a maximum cap for audio to protect your ears. Intended for people with auditory sensitivities
		- Custom Screen Shake Stength & Duration
		- Custom Controller Rumble Strength & Duration
		- Engine-level time scaling
			- Affects the speed at which the engine processes all simulations. Faster and slower are both allowed.
- Scenes Management
	- Out-of-the-box fade animation (can be customized with the scene data)
	- Scenes loaded by string path to avoid cyclic dependencies between scenes
	- Supports loading PackedScene
- Background Music Manager
	- Persistent tracks (even across scene loadings)
	- Limit 1 track at a time
	- Crossfade between tracks for seamless changes
	- Load a `null` track to fade the current track out
- Comprehensive Printing System
	- Print a variety of options
		- Error
			- Also pushes an error to the Godot Engine for quick code search
		- Warning
			- Also pushes an error to the Godot Engine for quick code search
		- Info
		- Debug
			- Debug prints are only printed in a "Debug" environment such as the editor and debug builds. *Useful for when you want to ignore that breakpoints exist*.
	- Print custom messages with the `Msg` builder class
		- allows a builder-format approach to constructing BBCode formatted text.
- Easy special effects
	- Cutscene Management: allows skipping a cutscene with any input
	- Controller Rumble and Screen Shake: give some extra juice to your cinematics
- Virtual Camera System
	- Option to interpolate between cameras
	- Treated as a camera stack: removing a Vcam from the scene will revert to the previous camera, interpolating between if set to interpolate.
- Pre-build FPS controller character
- Defult HUD system
	- Allows for generating arbitrary GUI elements on screen while still having a few defaults
	- Subtitles system
	- Reticle for interaction
- Interactable system for creating interactable objects with custom prompts
	- does support translations, but you will need to establish the translation keys yourself
