namespace Squiggles.Core.Scenes;

using System.Reflection;
using Godot;
using Squiggles.Core.Data;
using Squiggles.Core.Error;
using Squiggles.Core.Scenes.Registration;
using Squiggles.Core.Scenes.Utility;

/// <summary>
/// This is your main scene! This is the default main scene for SC4X as it handles a ton of rerouting and initialization processes.
/// </summary>
public partial class SC4X : Node
{

  private static SC4X _instance;

  /// <summary>
  /// A reference to the warning label that will show any malformed configuration settings
  /// </summary>
  [Export] private Label _warningLabel;

  /// <summary>
  /// The expected path of the configuration file
  /// </summary>
  private const string CONFIG_FILE_PATH = "res://squiggles_config.tres";
  /// <summary>
  /// The path for the default launch sequence. (which is loaded as soon as possible)
  /// </summary>
  private const string DEFAULT_LAUNCH_SEQUENCE = "res://Core/Scenes/UI/LaunchSequence/launch_sequence.tscn";

  /// <summary>
  /// A publicly available access to the current instance's configuration file.
  /// </summary>
  public static SquigglesCoreConfigFile Config => _instance?._config;

  private SquigglesCoreConfigFile _config;

  public override void _Ready()
  {
    Print.ClearLogFile();
    Print.AddSystemRedirect();

    // load data singletons
    Access.Load();
    AudioBuses.Load();
    Controls.Load();
    Effects.Load();
    GameplaySettings.Load();
    Graphics.Load();
    Stats.Load();

    if (_warningLabel is not null)
    {
      _warningLabel.Text = "";
    }

    _instance = this;
    _config = TryLoadConfigs();
    if (_config is not null)
    {
      ProcessConfig(_config);
      LoadNextScene(_config);
    }
    else if (_warningLabel is not null)
    {
      var msg = $"Failed to find configuration file at: \n {CONFIG_FILE_PATH}";
      _warningLabel.Text = msg;
      Print.Warn(msg);
    }
  }

  private static SquigglesCoreConfigFile TryLoadConfigs()
  {
    var loaded = GD.Load<SquigglesCoreConfigFile>(CONFIG_FILE_PATH);
    return loaded;
  }

  private static void ProcessConfig(SquigglesCoreConfigFile config)
  {
    foreach (var reg in config.RegistryTypes)
    {
      var path = config.RegistryPathPattern.Replace("%s", reg);
      RegistrationManager.RegisterRegistryType(reg, path);
    }
    RegistrationManager.ReloadRegistries();
  }

  private static void LoadNextScene(SquigglesCoreConfigFile config)
  {
    var path = DEFAULT_LAUNCH_SEQUENCE;
    if (config.LaunchSceneOverride.Length > 5)
    {
      path = config.LaunchSceneOverride;
    }

    Print.Debug($"Loading into launch sequence: {path}");
    SceneTransitions.LoadSceneAsync(path);
  }

}
