using System;
using System.Threading.Tasks;
using Godot;
using queen.data;
using queen.error;
using queen.extension;

public partial class UISounds : Node
{
    [Export] private float _PopUIScale = 1.1f;

    [Export] private AudioStreamPlayer _SFXSelect;
    [Export] private AudioStreamPlayer _SFXClick;
    private static string _VoiceID = "";
    private Tween _LastTween = null;

    public override void _Ready()
    {
        var parent = GetParent<Control>();
        if (_VoiceID == "")
        {
            bool IsEnabled = ProjectSettings.GetSetting("audio/general/text_to_speech", false).AsBool();
            if (IsEnabled) _VoiceID = DisplayServer.TtsGetVoicesForLanguage(OS.GetLocaleLanguage())[0];
        }

        if (parent is not null)
        {
            ConnectSignalsDelayed(parent, 50);
            parent.SetAnchorsPreset(Control.LayoutPreset.Center); // anchor center for uniform scaling
        }
        else
        {
            // we know this is false, push the assertion failure
            Debugging.Assert(false, "UISounds node must be child of a Control node!");
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
        {
            var text_string = GetParent()?.Get("text").AsString();
            if (text_string is not null) DoTTS(text_string);
        }
        _SFXSelect?.Play();
        AnimatePop();

    }

    private void OnClick() => _SFXClick?.Play();

    private static async void DoTTS(string msg)
    {
        if (_VoiceID == "") return;
        DisplayServer.TtsStop();
        await Task.Delay(300);
        DisplayServer.TtsSpeak(msg, _VoiceID);
    }

    private void AnimatePop()
    {
        if (_LastTween is not null)
        {
            // prevents duplicate scaling where the tweens will stack to create an infinitely large Control
            _LastTween.CustomStep(5.0f); // Forces a run to the end of the tween
            _LastTween.Kill();
        }

        var parent = GetParent<Control>();
        if (parent is null) return;
        var start_size = parent.Size;
        var tween = GetTree().CreateTween().SetDefaultStyle();
        tween.SetTrans(Tween.TransitionType.Elastic);
        tween.TweenProperty(parent, "size", start_size * _PopUIScale, 0.1);
        tween.TweenProperty(parent, "size", start_size, 0.05);
        _LastTween = tween;

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
