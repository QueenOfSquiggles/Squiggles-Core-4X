namespace Squiggles.Core.Scenes.UI.Menus.Gameplay;

using Godot;

[GlobalClass]
public partial class OptionSlider : OptionBase {

  [Export] public float DefaultValue = 0.5f;
  [Export] public float MinValue = 0.0f;
  [Export] public float MaxValue = 1.0f;
  [Export] public float StepValue = 0.001f;
  [Export] public bool AllowLesser = false;
  [Export] public bool AllowGreater = false;

}
