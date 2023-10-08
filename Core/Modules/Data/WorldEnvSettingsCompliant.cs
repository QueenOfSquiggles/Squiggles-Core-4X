namespace Squiggles.Core.Scenes.Utility;

using Godot;
using Squiggles.Core.Data;
using System;

/// <summary>
/// A World Environment that adheres to the <see cref="Graphics"/> settings. It also loads the <see cref="SquigglesCoreConfigFile.DefaultEnvironment"/> if none are assigned
/// </summary>
[GlobalClass]
public partial class WorldEnvSettingsCompliant : WorldEnvironment {

  public override void _Ready() {
    Environment = SC4X.Config?.DefaultEnvironment;
    ApplyGraphicsSettings();
    Graphics.OnGraphicsSettingsChanged += ApplyGraphicsSettings;
  }

  public override void _ExitTree() => Graphics.OnGraphicsSettingsChanged -= ApplyGraphicsSettings;

  private void ApplyGraphicsSettings() {
    Environment.GlowEnabled = Graphics.Bloom;
    Environment.SsrEnabled = Graphics.SSR;
    Environment.SsaoEnabled = Graphics.SSAO;
    Environment.SsilEnabled = Graphics.SSIL;
    Environment.SdfgiEnabled = Graphics.SDFGI;

    if (SC4X.Config?.EnableColourCorrection is true) {
      Environment.TonemapExposure = Graphics.TonemapExposure;
      Environment.AdjustmentBrightness = Graphics.Brightness;
      Environment.AdjustmentContrast = Graphics.Contrast;
      Environment.AdjustmentSaturation = Graphics.Saturation;
    }
  }
}
