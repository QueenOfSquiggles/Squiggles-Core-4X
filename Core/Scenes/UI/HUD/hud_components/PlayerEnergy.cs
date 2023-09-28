namespace Squiggles.Core.Scenes.UI.HUD;

using Godot;
using Squiggles.Core.Events;

public partial class PlayerEnergy : TextureProgressBar {

  [Export] private Label _label;

  public override void _Ready()
    => EventBus.Gameplay.OnPlayerStatsUpdated += OnPlayerStatsUpdated;
  public override void _ExitTree()
    => EventBus.Gameplay.OnPlayerStatsUpdated -= OnPlayerStatsUpdated;

  private void OnPlayerStatsUpdated(float _1, float _2, float energy, float maxEnergy) {
    Value = energy / maxEnergy;
    _label.Text = energy.ToString("0");
  }
}
