using Godot;
using queen.data;
using System;

public partial class WorldEnvSettingsCompliant : WorldEnvironment
{

    public override void _Ready()
    {
        ApplyGraphicsSettings();
        Graphics.Instance.OnGraphicsSettingsChanged += ApplyGraphicsSettings;
    }

    public override void _ExitTree()
    {
        Graphics.Instance.OnGraphicsSettingsChanged -= ApplyGraphicsSettings;
    }

    private void ApplyGraphicsSettings()
    {
        Environment.GlowEnabled = Graphics.Instance.Bloom;        
        Environment.SsrEnabled = Graphics.Instance.SSR;        
        Environment.SsaoEnabled = Graphics.Instance.SSAO;        
        Environment.SsilEnabled = Graphics.Instance.SSIL;        
        Environment.SdfgiEnabled = Graphics.Instance.SDFGI;        
        Environment.TonemapExposure = Graphics.Instance.TonemapExposure;
        Environment.AdjustmentBrightness = Graphics.Instance.Brightness;
        Environment.AdjustmentContrast = Graphics.Instance.Contrast;
        Environment.AdjustmentSaturation = Graphics.Instance.Saturation;
    }
}
