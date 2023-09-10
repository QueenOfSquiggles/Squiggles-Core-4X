using System;
using Godot;
using queen.data;
using queen.error;
using queen.extension;

public partial class LaunchSequence : Control
{
	[Export] private bool testing = false;
	[Export(PropertyHint.Range, "0,1,0.01")] private float chance_do_anyway = 0.25f;
	[Export] private AnimationPlayer _Anim;
	[Export] private TextureRect _GameLogo;

	private const string DEFAULT_MAIN_MENU = "res://Core/Scenes/UI/Menus/main_menu.tscn";

	public override void _Ready()
	{
#if DEBUG
		// For debug builds. Never run it, excepting first time launches.
		// Demo overrides
		Print.Debug("Detected Debug Build");
		chance_do_anyway = -2.0f;
#else
			if (OS.HasFeature("demo"))
			{
				// For the demo run animation every time
				Print.Debug("Detected Demo Build");
				chance_do_anyway = 2.0f;
			} else {
				Print.Debug("Detected Release Build");
			}
#endif

		_GameLogo.Texture = ThisIsYourMainScene.Config.GameLogo;
		var ran = new Random();
		if (!Stats.Instance.FirstTimeLaunch && (ran.NextSingle() > chance_do_anyway))
		{
#if DEBUG

			if (!testing)
			{
				EndLaunchSequence();
				return;
			}
			else
			{
				// technically doesn't have to be fixed. But for professionalism I want it to be
				Print.Warn("Launch Sequence is currently set to testing!!! Clear this before release!!!");
			}

#else

			EndLaunchSequence();
			return;

#endif
		}

		Stats.Instance.FirstTimeLaunch = false;
		Stats.SaveSettings();
		_Anim.Play("OpeningAnimation");
	}

	public void EndLaunchSequence()
	{
		var path = ThisIsYourMainScene.Config.MainMenuOverride;
		if (path == "") path = DEFAULT_MAIN_MENU;
		Scenes.LoadSceneAsync(path);
	}


}
