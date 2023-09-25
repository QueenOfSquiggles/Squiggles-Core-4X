namespace Squiggles.Core;

using System;
using Godot;
using Squiggles.Core.Meta;
using Squiggles.Core.Scenes.UI.Menus.Gameplay;

/// <summary>
/// This is the main configuration file for your project. Think of this as your `project.godot` for interacting with SC4X. Setting up things properly, you should place this resource at `res://squiggles_config.tres` to properly integrate with the framework. An error will be thrown with more information if this is not done properly. See individual fields for more information on their purpose.
/// </summary>
[GlobalClass]
public partial class SquigglesCoreConfigFile : Resource {

  /// <summary>
  /// The default "game scene" to load. This scene will be loaded in any situation where the player would be entering the play space, whether that is starting a new game or loading an old save. Be sure to react to save data if that is important. Additionally, for larger projects you may want this scene to be a relay junction which selects which scene to load next based on save data
  /// </summary>
  [ExportGroup("Required Properties")]
  [Export(PropertyHint.File, "*.tscn")] public string PlayScene = "";
  /// <summary>
  /// This is the image that serves as your game's logo in the opening animation as well as the title screen. The UI components are designed to adjust so don't worry about aspect ratio. Just be sure to test size.
  /// </summary>
  [Export] public Texture2D GameLogo;
  /// <summary>
  /// Enables the reticle in the <see cref="DefaultHUD"/>. In some games you won't want a reticle at all so this allows disabling that feature.
  /// </summary>
  [Export] public bool EnableReticle = true;

  /// <summary>
  /// Determines whether to allow users (players) to modify the Brightness, Contrast, Saturation, and Exposure in the graphics settings. If you have a very particular style in mind, it would be wise to disable this
  /// </summary>
  [ExportGroup("Graphics")]
  [Export] public bool EnableColourCorrection = true;
  /// <summary>
  /// This is the default environment for your project. Load any <see cref="SettingsCompliantWorldEnvironment"/> into a scene and it will load this default scenes with any alterations applied from the player's settings. This enables you to create a custom feel to your game world, while also allowing players to configure performance options such as disabling SDFGI, SSAO, SSIL, and more.
  /// </summary>
  [Export] public Godot.Environment DefaultEnvironment;

  /// <summary>
  /// This is a string list of all action names that you want to enable remapping in the controls tab of the options menu. If this list is empty, all mappings will be allowed. If you want to disable all mappings, you would need to enter a value that is not associated with an action in the input map.
  /// </summary>
  [ExportGroup("Options Menus")]
  [ExportSubgroup("Remappable Controls")]
  [Export] public string[] RemapControlsNames = Array.Empty<string>();
  /// <summary>
  /// A utility to hide all action mappings that are prefixed with "ui" (case-sensitive) which will hide all built-in mappings.
  /// </summary>
  [Export] public bool HideUIMappings = true;

  /// <summary>
  /// An instance of the <see cref="GameplayOptionsSettings"/> resource which provides more details on gameplay settings. This is highly configurable and allows for setting up your preferred settings of your specific game in order to best serve your players
  /// </summary>
  [ExportSubgroup("Gameplay Options")]
  [Export] public GameplayOptionsSettings GameplayConfig;


  /// <summary>
  /// This is a pattern with "%s" where the type name of the resource should be located. It is possible your game will have no need of this feature. Larger projects may rely on this feature.
  /// </summary>
  [ExportGroup("Registries", "Registry")]
  [Export] public string RegistryPathPattern = "res://Game/Registries/%s/";
  /// <summary>
  /// The strings that are the type name of the necessary resources. Not the full name. Type associations are made when retrieving the resources, not when registering them.
  /// </summary>
  [Export] public string[] RegistryTypes = Array.Empty<string>();

  /// <summary>
  /// Metadata on who is making this game. Either your personal name, online alias, or studio name. Whatever you want front and center on the title screen
  /// </summary>
  [ExportGroup("Author Info", "Author")]
  [Export] public string AuthorName = "";
  /// <summary>
  /// A url to literally whatever. The main menu creates a URL link button with your <see cref="AuthorName"/> and clicking on it will perform an <c>OS.ShellOpen</c> with this URL. My personally recommendation is to link back to your creator page on whatever platform you prefer. But ultimately that's up to you
  /// </summary>
  [Export] public string AuthorGamesURL = "";

  /// <summary>
  ///   An instance of <see cref="SaveSlotSettings"/>. It handles how save slots should be presented in your game, and whether save data is even required at all.
  /// </summary>
  [ExportGroup("Save Slot Handling")]
  [Export] public SaveSlotSettings SaveSlotHandlingSettings;

  /// <summary>
  /// An override path to the launch sequence of your choice. The default launch sequence plays a short animation before loading into the main menu. Leave this value empty if you wish to use the built-in default
  /// </summary>
  [ExportGroup("Scene Overrides")]
  [Export(PropertyHint.File, "*.tscn")] public string LaunchSceneOverride = "";
  /// <summary>
  /// An override path to the main menu of your choice. Leave empty if you want the default
  /// </summary>
  [Export(PropertyHint.File, "*.tscn")] public string MainMenuOverride = "";


  /// <summary>
  /// An array of strings, each of which is a single line in the credits pane. Ideally you can add to this array as you go when you discover needs of attribution. Currently no formatting is done on the credits lines so feel free to write them as you wish.
  /// </summary>
  [ExportGroup("Credits")]
  [Export] public string[] CreditsLines = Array.Empty<string>();

}
