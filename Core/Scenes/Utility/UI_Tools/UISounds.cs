using System;
using System.Threading.Tasks;
using Godot;
using queen.data;
using queen.error;
using queen.extension;

public partial class UISounds : Node
{
    [Export] private float PopUIScale = 1.1f;

    [Export] private NodePath path_select_sfx;
    [Export] private NodePath path_click_sfx;

    private AudioStreamPlayer sfx_select;
    private AudioStreamPlayer sfx_click;
    private static string VoiceID = "";
    private Tween? LastTween = null;

    public override void _Ready()
    {
        this.GetNode(path_select_sfx, out sfx_select);
        this.GetNode(path_click_sfx, out sfx_click);
        var parent = GetParent<Control>();
        if (VoiceID == "")
        {
            bool IsEnabled = ProjectSettings.GetSetting("audio/general/text_to_speech", false).AsBool();
            if (IsEnabled) VoiceID = DisplayServer.TtsGetVoicesForLanguage(OS.GetLocaleLanguage())[0];
        }

        if (queen.error.Debugging.Assert(parent != null, "UISounds node must be child of a Control node!"))
        {
            ConnectSignalsDelayed(parent, 50);
            parent.SetAnchorsPreset(Control.LayoutPreset.Center); // anchor center for uniform scaling
        }
    }

    private async void ConnectSignalsDelayed(Control parent, int delay)
    {
        await Task.Delay(delay);
        if (!IsInstanceValid(parent)) return;
        if (parent is TabContainer tabs)
        {
            tabs.TabButtonPressed += (index) => OnClick();
            return; // don't do select on container. That would get annoying
        }

        parent.FocusEntered += OnSelect;
        parent.MouseEntered += OnSelect;
        if (parent is Button btn)
            btn.Pressed += OnClick;
        else if (parent is HSlider slider)
            slider.DragStarted += OnClick;
        else if (parent is LinkButton link)
            link.Pressed += OnClick;
        else
            Print.Warn($"Unsupported parent type: {parent.GetType()}");

    }

    private void OnSelect()
    {
        if (Access.Instance.ReadVisibleTextAloud && GetParent().Get("text").VariantType != Variant.Type.Nil)
            DoTTS(GetParent().Get("text").AsString());
        sfx_select.Play();
        AnimatePop();

    }

    private void OnClick() => sfx_click.Play();

    private async void DoTTS(string msg)
    {
        if (VoiceID == "") return;
        DisplayServer.TtsStop();
        DisplayServer.TtsSpeak(msg, VoiceID);
    }

    private void AnimatePop()
    {
        if (LastTween is not null)
        {
            // prevents duplicate scaling where the tweens will stack to create an infinitely large Control
            LastTween.CustomStep(5.0f); // Forces a run to the end of the tween
            LastTween.Kill();
        }

        var parent = GetParent<Control>();
        if (parent is null) return;
        var start_size = parent.Size;
        var tween = GetTree().CreateTween().SetDefaultStyle();
        tween.SetTrans(Tween.TransitionType.Elastic);
        tween.TweenProperty(parent, "size", start_size * PopUIScale, 0.1);
        tween.TweenProperty(parent, "size", start_size, 0.05);
        LastTween = tween;

    }

    private void GetTTSVoiceID()
    {
        var voices = DisplayServer.TtsGetVoicesForLanguage(OS.GetLocaleLanguage());
        if (voices.Length <= 0)
        {
            Print.Warn($"Warning. No TTS voices installed for host system locale: {OS.GetLocaleLanguage()}. We will not be able to perform TTS lookups. If user wishes to use TTS on UI elements. Please install a TTS voice for your system. More information here: https://docs.godotengine.org/en/stable/tutorials/audio/text_to_speech.html#requirements-for-functionality");
        }
    }

}
