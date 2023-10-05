namespace Squiggles.Core.Scenes.UI.Menus;
using Godot;
using Squiggles.Core.Data;
using Squiggles.Core.Error;
using Squiggles.Core.Scenes.UI.Menus.Gameplay;

/// <summary>
/// The tab which contains the gameplay settings dynamically loaded from <see cref="SquigglesCoreConfigFile.GameplayConfig"/> and applied to <see cref="GameplaySettings"/>
/// </summary>
public partial class GameplayTab : PanelContainer {

  /// <summary>
  /// The root for the content of this panel
  /// </summary>
  [Export] private VBoxContainer _content;

  public override void _Ready() {
    var config = SC4X.Config?.GameplayConfig;
    if (!Debugging.Assert(config is not null, "")) {
      Print.Info("Failed to find configuration data", this);
      return;
    }
    foreach (var option in config.OptionsArray) {
      if (option is OptionBool obool) {
        CreateBool(obool);
      }
      else if (option is OptionComboSelect ocombo) {
        CreateComboSelect(ocombo);
      }
      else if (option is OptionSlider oslider) {
        CreateSlider(oslider);
      }
      else {
        Print.Warn($"Unhandled option resource! {option?.InternalName} of type [{option?.GetType().FullName}]", this);
      }
    }
  }

  private void CreateBool(OptionBool option) {
    AddHeading(option.InMenuName);

    var check = new CheckBox {
      Text = option.Value ? "Enabled" : "Disabled",
      ButtonPressed = option.Value
    };
    if (GameplaySettings.HasKey(option.InternalName)) {
      check.ButtonPressed = GameplaySettings.GetBool(option.InternalName);
      check.Text = check.ButtonPressed ? "Enabled" : "Disabled";
    }

    _content.AddChild(check);
    check.Toggled += (value) => GameplaySettings.SetBool(option.InternalName, value);
    check.Toggled += (value) => check.Text = value ? "Enabled" : "Disabled";
  }
  private void CreateComboSelect(OptionComboSelect option) {
    AddHeading(option.InMenuName);

    var btn = new OptionButton() {
      Text = option.InMenuName
    };
    foreach (var op in option.Options) {
      btn.AddItem(op);
    }
    btn.Select(option.DefaultSelection);

    if (GameplaySettings.HasKey(option.InternalName)) {

      var sel = GameplaySettings.GetString(option.InternalName);
      if (sel is not "") {
        for (var i = 0; i < btn.ItemCount; i++) {
          var txt = btn.GetItemText(i);
          if (txt == sel) {
            btn.Select(i);
            break;
          }
        }
      }
    }

    _content.AddChild(btn);
    btn.ItemSelected += (index) => {
      var name = btn.GetItemText((int)index);
      GameplaySettings.SetString(option.InternalName, name);
    };
  }

  private void CreateSlider(OptionSlider option) {
    AddHeading(option.InMenuName);

    var slider = new HSlider {
      MaxValue = option.MaxValue,
      MinValue = option.MinValue,
      Step = option.StepValue,
      AllowGreater = option.AllowGreater,
      AllowLesser = option.AllowLesser,
      Value = option.DefaultValue,
      TicksOnBorders = true,
      TickCount = 5,
      SizeFlagsHorizontal = SizeFlags.ExpandFill,
      SizeFlagsStretchRatio = 0.7f // fill 70%
    };

    var numLabel = new Label() {
      Text = ((float)slider.Value).ToString("g3"),
      SizeFlagsHorizontal = SizeFlags.ExpandFill,
      SizeFlagsStretchRatio = 0.3f // fill 30%
    };
    var hbox = new HBoxContainer {
      SizeFlagsHorizontal = SizeFlags.ExpandFill,
      Alignment = BoxContainer.AlignmentMode.Center
    };

    hbox.AddChild(slider);
    hbox.AddChild(numLabel);
    _content.AddChild(hbox);

    slider.ValueChanged += (val) => GameplaySettings.SetFloat(option.InternalName, (float)val);
    slider.ValueChanged += (val) => numLabel.Text = ((float)val).ToString("g3");

    if (GameplaySettings.HasKey(option.InternalName)) {
      slider.Value = GameplaySettings.GetFloat(option.InternalName);
    }
  }

  private void AddHeading(string name) {
    _content.AddChild(new HSeparator() {
      CustomMinimumSize = new(0, 16)
    });
    _content.AddChild(new Label {
      Text = name
    });
  }

  private void ApplyChanges() {
    _ = _content.Name; // accessing instance data to clear error against unnecessarily non-static methods. In this case, it's a callback for Godot.
    GameplaySettings.SaveSettings();
  }
}
