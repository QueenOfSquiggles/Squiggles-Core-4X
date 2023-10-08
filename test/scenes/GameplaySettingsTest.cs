namespace Squiggles.Test.Scenes;

using Godot;
using Squiggles.Core.Data;

public partial class GameplaySettingsTest : Control {

  [Export] private string _key = "test-value";
  [Export] private LineEdit _text;

  public override void _Ready() => _text.Text = GameplaySettings.GetString(_key);

  public void OnSave() {
    GameplaySettings.SetString(_key, _text.Text);
    GameplaySettings.SaveSettings();
  }

  public void OnLoad() => _text.Text = GameplaySettings.GetString(_key);

}
