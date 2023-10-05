namespace Squiggles.Core.Scenes.UI.Menus;

using System;
using Godot;
using Squiggles.Core.Attributes;
using Squiggles.Core.Scenes.Utility;

/// <summary>
/// The scene which displays the credits. These are loaded from the config: <see cref="SquigglesCoreConfigFile.CreditsLines"/>
/// </summary>
public partial class CreditsScene : Control {

  /// <summary>
  /// The path for the main menu. Legacy feature
  /// </summary>
  [MarkForRefactor("Obsolete", "property from when credits was a separate scene from the main menu.")]
  [Export(PropertyHint.File, "*.tscn")] private string _mainMenuPath = "";
  /// <summary>
  /// The root node for the credits lines.
  /// </summary>
  [Export] private Control _creditsLinesRoot;
  /// <summary>
  /// The styling to apply to the labels in the credits
  /// </summary>
  [Export] private LabelSettings _labelStyling;

  public override void _Ready() {
    var lines = SC4X.Config?.CreditsLines ?? Array.Empty<string>();
    foreach (var line in lines) {
      _creditsLinesRoot?.AddChild(new Label() {
        Text = line,
        LabelSettings = _labelStyling
      });
    }
  }

  [MarkForRefactor("Obsolete", "property from when credits was a separate scene from the main menu.")]
  private void OnMenuButtonPressed() => SceneTransitions.LoadSceneAsync(_mainMenuPath);
}
