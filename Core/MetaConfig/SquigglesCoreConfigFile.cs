namespace Squiggles.Core;
using System;
using Godot;
using Squiggles.Core.Meta;
using Squiggles.Core.Scenes.UI.Menus.Gameplay;

[GlobalClass]
public partial class SquigglesCoreConfigFile : Resource {

  [ExportGroup("Required Properties")]
  [Export(PropertyHint.File, "*.tscn")] public string PlayScene = "";
  [Export] public Texture2D GameLogo;

  [ExportGroup("Scene Overrides")]
  [Export(PropertyHint.File, "*.tscn")] public string LaunchSceneOverride = "";
  [Export(PropertyHint.File, "*.tscn")] public string MainMenuOverride = "";


  [ExportGroup("Options Menus")]

  [ExportSubgroup("Remappable Controls")]
  [Export] public string[] RemapControlsNames = Array.Empty<string>();
  [Export] public bool HideUIMappings = true;

  [ExportSubgroup("Gameplay Options")]
  [Export] public GameplayOptionsSettings GameplayConfig;


  [ExportGroup("Registries", "Registry")]
  [Export] public string RegistryPathPattern = "res://Game/Registries/%s/";
  [Export] public string[] RegistryTypes = Array.Empty<string>();

  [ExportGroup("Author Info", "Author")]
  [Export] public string AuthorName = "";
  [Export] public string AuthorGamesURL = "";
  [ExportGroup("Save Slot Handling")]
  [Export] public SaveSlotSettings SaveSlotHandlingSettings;


  [ExportGroup("Credits")]
  [Export] public string[] CreditsLines = Array.Empty<string>();



}
