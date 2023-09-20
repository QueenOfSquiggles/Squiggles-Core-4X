namespace Squiggles.Core.Scenes.UI.Menus;

using Godot;

public partial class VersioningLabel : Label {
  [Export] private string _textRelease = "Full Release";
  [Export] private string _textDemo = "Demo Version";

  public override void _Ready() {
    Text = OS.HasFeature("demo") ? _textDemo : _textRelease;
#if DEBUG
    Text += " - debug";
#endif
  }


}
