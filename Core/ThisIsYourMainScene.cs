using System;
using Godot;

public partial class ThisIsYourMainScene : Node
{

	private static ThisIsYourMainScene instance = null;

	[Export] private Label _WarningLabel;

	private const string CONFIG_FILE_PATH = "res://squiggles_config.tres";
	private const string DEFAULT_LAUNCH_SEQUENCE = "res://Core/Scenes/UI/LaunchSequence/launch_sequence.tscn";

	public static SquigglesCoreConfigFile Config
	{
		get
		{
			if (instance is null) return null;
			return instance.config;
		}
	}

	private SquigglesCoreConfigFile config = null;

	public override void _Ready()
	{
		_WarningLabel.Text = "";
		instance = this;
		config = TryLoadConfigs();
		if (config is not null)
		{
			ProcessConfig(config);
			LoadNextScene(config);
		}
		else
		{
			_WarningLabel.Text = $"Failed to find configuration file at: \n {CONFIG_FILE_PATH}";
		}
	}

	private SquigglesCoreConfigFile TryLoadConfigs()
	{
		var loaded = GD.Load<SquigglesCoreConfigFile>(CONFIG_FILE_PATH);
		return loaded;
	}

	private void ProcessConfig(SquigglesCoreConfigFile config)
	{
		foreach (var reg in config.RegistryTypes)
		{
			var path = config.RegistryPathPattern;
			RegistrationManager.RegisterRegistryType(reg, path);
		}
		RegistrationManager.ReloadRegistries();
	}

	private void LoadNextScene(SquigglesCoreConfigFile config)
	{
		var path = DEFAULT_LAUNCH_SEQUENCE;
		//if (config.LaunchSceneOverride != "") path = config.LaunchSceneOverride;

		Scenes.LoadSceneAsync(path);
	}

}
