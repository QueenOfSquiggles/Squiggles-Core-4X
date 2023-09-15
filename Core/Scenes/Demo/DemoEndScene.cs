namespace Squiggles.Core.Scenes.Demo;
using Godot;
using Squiggles.Core.Scenes.Utility;

public partial class DemoEndScene : Control {
  [Export(PropertyHint.File, "*.tscn")] private string _mainMenuScene = "";

  // extra safe guard since we are likely to be coming from the play space, which has the mouse captured
  public override void _Ready() => Input.MouseMode = Input.MouseModeEnum.Visible;

  public void ReturnMainMenu() => SceneTransitions.LoadSceneAsync(_mainMenuScene);
}
