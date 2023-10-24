namespace Squiggles.Core.Scenes.Demo;
using Godot;
using Squiggles.Core.Attributes;
using Squiggles.Core.Scenes.Utility;

/// <summary>
/// A node for handling the end of the demo. It is not typically used directly but is rather spawned by <see cref="TriggerEndOfDemo"/>
/// </summary>
[MarkForRefactor("Refactor to GlobalClass", "This class will likely be refactored out to use GlobalClass. This will create breaking changes if the scene file is refernced.")]
public partial class DemoEndScene : Control {
  [Export(PropertyHint.File, "*.tscn")] private string _mainMenuScene = "";

  // extra safe guard since we are likely to be coming from the play space, which has the mouse captured
  public override void _Ready() => Input.MouseMode = Input.MouseModeEnum.Visible;

  /// <summary>
  /// A utility that allows returning to the main menu from calling code or a signal connection. Recommended case is to use an AnimationPlayer to call it with some kind of cutscene.
  /// </summary>
  public void ReturnMainMenu() => SceneTransitions.LoadSceneAsync(
    SC4X.Config?.MainMenuOverride?.Length > 0 ?
      SC4X.Config?.MainMenuOverride :
      _mainMenuScene
    );
}
