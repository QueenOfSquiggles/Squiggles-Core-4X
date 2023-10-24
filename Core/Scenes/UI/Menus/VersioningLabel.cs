namespace Squiggles.Core.Scenes.UI.Menus;

using Godot;

/// <summary>
/// A label that appears in the default main menu that displays what kind of build is being played on. Helpful for quickly determining what kind of context a player is using so you can better provide assistance when troubles arise.
/// </summary>
public partial class VersioningLabel : Label {
  /// <summary>
  /// The text to display when a release build is detected
  /// </summary>
  [Export] private string _textRelease = "Full Release";
  /// <summary>
  /// The text ot display when a demo build is detected
  /// </summary>
  [Export] private string _textDemo = "Demo Version";

  public override void _Ready() {
    Text = OS.HasFeature("demo") ? _textDemo : _textRelease;
#if DEBUG
    Text += " - debug";
#endif
  }


}
