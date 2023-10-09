namespace Squiggles.Core.Scenes.UI.Menus;

using System;
using Godot;
using Squiggles.Core.Data;
using Squiggles.Core.Error;
using Squiggles.Core.Scenes.Utility;

/// <summary>
/// The launch sequence is a controlled animation sequence that plays on startup. With default settings, it will always play on first launch. Then there will be <see cref="_chanceDoAnyway"/> (default 25%) chance to play it on subsequent launches. In debug builds (expected for editing and development) it is skipped unless <see cref="_testing"/> is true, in which case it will always show. Additionally, in a "demo" context it will always play regardless of whether its the first session. This is considered a small penalty for playing the demo version as, usually, a demo isn't something that you play for more than a handfull of sessions (if more than just one!)
/// </summary>
public partial class LaunchSequence : Control {
  /// <summary>
  /// True if currently testing the launch sequence.
  /// </summary>
  [Export] private bool _testing = false;
  /// <summary>
  /// The percent chance (0.0-1.0) that the launch animation will play in a release build, even though it is not the first launch, in which case it will always play.
  /// </summary>
  [Export(PropertyHint.Range, "0,1,0.01")] private float _chanceDoAnyway = 0.25f;
  /// <summary>
  /// The animation player used to play the animation
  /// </summary>
  [Export] private AnimationPlayer _anim;
  /// <summary>
  /// A reference to the game logo texture so we can load the game's custom logo from configuration.
  /// </summary>
  [Export] private TextureRect _gameLogo;
  // TODO: add in the "created by" label so it can load the author's name.
  // TODO: maybe include a section about SC4X framework? IDK feels narcissistic.

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
				_chanceDoAnyway = 2.0f;
			} else {
				Print.Debug("Detected Release Build");
			}
#endif
    // Load in configuratuion settings
    if (_gameLogo is not null) {
      _gameLogo.Texture = SC4X.Config?.GameLogo;
    }

    var ran = new Random();
    if (!Stats.FirstTimeLaunch && (ran.NextSingle() > _chanceDoAnyway)) {
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

    Stats.FirstTimeLaunch = false;
    Stats.SaveSettings();
    _anim?.Play("OpeningAnimation");
  }

  /// <summary>
  /// Used to load to the main menu. Called from the animation so that the <see cref="CutsceneSkipper"/> ensures operation.
  /// </summary>
  public void EndLaunchSequence() {
    var path = SC4X.Config?.MainMenuOverride ?? "";
    if (path is null or "") {
      path = DEFAULT_MAIN_MENU;
    }

    Stats.FirstTimeLaunch = false;
    Stats.SaveSettings();
    SceneTransitions.LoadSceneAsync(path);
  }


}
