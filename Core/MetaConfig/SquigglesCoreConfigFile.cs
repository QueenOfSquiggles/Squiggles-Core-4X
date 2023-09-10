using System;
using Godot;

[GlobalClass]
public partial class SquigglesCoreConfigFile : Resource
{

	[ExportGroup("Required Properties")]
	[Export(PropertyHint.File, "*.tscn")] public string PlayScene = "";
	[Export] public Texture2D GameLogo;

	[ExportGroup("Scene Overrides")]
	[Export(PropertyHint.File, "*.tscn")] public string LaunchSceneOverride = "";
	[Export(PropertyHint.File, "*.tscn")] public string MainMenuOverride = "";


	[ExportGroup("Registries", "Registry")]
	[Export] public string RegistryPathPattern = "res://Game/Registries/%s/";
	[Export] public string[] RegistryTypes = Array.Empty<string>();

	[ExportGroup("Credits")]
	[Export] public string[] CreditsLines = Array.Empty<string>();



}
