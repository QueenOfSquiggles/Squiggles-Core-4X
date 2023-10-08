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
    Environment = ThisIsYourMainScene.Config?.DefaultEnvironment;
    ApplyGraphicsSettings();
    Graphics.Instance.OnGraphicsSettingsChanged += ApplyGraphicsSettings;
  }

  public override void _ExitTree() => Graphics.Instance.OnGraphicsSettingsChanged -= ApplyGraphicsSettings;

  private void ApplyGraphicsSettings() {
    Environment.GlowEnabled = Graphics.Instance.Bloom;
    Environment.SsrEnabled = Graphics.Instance.SSR;
    Environment.SsaoEnabled = Graphics.Instance.SSAO;
    Environment.SsilEnabled = Graphics.Instance.SSIL;
    Environment.SdfgiEnabled = Graphics.Instance.SDFGI;

    if (ThisIsYourMainScene.Config?.EnableColourCorrection is true) {
      Environment.TonemapExposure = Graphics.Instance.TonemapExposure;
      Environment.AdjustmentBrightness = Graphics.Instance.Brightness;
      Environment.AdjustmentContrast = Graphics.Instance.Contrast;
      Environment.AdjustmentSaturation = Graphics.Instance.Saturation;
    }
  }
}
