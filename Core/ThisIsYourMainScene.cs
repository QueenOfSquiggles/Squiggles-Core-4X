namespace Squiggles.Core.Scenes;

using System.Reflection;
using Chickensoft.GoDotTest;
using Godot;
using Squiggles.Core.Error;
using Squiggles.Core.Scenes.Registration;
using Squiggles.Core.Scenes.Utility;

public partial class ThisIsYourMainScene : Node {

  private static ThisIsYourMainScene _instance;

  [Export] private Label _warningLabel;

  private const string CONFIG_FILE_PATH = "res://squiggles_config.tres";
  private const string DEFAULT_LAUNCH_SEQUENCE = "res://Core/Scenes/UI/LaunchSequence/launch_sequence.tscn";

  public static SquigglesCoreConfigFile Config => _instance?._config;

  private SquigglesCoreConfigFile _config;

  public TestEnvironment Environment = default!;

  public override void _Ready() {
    Print.ClearLogFile();
    Print.AddSystemRedirect();
#if DEBUG
    // using Chickensoft.GoDotTest, execute tests.
    Environment = TestEnvironment.From(OS.GetCmdlineArgs());
    if (Environment.ShouldRunTests) {
      CallDeferred("RunTests");
      return;
    }
#endif
    if (_warningLabel is not null) {
      _warningLabel.Text = "";
    }

    _instance = this;
    _config = TryLoadConfigs();
    if (_config is not null) {
      ProcessConfig(_config);
      LoadNextScene(_config);
    }
    else if (_warningLabel is not null) {
      var msg = $"Failed to find configuration file at: \n {CONFIG_FILE_PATH}";
      _warningLabel.Text = msg;
      Print.Warn(msg);
    }
  }

  private void RunTests() => _ = GoTest.RunTests(Assembly.GetExecutingAssembly(), this, Environment);

  private static SquigglesCoreConfigFile TryLoadConfigs() {
    var loaded = GD.Load<SquigglesCoreConfigFile>(CONFIG_FILE_PATH);
    return loaded;
  }

  private static void ProcessConfig(SquigglesCoreConfigFile config) {
    foreach (var reg in config.RegistryTypes) {
      var path = config.RegistryPathPattern;
      RegistrationManager.RegisterRegistryType(reg, path);
    }
    RegistrationManager.ReloadRegistries();
  }

  private static void LoadNextScene(SquigglesCoreConfigFile config) {
    var path = DEFAULT_LAUNCH_SEQUENCE;
    if (config.LaunchSceneOverride.Length > 5) {
      path = config.LaunchSceneOverride;
    }

    Print.Debug($"Loading into launch sequence: {path}");
    SceneTransitions.LoadSceneAsync(path);
  }

}
