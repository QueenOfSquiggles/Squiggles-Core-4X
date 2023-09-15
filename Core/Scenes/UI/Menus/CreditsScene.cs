namespace Squiggles.Core.Scenes.UI.Menus;

using System;
using Godot;
using Squiggles.Core.Scenes.Utility;

public partial class CreditsScene : Control {

  [Export(PropertyHint.File, "*.tscn")] private string _mainMenuPath = "";
  [Export] private Control _creditsLinesRoot;
  [Export] private LabelSettings _labelStyling;

  public override void _Ready() {
    var lines = ThisIsYourMainScene.Config?.CreditsLines ?? Array.Empty<string>();
    foreach (var line in lines) {
      _creditsLinesRoot?.AddChild(new Label() {
        Text = line,
        LabelSettings = _labelStyling
      });
    }
  }
  private void OnMenuButtonPressed() => SceneTransitions.LoadSceneAsync(_mainMenuPath);
}
