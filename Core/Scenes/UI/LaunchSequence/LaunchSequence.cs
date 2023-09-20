namespace Squiggles.Core.Scenes.UI.Menus;

using System;
using Godot;
using Squiggles.Core.Data;
using Squiggles.Core.Error;
using Squiggles.Core.Scenes.Utility;

public partial class LaunchSequence : Control {
  [Export] private bool _testing = false;
  [Export(PropertyHint.Range, "0,1,0.01")] private float _chanceDoAnyway = 0.25f;
  [Export] private AnimationPlayer _anim;
  [Export] private TextureRect _gameLogo;

  private const string DEFAULT_MAIN_MENU = "res://Core/Scenes/UI/Menus/main_menu.tscn";

  public override void _Ready() {
#if DEBUG
    // For debug builds. Never run it, excepting first time launches.
    // Demo overrides
    Print.Debug("Detected Debug Build");
    _chanceDoAnyway = -2.0f;
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
    if (_gameLogo is not null) {
      _gameLogo.Texture = ThisIsYourMainScene.Config?.GameLogo;
    }

    var ran = new Random();
    if (!Stats.Instance.FirstTimeLaunch && (ran.NextSingle() > _chanceDoAnyway)) {
#if DEBUG

      if (!_testing) {
        EndLaunchSequence();
        return;
      }
      else {
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
    _anim?.Play("OpeningAnimation");
  }

  public void EndLaunchSequence() {
    var path = ThisIsYourMainScene.Config?.MainMenuOverride ?? "";
    if (path is null or "") {
      path = DEFAULT_MAIN_MENU;
    }

    SceneTransitions.LoadSceneAsync(path);
  }


}
