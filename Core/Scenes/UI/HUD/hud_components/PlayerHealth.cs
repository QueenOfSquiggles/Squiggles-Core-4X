namespace Squiggles.Core.Scenes.UI.HUD;

using Godot;
using Squiggles.Core.Events;

public partial class PlayerHealth : TextureProgressBar {
  [Export] private Label _label;

  public override void _Ready()
    => EventBus.Gameplay.OnPlayerStatsUpdated += OnPlayerStatsUpdated;
  public override void _ExitTree()
    => EventBus.Gameplay.OnPlayerStatsUpdated -= OnPlayerStatsUpdated;

  private void OnPlayerStatsUpdated(float health, float maxHealth, float _1, float _2) {
    Value = health / maxHealth;
    _label.Text = health.ToString("0");
  }

}
