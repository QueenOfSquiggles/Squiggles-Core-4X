namespace Squiggles.Core.Scenes.UI.Menus;

using System;
using Godot;

/// <summary>
/// The scene which displays the credits. These are loaded from the config: <see cref="SquigglesCoreConfigFile.CreditsLines"/>
/// </summary>
public partial class CreditsScene : Control {

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
}
